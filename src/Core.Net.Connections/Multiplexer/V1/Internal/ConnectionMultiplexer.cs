using System.Buffers;
using System.Collections.Immutable;
using Omnius.Core.Base;
using Omnius.Core.Pipelines;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal;

internal sealed partial class ConnectionMultiplexer : AsyncDisposableBase
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly IConnection _connection;
    private readonly IBytesPool _bytesPool;
    private readonly IBatchActionDispatcher _batchActionDispatcher;
    private readonly ISystemClock _systemClock;
    private readonly OmniConnectionMultiplexerOptions _options;

    private SessionOptions? _sessionOptions;

    private uint _nextStreamId;

    private SemaphoreSlim? _requestingStreamSemaphore;

    private BoundedMessagePipe? _sendStreamRequestPipe;
    private BoundedMessagePipe<uint>? _sendStreamRequestAcceptedPipe;
    private BoundedMessagePipe? _receiveStreamRequestPipe;
    private BoundedMessagePipe<uint>? _receiveStreamRequestAcceptedPipe;

    private ImmutableDictionary<uint, StreamConnection> _streamConnectionMap = ImmutableDictionary.Create<uint, StreamConnection>();

    private DateTime _lastReceivedTime = DateTime.MinValue;
    private DateTime _lastSentTime = DateTime.MinValue;

    private ErrorCode _sendErrorCode = ErrorCode.None;
    private ErrorCode _receiveErrorCode = ErrorCode.None;

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private BatchAction? _batchAction;

    private readonly object _lockObject = new();
    private bool _canceled = false;

    private readonly List<IDisposable> _disposables = new();

    public ConnectionMultiplexer(IConnection connection, IBatchActionDispatcher batchActionDispatcher, ISystemClock systemClock, IBytesPool bytesPool, OmniConnectionMultiplexerOptions options)
    {
        _connection = connection;
        _batchActionDispatcher = batchActionDispatcher;
        _systemClock = systemClock;
        _bytesPool = bytesPool;
        _options = options;

        _nextStreamId = (uint)((_options.Type == OmniConnectionMultiplexerType.Connected) ? 0 : 1);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        this.Cancel();

        foreach (var connection in _streamConnectionMap.Values)
        {
            await connection.DisposeAsync();
        }

        _streamConnectionMap = _streamConnectionMap.Clear();

        _requestingStreamSemaphore!.Dispose();
        _sendStreamRequestPipe!.Dispose();
        _sendStreamRequestAcceptedPipe!.Dispose();
        _receiveStreamRequestPipe!.Dispose();
        _receiveStreamRequestAcceptedPipe!.Dispose();
    }

    public bool IsConnected => _connection.IsConnected;

    public async ValueTask HandshakeAsync(CancellationToken cancellationToken = default)
    {
        var otherProfileMessage = await this.ExchangeProfileMessageAsync(cancellationToken);

        _sessionOptions = new SessionOptions(
            PacketReceiveTimeout: TimeSpan.FromMilliseconds(otherProfileMessage.PacketReceiveTimeoutMilliseconds),
            MaxStreamRequestQueueSize: otherProfileMessage.MaxStreamRequestQueueSize,
            MaxDataSize: otherProfileMessage.MaxStreamDataSize,
            MaxDataQueueSize: otherProfileMessage.MaxStreamDataQueueSize);

        _sendStreamRequestPipe = new BoundedMessagePipe((int)_sessionOptions.MaxStreamRequestQueueSize);
        _sendStreamRequestAcceptedPipe = new BoundedMessagePipe<uint>((int)_sessionOptions.MaxStreamRequestQueueSize);
        _receiveStreamRequestPipe = new BoundedMessagePipe((int)_options.MaxStreamRequestQueueSize);
        _receiveStreamRequestAcceptedPipe = new BoundedMessagePipe<uint>((int)_options.MaxStreamRequestQueueSize);

        _requestingStreamSemaphore = new SemaphoreSlim(1, (int)_sessionOptions.MaxStreamRequestQueueSize);

        _batchAction = new BatchAction(this, this.HandleException);
        _batchActionDispatcher.Register(_batchAction);
    }

    private void HandleException(Exception e)
    {
        this.Cancel();

        _logger.Debug(e, "ConnectionMultiplexer Exception");
    }

    private void Cancel()
    {
        lock (_lockObject)
        {
            if (_canceled) return;
            _canceled = true;

            if (_batchAction is not null) _batchActionDispatcher.Unregister(_batchAction);
            _cancellationTokenSource.Cancel();
        }
    }

    private async ValueTask<ProfileMessage> ExchangeProfileMessageAsync(CancellationToken cancellationToken = default)
    {
        var myProfileMessage = new ProfileMessage((uint)_options.PacketReceiveTimeout.TotalMilliseconds, _options.MaxStreamRequestQueueSize, _options.MaxStreamDataSize, _options.MaxStreamDataQueueSize);
        var enqueueTask = _connection.Sender.SendAsync(myProfileMessage, cancellationToken).AsTask();
        var dequeueTask = _connection.Receiver.ReceiveAsync<ProfileMessage>(cancellationToken).AsTask();

        await Task.WhenAll(enqueueTask, dequeueTask);
        var otherProfileMessage = dequeueTask.Result;
        return otherProfileMessage;
    }

    private void InternalSend()
    {
        if (_canceled) return;

        try
        {
            _connection.Sender.TrySend(this.InternalSend);
        }
        catch (Exception)
        {
            throw;
        }
    }

    internal void InternalSend(IBufferWriter<byte> bufferWriter)
    {
        if (_sessionOptions is null) return;
        if (_sendStreamRequestPipe is null) return;
        if (_sendStreamRequestAcceptedPipe is null) return;

        if (_sendErrorCode != ErrorCode.None)
        {
            var builder = new PacketBuilder(bufferWriter);
            builder.WriteSessionError(_sendErrorCode);

            this.Cancel();

            _lastSentTime = _systemClock.GetUtcNow();
            return;
        }

        if (_sendStreamRequestPipe.Reader.TryRead())
        {
            var builder = new PacketBuilder(bufferWriter);
            builder.WriteStreamRequest();

            _lastSentTime = _systemClock.GetUtcNow();
            return;
        }

        if (_sendStreamRequestAcceptedPipe.Reader.TryRead(out var streamRequestAcceptedStreamId))
        {
            var builder = new PacketBuilder(bufferWriter);
            builder.WriteStreamRequestAccepted(streamRequestAcceptedStreamId);

            _lastSentTime = _systemClock.GetUtcNow();
            return;
        }

        foreach (var (streamId, connection) in _streamConnectionMap)
        {
            try
            {
                if (connection.SendDataReader.TryRead(out var data))
                {
                    var builder = new PacketBuilder(bufferWriter);
                    builder.WriteStreamData(streamId, data);
                    _bytesPool.Array.Return(data.Array!);

                    _lastSentTime = _systemClock.GetUtcNow();
                    return;
                }

                if (connection.SendDataAcceptedReader.TryRead())
                {
                    var builder = new PacketBuilder(bufferWriter);
                    builder.WriteStreamDataAccepted(streamId);

                    _lastSentTime = _systemClock.GetUtcNow();
                    return;
                }

                if (connection.SendFinishReader.TryRead())
                {
                    var builder = new PacketBuilder(bufferWriter);
                    builder.WriteStreamFinish(streamId);

                    _streamConnectionMap = _streamConnectionMap.Remove(streamId);
                    connection.InternalDispose();

                    _lastSentTime = _systemClock.GetUtcNow();
                    return;
                }
            }
            catch (ObjectDisposedException e)
            {
                _logger.Debug(e, "StreamConnection Disposed");
            }
        }

        if ((DateTime.UtcNow - _lastSentTime) > (_sessionOptions.PacketReceiveTimeout / 2))
        {
            var builder = new PacketBuilder(bufferWriter);
            builder.WriteKeepAlive();

            _lastSentTime = _systemClock.GetUtcNow();
            return;
        }
    }

    private void Receive()
    {
        if (_canceled) return;

        try
        {
            _connection.Receiver.TryReceive(this.InternalReceive);
        }
        catch (Exception)
        {
            throw;
        }
    }

    internal void InternalReceive(ReadOnlySequence<byte> sequence)
    {
        if (_receiveStreamRequestPipe is null) return;
        if (_receiveStreamRequestAcceptedPipe is null) return;

        if (!PacketParser.TryParsePacketType(ref sequence, out var packetType))
        {
            _sendErrorCode = ErrorCode.PacketTypeInvalid;
            return;
        }

        switch (packetType)
        {
            case PacketType.KeepAlive:
                _lastReceivedTime = _systemClock.GetUtcNow();
                break;
            case PacketType.StreamRequest:
                {
                    if (!_receiveStreamRequestPipe.Writer.TryWrite())
                    {
                        _sendErrorCode = ErrorCode.StreamRequestQueueOverflow;
                        return;
                    }

                    _lastReceivedTime = _systemClock.GetUtcNow();
                    break;
                }
            case PacketType.StreamRequestAccepted:
                {
                    if (!PacketParser.TryParseStreamId(ref sequence, out var streamId))
                    {
                        _sendErrorCode = ErrorCode.PacketFormatInvalid;
                        return;
                    }

                    if (!_receiveStreamRequestAcceptedPipe.Writer.TryWrite(streamId))
                    {
                        _sendErrorCode = ErrorCode.StreamRequestAcceptedQueueOverflow;
                        return;
                    }

                    _lastReceivedTime = _systemClock.GetUtcNow();
                    break;
                }
            case PacketType.StreamData:
                {
                    if (!PacketParser.TryParseStreamId(ref sequence, out var streamId))
                    {
                        _sendErrorCode = ErrorCode.PacketFormatInvalid;
                        return;
                    }

                    if (!_streamConnectionMap.TryGetValue(streamId, out var connection))
                    {
                        _sendErrorCode = ErrorCode.StreamIdInvalid;
                        return;
                    }

                    if (sequence.Length == 0) return;

                    if (sequence.Length > _options.MaxStreamDataSize)
                    {
                        _sendErrorCode = ErrorCode.StreamDataSizeTooLarge;
                        return;
                    }

                    var buffer = _bytesPool.Array.Rent((int)sequence.Length);
                    sequence.CopyTo(buffer.AsSpan());
                    if (!connection.ReceiveDataWriter.TryWrite(new ArraySegment<byte>(buffer, 0, (int)sequence.Length)))
                    {
                        _sendErrorCode = ErrorCode.StreamDataQueueOverflow;
                        return;
                    }

                    _lastReceivedTime = _systemClock.GetUtcNow();
                    break;
                }
            case PacketType.StreamDataAccepted:
                {
                    if (!PacketParser.TryParseStreamId(ref sequence, out var streamId))
                    {
                        _sendErrorCode = ErrorCode.PacketFormatInvalid;
                        return;
                    }

                    if (!_streamConnectionMap.TryGetValue(streamId, out var connection))
                    {
                        _sendErrorCode = ErrorCode.StreamIdInvalid;
                        return;
                    }

                    connection.ReceiveDataAcceptedCaller.Call();

                    _lastReceivedTime = _systemClock.GetUtcNow();
                    break;
                }
            case PacketType.StreamFinish:
                {
                    if (!PacketParser.TryParseStreamId(ref sequence, out var streamId))
                    {
                        _sendErrorCode = ErrorCode.PacketFormatInvalid;
                        return;
                    }

                    if (!_streamConnectionMap.TryGetValue(streamId, out var connection))
                    {
                        _sendErrorCode = ErrorCode.StreamIdInvalid;
                        return;
                    }

                    connection.InternalFinish();

                    _lastReceivedTime = _systemClock.GetUtcNow();
                    break;
                }
            case PacketType.SessionError:
                {
                    if (!PacketParser.TryParseErrorCode(ref sequence, out var errorCode))
                    {
                        _sendErrorCode = ErrorCode.PacketFormatInvalid;
                        return;
                    }

                    _receiveErrorCode = errorCode;

                    this.Cancel();

                    _lastReceivedTime = _systemClock.GetUtcNow();
                    break;
                }
            case PacketType.SessionFinish:
                {
                    this.Cancel();

                    _lastReceivedTime = _systemClock.GetUtcNow();
                    break;
                }
        }
    }

    public async ValueTask<IConnection> ConnectAsync(CancellationToken cancellationToken = default)
    {
        await _sendStreamRequestPipe!.Writer.WriteAsync(cancellationToken);

        var streamId = await _receiveStreamRequestAcceptedPipe!.Reader.ReadAsync(cancellationToken);

        var connection = this.AddConnection(streamId);
        return connection;
    }

    public async ValueTask<IConnection> AcceptAsync(CancellationToken cancellationToken = default)
    {
        await _receiveStreamRequestPipe!.Reader.ReadAsync(cancellationToken);

        var streamId = this.GenNextStreamId();

        await _sendStreamRequestAcceptedPipe!.Writer.WriteAsync(streamId, cancellationToken);

        var connection = this.AddConnection(streamId);
        return connection;
    }

    private StreamConnection AddConnection(uint streamId)
    {
        var connection = new StreamConnection((int)_sessionOptions!.MaxDataQueueSize, (int)_options.MaxStreamDataQueueSize, _bytesPool, _cancellationTokenSource.Token);
        _streamConnectionMap = _streamConnectionMap.Add(streamId, connection);
        return connection;
    }

    private uint GenNextStreamId()
    {
        return Interlocked.Add(ref _nextStreamId, 2);
    }
}
