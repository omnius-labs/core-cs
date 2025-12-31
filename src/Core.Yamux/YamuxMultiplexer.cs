using System.Buffers;
using Microsoft.Extensions.Logging;
using Omnius.Yamux.Internal;

namespace Omnius.Yamux;

// <summary>
// Yamux session type. It affects stream ID generation.
// </summary>
public enum YamuxSessionType
{
    Client,
    Server,
}

public class YamuxMultiplexer : IAsyncDisposable
{
    private readonly Stream _networkStream;
    private readonly YamuxOptions _options;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger _logger;

    private readonly ArrayPool<byte> _bytesPool = ArrayPool<byte>.Shared;
    private readonly StreamRegistry _streamRegistry;
    private readonly FrameReceiver _frameReceiver;
    private readonly FrameSender _frameSender;

    private Task? _keepAliveTask = null;

    private uint _pingId;
    private readonly Dictionary<uint, TaskCompletionSource> _pingTcsMap = new();
    private readonly SemaphoreSlim _pingAckSemaphore = new(1);
    private readonly object _pingLock = new();

    private GoAwayCode _remoteGoAwayCode = GoAwayCode.None;
    private GoAwayCode _localGoAwayCode = GoAwayCode.None;
    private YamuxErrorCode _shutdownErrorCode;

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private int _closed = 0;

    public YamuxMultiplexer(YamuxSessionType sessionType, Stream stream, YamuxOptions options, TimeProvider timeProvider, ILogger logger)
    {
        _networkStream = stream;
        _options = options;
        _options.Verify();
        _timeProvider = timeProvider;
        _logger = logger;

        _streamRegistry = new StreamRegistry(sessionType, (streamId, state) => new YamuxStream(this, streamId, state, _bytesPool, _timeProvider, _logger, _cancellationTokenSource.Token), _options);

        var frameReader = new FrameReader(_networkStream, _bytesPool, _logger);
        var frameWriter = new FrameWriter(_networkStream);
        var streamMessageHandler = new StreamMessageHandler(_streamRegistry, frameReader, _networkStream, () => _localGoAwayCode, this.SendFrameFireAndForgetAsync, _logger);
        _frameSender = new FrameSender(frameWriter, _options, _logger, this.ExitAsync, _cancellationTokenSource.Token);
        _frameReceiver = new FrameReceiver(frameReader, streamMessageHandler, this.HandlePingAsync, this.HandleGoAwayAsync, _logger, this.ExitAsync, _cancellationTokenSource.Token);

        _frameSender.Start();
        _frameReceiver.Start();

        if (options.EnableKeepAlive)
        {
            _keepAliveTask = this.KeepAliveLoop(_cancellationTokenSource.Token);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await this.CloseAsync();

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

        await _frameSender.Completion;
        await _frameReceiver.Completion;
        if (_keepAliveTask != null) await _keepAliveTask;
    }

    public YamuxOptions Options => _options;
    public int StreamCount => _streamRegistry.Count;

    private CancellationTokenSource GetMixedCancellationToken(CancellationToken cancellationToken = default)
    {
        return CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);
    }

    public async ValueTask<YamuxStream> ConnectStreamAsync(CancellationToken cancellationToken = default)
    {
        using var cts = this.GetMixedCancellationToken(cancellationToken);
        cancellationToken = cts.Token;

        if (_shutdownErrorCode != YamuxErrorCode.None) throw new YamuxException(_shutdownErrorCode);
        if (_remoteGoAwayCode != GoAwayCode.None) throw new YamuxException(YamuxErrorCode.RemoteGoAway);

        YamuxStream stream = await _streamRegistry.CreateOutboundAsync(cancellationToken);

        await stream.SendWindowUpdateAsync(cancellationToken);

        await stream.WaitForEstablishedAsync(cancellationToken);

        return stream;
    }

    public async ValueTask<YamuxStream> AcceptStreamAsync(CancellationToken cancellationToken = default)
    {
        using var cts = this.GetMixedCancellationToken(cancellationToken);
        cancellationToken = cts.Token;

        if (_shutdownErrorCode != YamuxErrorCode.None) throw new YamuxException(_shutdownErrorCode);
        if (_remoteGoAwayCode != GoAwayCode.None) throw new YamuxException(YamuxErrorCode.RemoteGoAway);

        YamuxStream stream = await _streamRegistry.AcceptAsync(cancellationToken);
        await stream.SendWindowUpdateAsync(cancellationToken);
        return stream;
    }

    internal void RemoveStream(uint id)
    {
        _streamRegistry.Remove(id);
    }

    public async ValueTask CloseAsync()
    {
        if (Interlocked.CompareExchange(ref _closed, 1, 0) != 0) return;

        _frameSender.Complete();
        _frameReceiver.Complete();

        _streamRegistry.CompleteAccepting();

        foreach (var stream in _streamRegistry.Streams)
        {
            stream.ForceClose();
        }

        _networkStream.Close();
    }

    public async ValueTask GoAwayAsync(CancellationToken cancellationToken = default)
    {
        _localGoAwayCode = GoAwayCode.Normal;

        var header = new Header(MessageType.GoAway, MessageFlag.None, 0, (uint)GoAwayCode.Normal);
        await this.SendFrameAsync(header, ReadOnlyMemory<byte>.Empty);
    }

    private async Task KeepAliveLoop(CancellationToken cancellationToken = default)
    {
        await Task.Delay(1, cancellationToken).ConfigureAwait(false);

        try
        {
            for (; ; )
            {
                await Task.Delay(_options.KeepAliveInterval, cancellationToken);
                await this.PingAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (TimeoutException)
        {
            await this.ExitAsync(YamuxErrorCode.Timeout);
        }
        catch (YamuxException e)
        {
            await this.ExitAsync(e.ErrorCode);
        }
    }

    public async ValueTask<TimeSpan> PingAsync(CancellationToken cancellationToken = default)
    {
        using var cts = this.GetMixedCancellationToken(cancellationToken);
        cancellationToken = cts.Token;

        var timestamp = _timeProvider.GetTimestamp();

        uint pingId;
        var pingTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        CancellationTokenRegistration ctr = default;

        lock (_pingLock)
        {
            pingId = _pingId++;
            _pingTcsMap.Add(pingId, pingTcs);
        }

        try
        {
            ctr = cancellationToken.Register(static state =>
            {
                ((TaskCompletionSource)state!).TrySetCanceled();
            }, pingTcs);

            var header = new Header(MessageType.Ping, MessageFlag.SYN, 0, pingId);
            await this.SendFrameAsync(header, ReadOnlyMemory<byte>.Empty, cancellationToken);

            var timeoutTask = _timeProvider.Delay(_options.PingTimeout, CancellationToken.None);

            var completedTask = await Task.WhenAny(timeoutTask, pingTcs.Task);

            if (completedTask == timeoutTask)
            {
                if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException(cancellationToken);
                throw new TimeoutException();
            }

            await pingTcs.Task;
            return _timeProvider.GetElapsedTime(timestamp);
        }
        finally
        {
            ctr.Dispose();
            lock (_pingLock)
            {
                _pingTcsMap.Remove(pingId);
            }
        }
    }

    internal async ValueTask SendFrameAsync(Header header, ReadOnlyMemory<byte> payload, CancellationToken cancellationToken = default)
    {
        var headerBytes = header.GetBytes();
        var payloadBytes = payload.ToArray();
        var result = await _frameSender.EnqueueAsync(headerBytes, payloadBytes, cancellationToken);

        if (result != YamuxErrorCode.None)
        {
            throw new YamuxException(result);
        }
    }

    internal async ValueTask SendFrameFireAndForgetAsync(Header header, ReadOnlyMemory<byte> payload, CancellationToken cancellationToken = default)
    {
        var headerBytes = header.GetBytes();
        var payloadBytes = payload.ToArray();
        await _frameSender.EnqueueFireAndForgetAsync(headerBytes, payloadBytes, cancellationToken);
    }

    private async ValueTask HandlePingAsync(Header header, CancellationToken cancellationToken = default)
    {
        var pingId = header.Length;

        if (header.Flags.HasFlag(MessageFlag.SYN))
        {
            await _pingAckSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            _ = Task.Run(async () =>
            {
                try
                {
                    var header2 = new Header(MessageType.Ping, MessageFlag.ACK, 0, pingId);
                    await this.SendFrameFireAndForgetAsync(header2, ReadOnlyMemory<byte>.Empty, cancellationToken);
                }
                finally
                {
                    _pingAckSemaphore.Release();
                }
            });
        }

        if (header.Flags.HasFlag(MessageFlag.ACK))
        {
            lock (_pingLock)
            {
                if (_pingTcsMap.TryGetValue(pingId, out var task))
                {
                    task.TrySetResult();
                }
            }
        }
    }

    private async ValueTask HandleGoAwayAsync(Header header, CancellationToken cancellationToken = default)
    {
        var code = header.Length;

        switch (code)
        {
            case (uint)GoAwayCode.Normal:
                _remoteGoAwayCode = GoAwayCode.Normal;
                break;
            case (uint)GoAwayCode.ProtocolError:
                _logger.LogWarning("yamux: received protocol error go away");
                throw new YamuxException(YamuxErrorCode.ProtocolError);
            case (uint)GoAwayCode.InternalError:
                _logger.LogWarning("yamux: received internal error go away");
                throw new YamuxException(YamuxErrorCode.InternalError);
            default:
                _logger.LogWarning("yamux: received unexpected go away code: {0}", code);
                throw new YamuxException(YamuxErrorCode.Unexpected);
        }
    }

    private async ValueTask ExitAsync(YamuxErrorCode errorCode)
    {
        if (_shutdownErrorCode != YamuxErrorCode.None) return;
        _shutdownErrorCode = errorCode;
        await this.CloseAsync();
    }
}
