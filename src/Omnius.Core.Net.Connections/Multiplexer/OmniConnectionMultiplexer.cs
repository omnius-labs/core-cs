using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Helpers;
using Omnius.Core.Net.Connections.Multiplexer.Internal;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Multiplexer;

public sealed class OmniConnectionMultiplexer : AsyncDisposableBase, IConnectionMultiplexer
{
    private readonly IConnection _connection;
    private readonly V1.Internal.ConnectionMultiplexer? _multiplexer_v1 = null;

    private OmniConnectionMultiplexerVersion? _version;

    public static OmniConnectionMultiplexer CreateV1(IConnection connection, IBatchActionDispatcher batchActionDispatcher, IBytesPool bytesPool, V1.OmniConnectionMultiplexerOptions options)
    {
        var multiplexer = new V1.Internal.ConnectionMultiplexer(connection, batchActionDispatcher, bytesPool, options);
        return new OmniConnectionMultiplexer(connection, multiplexer);
    }

    private OmniConnectionMultiplexer(IConnection connection, V1.Internal.ConnectionMultiplexer? multiplexer)
    {
        _connection = connection;
        _multiplexer_v1 = multiplexer;
    }

    protected override async ValueTask OnDisposeAsync()
    {
        if (_multiplexer_v1 is not null) await _multiplexer_v1.DisposeAsync();
    }

    public bool IsConnected
    {
        get
        {
            if (_multiplexer_v1 != null)
            {
                return _multiplexer_v1.IsConnected;
            }

            return false;
        }
    }

    public async ValueTask HandshakeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await this.HelloAsync(cancellationToken);

            if (_version == OmniConnectionMultiplexerVersion.Version1 && _multiplexer_v1 is not null)
            {
                await _multiplexer_v1.HandshakeAsync(cancellationToken);
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
        var versions = new List<OmniConnectionMultiplexerVersion>();
        if (_multiplexer_v1 is not null) versions.Add(OmniConnectionMultiplexerVersion.Version1);

        var sendHelloMessage = new HelloMessage(versions.ToArray());
        var receiveHelloMessage = await _connection.ExchangeAsync(sendHelloMessage, cancellationToken);

        _version = EnumHelper.GetOverlappedMaxValue(sendHelloMessage.Versions, receiveHelloMessage.Versions) ?? OmniConnectionMultiplexerVersion.Unknown;
    }

    public async ValueTask<IConnection> ConnectAsync(CancellationToken cancellationToken = default)
    {
        if (_multiplexer_v1 is not null)
        {
            return await _multiplexer_v1.ConnectAsync(cancellationToken);
        }

        throw new InvalidOperationException();
    }

    public async ValueTask<IConnection> AcceptAsync(CancellationToken cancellationToken = default)
    {
        if (_multiplexer_v1 is not null)
        {
            return await _multiplexer_v1.AcceptAsync(cancellationToken);
        }

        throw new InvalidOperationException();
    }
}