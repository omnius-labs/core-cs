using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Helpers;
using Omnius.Core.Net.Connections.Secure.Internal;

namespace Omnius.Core.Net.Connections.Secure
{
    public sealed class OmniSecureConnection : AsyncDisposableBase, IConnection
    {
        private readonly IConnection _connection;
        private readonly OmniSecureConnectionOptions _options;
        private readonly IBytesPool _bytesPool;

        private OmniSecureConnectionVersion _version = OmniSecureConnectionVersion.Version1;
        private V1.Internal.SecureConnection? _secureConnection_v1;

        public OmniSecureConnection(IConnection connection, OmniSecureConnectionOptions options)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (!EnumHelper.IsValid(options.Type)) throw new ArgumentException(nameof(options.Type));

            _connection = connection;
            _options = options;
            _bytesPool = options.BytesPool;
        }

        protected override async ValueTask OnDisposeAsync()
        {
            if (_secureConnection_v1 is not null) await _secureConnection_v1.DisposeAsync();
        }

        public IConnection BaseConnection => _connection;

        public bool IsConnected => _connection.IsConnected;

        public OmniSecureConnectionType Type => _options.Type;

        public IEnumerable<string> MatchedPasswords
        {
            get
            {
                if (_secureConnection_v1 != null)
                {
                    return _secureConnection_v1.MatchedPasswords;
                }

                return Enumerable.Empty<string>();
            }
        }

        public IConnectionSender Sender
        {
            get
            {
                if (_secureConnection_v1 != null)
                {
                    return _secureConnection_v1.Sender;
                }

                throw new InvalidOperationException();
            }
        }

        public IConnectionReceiver Receiver
        {
            get
            {
                if (_secureConnection_v1 != null)
                {
                    return _secureConnection_v1.Receiver;
                }

                throw new InvalidOperationException();
            }
        }

        public IConnectionSubscribers Subscribers
        {
            get
            {
                if (_secureConnection_v1 != null)
                {
                    return _secureConnection_v1.Subscribers;
                }

                throw new InvalidOperationException();
            }
        }

        public async ValueTask HandshakeAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await this.HelloAsync(cancellationToken);

                if (_version == OmniSecureConnectionVersion.Version1)
                {
                    _secureConnection_v1 = new V1.Internal.SecureConnection(_connection, _options);
                    await _secureConnection_v1.Handshake(cancellationToken);
                }
                else
                {
                    throw new NotSupportedException("Not supported OmniSecureConnectionVersion.");
                }
            }
            catch (OmniSecureConnectionException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new OmniSecureConnectionException($"Handshake of {nameof(OmniSecureConnection)} failed.", e);
            }
        }

        private async ValueTask HelloAsync(CancellationToken cancellationToken)
        {
            HelloMessage sendHelloMessage;
            HelloMessage? receiveHelloMessage = null;
            {
                sendHelloMessage = new HelloMessage(new[] { _version });

                var enqueueTask = _connection.Sender.SendAsync(sendHelloMessage, cancellationToken).AsTask();
                var dequeueTask = _connection.Receiver.ReceiveAsync<HelloMessage>(cancellationToken).AsTask();

                await Task.WhenAll(enqueueTask, dequeueTask);
                receiveHelloMessage = dequeueTask.Result;

                if (receiveHelloMessage is null) throw new NullReferenceException();
            }

            _version = EnumHelper.GetOverlappedMaxValue(sendHelloMessage.Versions, receiveHelloMessage.Versions) ?? OmniSecureConnectionVersion.Unknown;
        }
    }
}
