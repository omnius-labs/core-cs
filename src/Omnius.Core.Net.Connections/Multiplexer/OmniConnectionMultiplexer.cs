using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Helpers;

namespace Omnius.Core.Net.Connections.Multiplexer
{
    public sealed class OmniConnectionMultiplexer : AsyncDisposableBase
    {
        private readonly IConnection _connection;
        private readonly OmniConnectionMultiplexerV1Options _options;
        private readonly IBytesPool _bytesPool;

        private OmniConnectionMultiplexerVersion _version = OmniConnectionMultiplexerVersion.Version1;
        private V1.Internal.ConnectionMultiplexer? _connectionMultiplexer_v1;

        public OmniConnectionMultiplexer(IConnection connection, OmniConnectionMultiplexerV1Options options)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (options == null) throw new ArgumentNullException(nameof(options));

            _connection = connection;
            _options = options;
            _bytesPool = options.BufferPool ?? BytesPool.Shared;
        }

        protected override async ValueTask OnDisposeAsync()
        {
            if (_connectionMultiplexer_v1 is not null) await _connectionMultiplexer_v1.DisposeAsync();
        }

        public IConnection BaseConnection => _connection;

        public bool IsConnected => _connection.IsConnected;

        public long TotalBytesSent => _connection.TotalBytesSent;

        public long TotalBytesReceived => _connection.TotalBytesReceived;

        public async ValueTask HandshakeAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await this.HelloAsync(cancellationToken);

                if (_version == OmniConnectionMultiplexerVersion.Version1)
                {
                    _connectionMultiplexer_v1 = new V1.Internal.ConnectionMultiplexer(_connection, _options);
                    await _connectionMultiplexer_v1.Handshake(cancellationToken);
                }
                else
                {
                    throw new NotSupportedException("Not supported OmniConnectionMultiplexerVersion.");
                }
            }
            catch (OmniConnectionMultiplexerException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new OmniConnectionMultiplexerException($"Handshake of {nameof(OmniConnectionMultiplexer)} failed.", e);
            }
        }

        private async ValueTask HelloAsync(CancellationToken cancellationToken)
        {
            HelloMessage sendHelloMessage;
            HelloMessage? receiveHelloMessage = null;
            {
                sendHelloMessage = new HelloMessage(new[] { _version });

                var enqueueTask = _connection.EnqueueAsync((bufferWriter) => sendHelloMessage.Export(bufferWriter, _bytesPool), cancellationToken);
                var dequeueTask = _connection.DequeueAsync((sequence) => receiveHelloMessage = HelloMessage.Import(sequence, _bytesPool), cancellationToken);

                await ValueTaskHelper.WhenAll(enqueueTask, dequeueTask);

                if (receiveHelloMessage is null) throw new NullReferenceException();
            }

            _version = GetOverlapMaxEnum(sendHelloMessage.Versions, receiveHelloMessage.Versions);
        }

        public bool TryEnqueue(Action<IBufferWriter<byte>> action)
        {
            if (_connectionMultiplexer_v1 != null)
            {
                return _connectionMultiplexer_v1.TryEnqueue(action);
            }

            throw new NotSupportedException("Not supported OmniConnectionMultiplexerVersion.");
        }

        public async ValueTask EnqueueAsync(Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default)
        {
            if (_connectionMultiplexer_v1 != null)
            {
                await _connectionMultiplexer_v1.EnqueueAsync(action, cancellationToken);
                return;
            }

            throw new NotSupportedException("Not supported OmniConnectionMultiplexerVersion.");
        }

        public bool TryDequeue(Action<ReadOnlySequence<byte>> action)
        {
            if (_connectionMultiplexer_v1 != null)
            {
                return _connectionMultiplexer_v1.TryDequeue(action);
            }

            throw new NotSupportedException("Not supported OmniConnectionMultiplexerVersion.");
        }

        public async ValueTask DequeueAsync(Action<ReadOnlySequence<byte>> action, CancellationToken cancellationToken = default)
        {
            if (_connectionMultiplexer_v1 != null)
            {
                await _connectionMultiplexer_v1.DequeueAsync(action, cancellationToken);
                return;
            }

            throw new NotSupportedException("Not supported OmniConnectionMultiplexerVersion.");
        }
    }
}
