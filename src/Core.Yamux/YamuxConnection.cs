using System.Threading.Channels;

using Omnius.Core.Base;

namespace Omnius.Core.Yamux;

public enum YamuxMode
{
    Client,
    Server,
}

public sealed class YamuxConnection : IAsyncDisposable
{
    private readonly Stream _transport;
    private readonly YamuxConfig _config;
    private readonly IBytesPool _bytesPool;

    private uint _nextStreamId;

    private readonly Channel<Frame> _outgoing;
    private readonly Channel<YamuxStream> _inbound;
    private readonly Dictionary<uint, YamuxStream> _streams = new();

    private readonly Task _readLoop;
    private readonly Task _writeLoop;

    private ManualResetSignal _connectStreamSignal = new(false);
    private int _pendingAckCount;

    private readonly CancellationTokenSource _cts = new();
    private bool _closed;
    private readonly object _lockObject = new();

    public YamuxConnection(Stream transport, YamuxConfig? config, YamuxMode mode, IBytesPool bytesPool)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        _config = config ?? new YamuxConfig();
        _config.EnsureWindowLimits();
        this.Mode = mode;
        _bytesPool = bytesPool ?? throw new ArgumentNullException(nameof(bytesPool));

        _nextStreamId = mode == YamuxMode.Client ? 1u : 2u;

        _outgoing = Channel.CreateUnbounded<Frame>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = true
        });

        _inbound = Channel.CreateUnbounded<YamuxStream>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = true,
            AllowSynchronousContinuations = true
        });

        _readLoop = Task.Run(this.ReadLoopAsync);
        _writeLoop = Task.Run(this.WriteLoopAsync);
    }

    public YamuxMode Mode { get; }
    public int StreamCount => _streams.Count;

    public async ValueTask<YamuxStream?> AcceptStreamAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _inbound.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (ChannelClosedException)
        {
            return null;
        }
    }

    public async ValueTask<YamuxStream> ConnectStreamAsync(CancellationToken cancellationToken = default)
    {
        for (; ; )
        {
            cancellationToken.ThrowIfCancellationRequested();

            lock (_lockObject)
            {
                if (_closed) throw new YamuxConnectionClosedException("Connection is closed.");

                if (_streams.Count < _config.MaxNumStreams && _pendingAckCount < YamuxConstants.MaxAckBacklog)
                {
                    var id = _nextStreamId;
                    if (id == 0 || id > uint.MaxValue - 2) throw new YamuxException("No more stream IDs available.");

                    _nextStreamId += 2;
                    var stream = YamuxStream.CreateOutbound(this, _config, id, _bytesPool);
                    _streams[id] = stream;
                    _pendingAckCount++;

                    return stream;
                }

                _connectStreamSignal.Reset();
            }

            await _connectStreamSignal.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task CloseAsync(CancellationToken cancellationToken = default)
    {
        if (!this.BeginClose()) return;

        var goAway = Frame.GoAway(GoAwayCode.Normal);
        if (!_outgoing.Writer.TryWrite(goAway))
        {
            goAway.Dispose();
        }

        _outgoing.Writer.TryComplete();

        try
        {
            await _writeLoop.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
        }

        _cts.Cancel();

        try
        {
            await _readLoop.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
        }

        this.CloseAllStreams();
    }

    public async ValueTask DisposeAsync()
    {
        await this.CloseAsync().ConfigureAwait(false);
        _cts.Dispose();
    }

    internal void EnqueueFrame(Frame frame)
    {
        if (!_outgoing.Writer.TryWrite(frame))
        {
            frame.Dispose();
            throw new YamuxConnectionClosedException("Connection is closed.");
        }
    }

    internal void NotifyStreamClosed(uint streamId, YamuxStream stream)
    {
        lock (_lockObject)
        {
            if (_streams.Remove(streamId))
            {
                if (stream.IsOutbound && stream.IsPendingAck && _pendingAckCount > 0)
                {
                    _pendingAckCount--;
                }

                _connectStreamSignal.Set();
            }
        }
    }

    private async Task ReadLoopAsync()
    {
        try
        {
            while (!_cts.IsCancellationRequested)
            {
                var frame = await FrameCodec.ReadAsync(_transport, _bytesPool, _cts.Token).ConfigureAwait(false);
                if (frame is null) break;

                try
                {
                    await this.HandleFrameAsync(frame).ConfigureAwait(false);
                }
                finally
                {
                    frame.Dispose();
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (YamuxProtocolException)
        {
            await this.TerminateAsync(GoAwayCode.ProtocolError).ConfigureAwait(false);
        }
        catch (Exception)
        {
            await this.TerminateAsync(GoAwayCode.InternalError).ConfigureAwait(false);
        }
        finally
        {
            this.BeginClose();
            _inbound.Writer.TryComplete();
            _outgoing.Writer.TryComplete();
            this.CloseAllStreams();
        }
    }

    private async Task WriteLoopAsync()
    {
        try
        {
            await foreach (var frame in _outgoing.Reader.ReadAllAsync(_cts.Token))
            {
                try
                {
                    await FrameCodec.WriteAsync(_transport, frame, _bytesPool, _cts.Token).ConfigureAwait(false);
                }
                finally
                {
                    frame.Dispose();
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            while (_outgoing.Reader.TryRead(out var frame))
            {
                frame.Dispose();
            }
        }
    }

    private async ValueTask HandleFrameAsync(Frame frame)
    {
        var header = frame.Header;

        if ((header.Flags & FrameFlags.Ack) != 0 && (header.Tag == FrameTag.Data || header.Tag == FrameTag.WindowUpdate))
        {
            if (this.TryGetStream(header.StreamId, out var ackStream))
            {
                if (ackStream.MarkAcknowledgedByRemote())
                {
                    lock (_lockObject)
                    {
                        if (_pendingAckCount > 0)
                        {
                            _pendingAckCount--;
                        }

                        _connectStreamSignal.Set();
                    }
                }
            }
        }

        switch (header.Tag)
        {
            case FrameTag.Data:
                await this.HandleDataAsync(frame).ConfigureAwait(false);
                return;
            case FrameTag.WindowUpdate:
                await this.HandleWindowUpdateAsync(frame).ConfigureAwait(false);
                return;
            case FrameTag.Ping:
                await this.HandlePingAsync(frame).ConfigureAwait(false);
                return;
            case FrameTag.GoAway:
                this.BeginClose();
                return;
            default:
                throw new YamuxProtocolException("Unknown frame tag.");
        }
    }

    private async ValueTask HandleDataAsync(Frame frame)
    {
        var header = frame.Header;
        var flags = header.Flags;
        var streamId = header.StreamId;

        if ((flags & FrameFlags.Rst) != 0)
        {
            if (this.TryGetStream(streamId, out var rstStream))
            {
                rstStream.ReceiveReset();
            }

            return;
        }

        var fin = (flags & FrameFlags.Fin) != 0;
        var syn = (flags & FrameFlags.Syn) != 0;

        if (syn)
        {
            if (!this.IsValidRemoteId(streamId, FrameTag.Data))
            {
                await this.TerminateAsync(GoAwayCode.ProtocolError).ConfigureAwait(false);
                return;
            }

            if (frame.Body.Length > YamuxConstants.DefaultCredit)
            {
                await this.TerminateAsync(GoAwayCode.ProtocolError).ConfigureAwait(false);
                return;
            }

            bool tooMany = false;
            YamuxStream? stream = null;

            lock (_lockObject)
            {
                if (_streams.ContainsKey(streamId)) throw new YamuxProtocolException("Stream already exists.");

                if (_streams.Count >= _config.MaxNumStreams)
                {
                    tooMany = true;
                }
                else
                {
                    stream = YamuxStream.CreateInbound(this, _config, streamId, YamuxConstants.DefaultCredit, _bytesPool);
                    _streams[streamId] = stream;
                }
            }

            if (tooMany)
            {
                await this.TerminateAsync(GoAwayCode.InternalError).ConfigureAwait(false);
                return;
            }

            if (stream is null) return;

            var data = frame.TakeBody();
            stream.ReceiveData(data, fin);
            _inbound.Writer.TryWrite(stream);
            return;
        }

        if (this.TryGetStream(streamId, out var target))
        {
            var data = frame.TakeBody();
            target.ReceiveData(data, fin);
        }
    }

    private async ValueTask HandleWindowUpdateAsync(Frame frame)
    {
        var header = frame.Header;
        var flags = header.Flags;
        var streamId = header.StreamId;

        if ((flags & FrameFlags.Rst) != 0)
        {
            if (this.TryGetStream(streamId, out var rstStream))
            {
                rstStream.ReceiveReset();
            }

            return;
        }

        var fin = (flags & FrameFlags.Fin) != 0;
        var syn = (flags & FrameFlags.Syn) != 0;

        if (syn)
        {
            if (!this.IsValidRemoteId(streamId, FrameTag.WindowUpdate))
            {
                await this.TerminateAsync(GoAwayCode.ProtocolError).ConfigureAwait(false);
                return;
            }

            bool tooMany = false;
            YamuxStream? stream = null;

            lock (_lockObject)
            {
                if (_streams.ContainsKey(streamId)) throw new YamuxProtocolException("Stream already exists.");

                if (_streams.Count >= _config.MaxNumStreams)
                {
                    tooMany = true;
                }
                else
                {
                    var initialSendWindow = YamuxConstants.DefaultCredit + header.Length;
                    stream = YamuxStream.CreateInbound(this, _config, streamId, initialSendWindow, _bytesPool);
                    _streams[streamId] = stream;
                }
            }

            if (tooMany)
            {
                await this.TerminateAsync(GoAwayCode.InternalError).ConfigureAwait(false);
                return;
            }

            if (stream is null) return;

            if (fin)
            {
                stream.ReceiveWindowUpdate(0, fin);
            }

            _inbound.Writer.TryWrite(stream);
            return;
        }

        if (this.TryGetStream(streamId, out var target))
        {
            target.ReceiveWindowUpdate(header.Length, fin);
        }
    }

    private async ValueTask HandlePingAsync(Frame frame)
    {
        var header = frame.Header;
        if ((header.Flags & FrameFlags.Ack) != 0) return;

        var pong = Frame.Ping(header.Length, FrameFlags.Ack);
        if (!_outgoing.Writer.TryWrite(pong))
        {
            pong.Dispose();
        }
    }

    private async ValueTask TerminateAsync(GoAwayCode code)
    {
        if (this.BeginClose())
        {
            var goAway = Frame.GoAway(code);
            if (!_outgoing.Writer.TryWrite(goAway))
            {
                goAway.Dispose();
            }

            _outgoing.Writer.TryComplete();
            _cts.Cancel();
        }
    }

    private bool TryGetStream(uint streamId, out YamuxStream stream)
    {
        lock (_lockObject)
        {
            return _streams.TryGetValue(streamId, out stream!);
        }
    }

    private bool IsValidRemoteId(uint streamId, FrameTag tag)
    {
        if (tag == FrameTag.Ping || tag == FrameTag.GoAway)
        {
            return streamId == 0;
        }

        return this.Mode == YamuxMode.Client
            ? streamId % 2 == 0
            : streamId % 2 == 1;
    }

    private bool BeginClose()
    {
        lock (_lockObject)
        {
            if (_closed) return false;
            _closed = true;

            _connectStreamSignal.Set();
        }

        return true;
    }

    private void CloseAllStreams()
    {
        YamuxStream[] streams;

        lock (_lockObject)
        {
            streams = _streams.Values.ToArray();
            _streams.Clear();
            _pendingAckCount = 0;
        }

        foreach (var stream in streams)
        {
            stream.MarkConnectionClosed();
        }
    }
}
