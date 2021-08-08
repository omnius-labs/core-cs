using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Helpers;
using Omnius.Core.Net.Connections.Multiplexer.Internal;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Multiplexer
{
    public sealed class OmniConnectionMultiplexer : AsyncDisposableBase, IMultiplexer
    {
        private readonly IConnection _connection;
        private readonly BatchActionDispatcher _batchActionDispatcher;
        private readonly IBytesPool _bytesPool;
        private readonly V1.ConnectionMultiplexerOptions _options_v1;

        private OmniConnectionMultiplexerVersion? _version;
        private V1.Internal.ConnectionMultiplexer? _connectionMultiplexer_v1;

        public OmniConnectionMultiplexer(IConnection connection, BatchActionDispatcher batchActionDispatcher, IBytesPool bytesPool, V1.ConnectionMultiplexerOptions options)
        {
            _connection = connection;
            _batchActionDispatcher = batchActionDispatcher;
            _bytesPool = bytesPool;
            _options_v1 = options;
        }

        protected override async ValueTask OnDisposeAsync()
        {
            if (_connectionMultiplexer_v1 is not null) await _connectionMultiplexer_v1.DisposeAsync();
        }

        public IConnection BridgeConnection => _connection;

        public bool IsConnected => _connection.IsConnected;

        public async ValueTask HandshakeAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await this.HelloAsync(cancellationToken);

                if (_version == OmniConnectionMultiplexerVersion.Version1)
                {
                    _connectionMultiplexer_v1 = new V1.Internal.ConnectionMultiplexer(_connection, _batchActionDispatcher, _bytesPool, _options_v1);
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
            HelloMessage? receiveHelloMessage;
            {
                sendHelloMessage = new HelloMessage(new[] { OmniConnectionMultiplexerVersion.Version1 });

                var enqueueTask = _connection.Sender.SendAsync(sendHelloMessage, cancellationToken).AsTask();
                var dequeueTask = _connection.Receiver.ReceiveAsync<HelloMessage>(cancellationToken).AsTask();

                await Task.WhenAll(enqueueTask, dequeueTask);
                receiveHelloMessage = dequeueTask.Result;

                if (receiveHelloMessage is null) throw new NullReferenceException();
            }

            _version = EnumHelper.GetOverlappedMaxValue(sendHelloMessage.Versions, receiveHelloMessage.Versions);
        }

        public async ValueTask<IConnection> ConnectAsync(CancellationToken cancellationToken = default)
        {
            if (_connectionMultiplexer_v1 is not null)
            {
                return await _connectionMultiplexer_v1.ConnectAsync(cancellationToken);
            }

            throw new InvalidOperationException();
        }

        public async ValueTask<IConnection> AcceptAsync(CancellationToken cancellationToken = default)
        {
            if (_connectionMultiplexer_v1 is not null)
            {
                return await _connectionMultiplexer_v1.AcceptAsync(cancellationToken);
            }

            throw new InvalidOperationException();
        }
    }
}
