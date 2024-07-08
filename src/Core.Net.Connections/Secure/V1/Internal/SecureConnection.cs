using Omnius.Core.Base;
using Omnius.Core.Cryptography;
using Omnius.Core.Net.Connections.Internal;

namespace Omnius.Core.Net.Connections.Secure.V1.Internal;

public sealed partial class SecureConnection : AsyncDisposableBase
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly IConnection _connection;
    private readonly IBytesPool _bytesPool;
    private readonly OmniSecureConnectionOptions _options;

    private OmniSignature? _signature;

    private ConnectionSender? _sender;
    private ConnectionReceiver? _receiver;
    private ConnectionEvents? _events;

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private readonly object _lockObject = new();
    private bool _canceled = false;

    private const int MaxPayloadLength = 1024 * 1024 * 8;
    private const int TagLength = 16;

    public SecureConnection(IConnection connection, IBytesPool bytesPool, OmniSecureConnectionOptions options)
    {
        _connection = connection;
        _bytesPool = bytesPool;
        _options = options;
    }

    protected override async ValueTask OnDisposeAsync()
    {
        this.Cancel();

        _events?.Dispose();
        _sender?.Dispose();
        _receiver?.Dispose();
        await _connection.DisposeAsync();
        _cancellationTokenSource.Dispose();
    }

    public bool IsConnected => _connection.IsConnected;

    public IConnectionSender Sender => _sender ?? throw new InvalidOperationException();

    public IConnectionReceiver Receiver => _receiver ?? throw new InvalidOperationException();

    public IConnectionEvents Subscribers => _events ?? throw new InvalidOperationException();

    public OmniSignature? Signature => _signature;

    public async ValueTask HandshakeAsync(CancellationToken cancellationToken = default)
    {
        var authenticator = new Authenticator(_connection, _options.Type, _options.DigitalSignature, _bytesPool);
        var authenticatedResult = await authenticator.AuthenticateAsync(cancellationToken);

        _signature = authenticatedResult.Signature;
        _sender = new ConnectionSender(_connection.Sender, authenticatedResult.CryptoAlgorithmType, authenticatedResult.EncryptKey, authenticatedResult.EncryptNonce, _bytesPool);
        _receiver = new ConnectionReceiver(_connection.Receiver, authenticatedResult.CryptoAlgorithmType, _options.MaxReceiveByteCount, authenticatedResult.DecryptKey, authenticatedResult.DecryptNonce, _bytesPool);
        _events = new ConnectionEvents(_cancellationTokenSource.Token);
    }

    private void Cancel()
    {
        lock (_lockObject)
        {
            if (_canceled) return;
            _canceled = true;

            _cancellationTokenSource.Cancel();
        }
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
