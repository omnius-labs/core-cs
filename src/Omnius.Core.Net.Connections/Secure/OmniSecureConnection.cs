using Omnius.Core.Cryptography;
using Omnius.Core.Helpers;
using Omnius.Core.Net.Connections.Secure.Internal;

namespace Omnius.Core.Net.Connections.Secure;

public sealed class OmniSecureConnection : AsyncDisposableBase, IConnection
{
    private readonly IConnection _connection;
    private readonly V1.Internal.SecureConnection? _secureConnection_v1 = null;

    private OmniSecureConnectionVersion _version = OmniSecureConnectionVersion.Unknown;

    public static OmniSecureConnection CreateV1(IConnection connection, IBytesPool bytesPool, V1.OmniSecureConnectionOptions options)
    {
        var secureConnection = new V1.Internal.SecureConnection(connection, bytesPool, options);
        return new OmniSecureConnection(connection, secureConnection);
    }

    private OmniSecureConnection(IConnection connection, V1.Internal.SecureConnection? secureConnection)
    {
        _connection = connection;
        _secureConnection_v1 = secureConnection;
    }

    protected override async ValueTask OnDisposeAsync()
    {
        if (_secureConnection_v1 is not null) await _secureConnection_v1.DisposeAsync();
    }

    public bool IsConnected
    {
        get
        {
            if (_secureConnection_v1 != null)
            {
                return _secureConnection_v1.IsConnected;
            }

            return false;
        }
    }

    public OmniSignature? Signature
    {
        get
        {
            if (_secureConnection_v1 != null)
            {
                return _secureConnection_v1.Signature;
            }

            return null;
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

    public IConnectionEvents Events
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

            if (_version == OmniSecureConnectionVersion.Version1 && _secureConnection_v1 is not null)
            {
                await _secureConnection_v1.HandshakeAsync(cancellationToken);
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
        var versions = new List<OmniSecureConnectionVersion>();
        if (_secureConnection_v1 is not null) versions.Add(OmniSecureConnectionVersion.Version1);

        var sendHelloMessage = new HelloMessage(versions.ToArray());
        var receiveHelloMessage = await _connection.ExchangeAsync(sendHelloMessage, cancellationToken);

        _version = EnumHelper.GetOverlappedMaxValue(sendHelloMessage.Versions, receiveHelloMessage.Versions) ?? OmniSecureConnectionVersion.Unknown;
    }
}