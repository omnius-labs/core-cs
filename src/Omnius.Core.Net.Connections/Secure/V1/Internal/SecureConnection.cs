using System;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Cryptography;
using Omnius.Core.Net.Connections.Internal;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Secure.V1.Internal
{
    public sealed partial class SecureConnection : AsyncDisposableBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IConnection _connection;
        private readonly IBatchActionDispatcher _batchActionDispatcher;
        private readonly IBytesPool _bytesPool;
        private readonly OmniSecureConnectionOptions _options;

        private OmniSignature? _signature;

        private ConnectionSender? _sender;
        private ConnectionReceiver? _receiver;
        private ConnectionEvents? _subscribers;
        private BatchAction? _batchAction;

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private const int FrameSize = 32 * 1024;
        private const int TagSize = 16;

        public SecureConnection(IConnection connection, IBatchActionDispatcher batchActionDispatcher, IBytesPool bytesPool, OmniSecureConnectionOptions options)
        {
            _connection = connection;
            _batchActionDispatcher = batchActionDispatcher;
            _bytesPool = bytesPool;
            _options = options;
        }

        protected override async ValueTask OnDisposeAsync()
        {
            if (_batchAction is not null) _batchActionDispatcher.Unregister(_batchAction);

            _cancellationTokenSource.Cancel();
            _subscribers?.Dispose();
            _sender?.Dispose();
            _receiver?.Dispose();
            await _connection.DisposeAsync();
            _cancellationTokenSource.Dispose();
        }

        public bool IsConnected => _connection.IsConnected;

        public IConnectionSender Sender => _sender ?? throw new InvalidOperationException();

        public IConnectionReceiver Receiver => _receiver ?? throw new InvalidOperationException();

        public IConnectionEvents Subscribers => _subscribers ?? throw new InvalidOperationException();

        public OmniSignature? Signature => _signature;

        public async ValueTask HandshakeAsync(CancellationToken cancellationToken = default)
        {
            var authenticator = new Authenticator(_connection, _options.Type, _options.DigitalSignature, _bytesPool);
            var authenticatedResult = await authenticator.AuthenticateAsync(cancellationToken);

            _signature = authenticatedResult.Signature;
            _sender = new ConnectionSender(_connection.Sender, authenticatedResult.CryptoAlgorithmType, authenticatedResult.EncryptKey, authenticatedResult.EncryptNonce, _bytesPool, _cancellationTokenSource);
            _receiver = new ConnectionReceiver(_connection.Receiver, authenticatedResult.CryptoAlgorithmType, _options.MaxReceiveByteCount, authenticatedResult.DecryptKey, authenticatedResult.DecryptNonce, _bytesPool, _cancellationTokenSource);
            _subscribers = new ConnectionEvents(_cancellationTokenSource.Token);
            _batchAction = new BatchAction(_sender, _receiver);
            _batchActionDispatcher.Register(_batchAction);
        }

        internal static void Increment(in byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0xff)
                {
                    bytes[i] = 0x00;
                }
                else
                {
                    bytes[i]++;
                    break;
                }
            }
        }
    }
}
