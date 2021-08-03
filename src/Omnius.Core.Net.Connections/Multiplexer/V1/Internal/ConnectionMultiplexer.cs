using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Helpers;
using Omnius.Core.Pipelines;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal
{
    internal sealed partial class ConnectionMultiplexer : AsyncDisposableBase
    {
        private readonly IConnection _connection;
        private readonly IBytesPool _bytesPool;
        private readonly BatchActionDispatcher _batchActionDispatcher;
        private readonly ConnectionMultiplexerOptions _options;

        private SessionOptions? _sessionOptions;

        private uint _nextStreamId;

        private SemaphoreSlim? _requestingStreamSemaphore;

        private BoundedMessagePipe? _sendStreamRequestPipe;
        private BoundedMessagePipe<uint>? _sendStreamRequestAcceptedPipe;
        private BoundedMessagePipe? _receiveStreamRequestPipe;
        private BoundedMessagePipe<uint>? _receiveStreamRequestAcceptedPipe;

        private readonly ImmutableDictionary<uint, StreamConnection> _streamConnectionMap = ImmutableDictionary.Create<uint, StreamConnection>();

        private DateTime _lastReceivedTime = DateTime.MinValue;
        private DateTime _lastSentTime = DateTime.MinValue;

        private ErrorCode? _sendErrorCode = null;
        private ErrorCode? _receiveErrorCode = null;

        private CancellationTokenSource _cancellationTokenSource = new();

        private BatchAction? _batchAction;

        public ConnectionMultiplexer(IConnection connection, BatchActionDispatcher batchActionDispatcher, IBytesPool bytesPool, ConnectionMultiplexerOptions options)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _batchActionDispatcher = batchActionDispatcher;
            _bytesPool = bytesPool ?? throw new ArgumentNullException(nameof(bytesPool));

            if (options == null) throw new ArgumentNullException(nameof(options));
            if (!EnumHelper.IsValid(options.Type)) throw new ArgumentException(nameof(options.Type));
            _options = options;

            _nextStreamId = (uint)((_options.Type == OmniConnectionMultiplexerType.Connected) ? 0 : 1);
        }

        protected override async ValueTask OnDisposeAsync()
        {
            foreach (var connection in _streamConnectionMap.Values)
            {
                await connection.DisposeAsync();
            }

            _batchActionDispatcher.Unregister(_batchAction!);

            _requestingStreamSemaphore!.Dispose();
            _sendStreamRequestPipe!.Dispose();
            _sendStreamRequestAcceptedPipe!.Dispose();
            _receiveStreamRequestPipe!.Dispose();
            _receiveStreamRequestAcceptedPipe!.Dispose();
        }

        private uint GetNextStreamId()
        {
            return Interlocked.Add(ref _nextStreamId, 2);
        }

        public async ValueTask Handshake(CancellationToken cancellationToken = default)
        {
            var myProfileMessage = new ProfileMessage((uint)_options.PacketReceiveTimeout.TotalMilliseconds, _options.MaxStreamRequestQueueSize, _options.MaxStreamDataSize, _options.MaxStreamDataQueueSize);
            var enqueueTask = _connection.Sender.SendAsync(myProfileMessage, cancellationToken).AsTask();
            var dequeueTask = _connection.Receiver.ReceiveAsync<ProfileMessage>(cancellationToken).AsTask();

            await Task.WhenAll(enqueueTask, dequeueTask);
            var otherProfileMessage = dequeueTask.Result;

            _sessionOptions = new SessionOptions(TimeSpan.FromMilliseconds(otherProfileMessage.PacketReceiveTimeoutMilliseconds), otherProfileMessage.MaxStreamRequestQueueSize, otherProfileMessage.MaxStreamDataSize, otherProfileMessage.MaxStreamDataQueueSize);

            _sendStreamRequestPipe = new BoundedMessagePipe((int)_sessionOptions.MaxStreamRequestQueueSize);
            _sendStreamRequestAcceptedPipe = new BoundedMessagePipe<uint>((int)_sessionOptions.MaxStreamRequestQueueSize);
            _receiveStreamRequestPipe = new BoundedMessagePipe((int)_options.MaxStreamRequestQueueSize);
            _receiveStreamRequestAcceptedPipe = new BoundedMessagePipe<uint>((int)_options.MaxStreamRequestQueueSize);

            _requestingStreamSemaphore = new SemaphoreSlim(0, (int)_sessionOptions.MaxStreamRequestQueueSize);

            _batchAction = new BatchAction(this);
            _batchActionDispatcher.Register(_batchAction);
        }

        private async ValueTask WaitToSendAsync(CancellationToken cancellationToken = default)
        {
            await _connection.Sender.WaitToSendAsync(cancellationToken);
        }

        private async ValueTask WaitToReceiveAsync(CancellationToken cancellationToken = default)
        {
            await _connection.Receiver.WaitToReceiveAsync(cancellationToken);
        }

        private bool TrySend()
        {
            if (_sessionOptions is null) return false;

            var result = _connection.Sender.TrySend(
                bufferWriter =>
                {
                    bool written = false;

                    if (_sendStreamRequestPipe!.Reader.TryRead())
                    {
                        var builder = new PacketBuilder(bufferWriter);
                        builder.WriteStreamRequest();
                        written = true;
                    }

                    if (written) return;

                    if (_sendStreamRequestAcceptedPipe!.Reader.TryRead(out var streamRequestAcceptedStreamId))
                    {
                        var builder = new PacketBuilder(bufferWriter);
                        builder.WriteStreamDataAccepted(streamRequestAcceptedStreamId);
                        written = true;
                    }

                    if (written) return;

                    foreach (var (streamId, connection) in _streamConnectionMap)
                    {
                        if (connection.SendDataReader.TryRead(out var data))
                        {
                            var builder = new PacketBuilder(bufferWriter);
                            builder.WriteStreamData(streamId, data);
                            _bytesPool.Array.Return(data.Array!);
                            written = true;
                        }

                        if (written) return;

                        if (connection.SendDataAcceptedReader.TryRead())
                        {
                            var builder = new PacketBuilder(bufferWriter);
                            builder.WriteStreamDataAccepted(streamId);
                            written = true;
                        }

                        if (written) return;

                        if (connection.SendFinishReader.TryRead())
                        {
                            var builder = new PacketBuilder(bufferWriter);
                            builder.WriteStreamFinish(streamId);
                            written = true;
                        }

                        if (written) return;
                    }

                    if ((DateTime.UtcNow - _lastSentTime) > (_sessionOptions.PacketReceiveTimeout / 2))
                    {
                        var builder = new PacketBuilder(bufferWriter);
                        builder.WriteKeepAlive();
                    }
                });

            if (result) _lastSentTime = DateTime.UtcNow;

            return result;
        }

        private bool TryReceive()
        {
            var result = _connection.Receiver.TryReceive(
                sequence =>
                {
                    var messageType = PacketParser.ParseMessageType(ref sequence);

                    switch (messageType)
                    {
                        case PacketType.KeepAlive:
                            break;
                        case PacketType.StreamRequest:
                            {
                                if (!_receiveStreamRequestPipe!.Writer.TryWrite())
                                {
                                    _sendErrorCode = ErrorCode.StreamRequestQueueOverflow;
                                }

                                break;
                            }

                        case PacketType.StreamRequestAccepted:
                            {
                                var streamId = PacketParser.ParseStreamId(ref sequence);
                                if (!_receiveStreamRequestAcceptedPipe!.Writer.TryWrite(() => streamId))
                                {
                                    _sendErrorCode = ErrorCode.StreamRequestQueueOverflow;
                                }

                                break;
                            }

                        case PacketType.StreamData:
                            {
                                var streamId = PacketParser.ParseStreamId(ref sequence);
                                if (!_streamConnectionMap.TryGetValue(streamId, out var connection)) return;

                                var buffer = _bytesPool.Array.Rent((int)sequence.Length);
                                sequence.CopyTo(buffer.AsSpan());
                                if (!connection.ReceiveDataWriter.TryWrite(() => new ArraySegment<byte>(buffer, 0, (int)sequence.Length)))
                                {
                                    _sendErrorCode = ErrorCode.StreamDataQueueOverflow;
                                }

                                break;
                            }

                        case PacketType.StreamDataAccepted:
                            {
                                var streamId = PacketParser.ParseStreamId(ref sequence);
                                if (!_streamConnectionMap.TryGetValue(streamId, out var connection)) return;

                                connection.ReceiveDataAcceptedPublicher.Publish();

                                break;
                            }

                        case PacketType.StreamFinish:
                            {
                                var streamId = PacketParser.ParseStreamId(ref sequence);
                                if (!_streamConnectionMap.TryGetValue(streamId, out var connection)) return;

                                connection.ReceiveFinishPublicher.Publish();

                                break;
                            }

                        case PacketType.SessionError:
                            {
                                var errorCode = PacketParser.ParseErrorCode(ref sequence);
                                _receiveErrorCode = errorCode;

                                break;
                            }

                        case PacketType.SessionFinish:
                            {
                                _cancellationTokenSource.Cancel();

                                break;
                            }
                    }
                });

            if (result) _lastReceivedTime = DateTime.UtcNow;

            return result;
        }

        public async ValueTask<IConnection> ConnectAsync(CancellationToken cancellationToken = default)
        {
            await _requestingStreamSemaphore!.WaitAsync(cancellationToken);

            try
            {
                _sendStreamRequestPipe!.Writer.TryWrite();

                var streamId = await _receiveStreamRequestAcceptedPipe!.Reader.ReadAsync(cancellationToken);
                var connection = this.CreateStreamConnection(streamId);
                return connection;
            }
            finally
            {
                _requestingStreamSemaphore.Release();
            }
        }

        public async ValueTask<IConnection> AcceptAsync(CancellationToken cancellationToken = default)
        {
            await _receiveStreamRequestPipe!.Reader.ReadAsync(cancellationToken);

            var streamId = this.GetNextStreamId();

            await _sendStreamRequestAcceptedPipe!.Writer.WriteAsync(() => streamId, cancellationToken);

            var connection = this.CreateStreamConnection(streamId);
            return connection;
        }

        private StreamConnection CreateStreamConnection(uint streamId)
        {
            var connection = new StreamConnection((int)_sessionOptions!.MaxDataQueueSize, (int)_options.MaxStreamDataQueueSize, _bytesPool);
            _streamConnectionMap.TryAdd(streamId, connection);
            return connection;
        }

        private sealed class BatchAction : IBatchAction
        {
            private readonly ConnectionMultiplexer _connectionMultiplexer;

            public BatchAction(ConnectionMultiplexer connectionMultiplexer)
            {
                _connectionMultiplexer = connectionMultiplexer;
            }

            public async ValueTask WaitAsync(CancellationToken cancellationToken = default)
            {
                var tasks = new List<Task>();
                tasks.Add(_connectionMultiplexer.WaitToSendAsync(cancellationToken).AsTask());
                tasks.Add(_connectionMultiplexer.WaitToReceiveAsync(cancellationToken).AsTask());
                await Task.WhenAny(tasks.ToArray());
            }

            public void Run()
            {
                _connectionMultiplexer.TrySend();
                _connectionMultiplexer.TryReceive();
            }
        }

        private sealed class SessionOptions
        {
            public SessionOptions(TimeSpan packetReceiveTimeout, uint maxStreamRequestQueueSize, uint maxDataSize, uint maxDataQueueSize)
            {
                this.PacketReceiveTimeout = packetReceiveTimeout;
                this.MaxStreamRequestQueueSize = maxStreamRequestQueueSize;
                this.MaxDataSize = maxDataSize;
                this.MaxDataQueueSize = maxDataQueueSize;
            }

            public TimeSpan PacketReceiveTimeout { get; }

            public uint MaxStreamRequestQueueSize { get; }

            public uint MaxDataSize { get; }

            public uint MaxDataQueueSize { get; }
        }
    }
}
