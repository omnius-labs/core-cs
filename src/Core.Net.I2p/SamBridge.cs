using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Omnius.Core.Base;
using Omnius.Core.Base.Helpers;
using Omnius.Core.Net.I2p.Internal;
using Omnius.Core.Base.Pipelines;

namespace Omnius.Core.Net.I2p;

public sealed partial class SamBridge : AsyncDisposableBase
{
    private readonly IPAddress _ipAddress;
    private readonly int _port;
    private readonly string _caption;
    private readonly ILogger _logger;
    private readonly BoundedMessagePipe<SamBridgeAcceptResult> _acceptResultPipe = new(3);
    private string _sessionId;

    private string? _base32Address;
    private Socket? _sessionSocket;
    private string? _lastPingMessage;

    private Task? _sendLoopTask;
    private Task? _receiveLoopTask;
    private Task? _acceptLoopTask;

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public static async ValueTask<SamBridge> CreateAsync(IPAddress ipAddress, int port, string caption, ILogger<SamBridge> logger, CancellationToken cancellationToken = default)
    {
        var result = new SamBridge(ipAddress, port, caption, logger);
        await result.InitAsync(cancellationToken);
        return result;
    }

    private SamBridge(IPAddress ipAddress, int port, string caption, ILogger<SamBridge> logger)
    {
        _ipAddress = ipAddress;
        _port = port;
        _caption = caption;
        _logger = logger;
        _sessionId = GenSessionId();
    }

    private static string GenSessionId()
    {
        return Guid.NewGuid().ToString("N");
    }

    private async ValueTask InitAsync(CancellationToken cancellationToken = default)
    {
        Socket? socket = null;

        try
        {
            socket = await SocketConnectAsync(new IPEndPoint(_ipAddress, _port));
            if (socket is null) throw new SamBridgeException($"Failed to connect {_ipAddress}");

            using var timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            using var callbackRegister = timeoutTokenSource.Token.Register(() => ExceptionHelper.ExecuteAndLogIfFailed<ObjectDisposedException>(() => socket.Dispose(), _logger));
            timeoutTokenSource.CancelAfter(60 * 1000);

            _base32Address = await this.HandshakeAsync(socket, timeoutTokenSource.Token);
            _sessionSocket = socket;

            _sendLoopTask = this.SendLoopAsync(_cancellationTokenSource.Token);
            _receiveLoopTask = this.ReceiveLoopAsync(_cancellationTokenSource.Token);
            _acceptLoopTask = this.AcceptLoopAsync(_cancellationTokenSource.Token);
        }
        catch (Exception)
        {
            if (socket != null) socket.Dispose();
            throw;
        }
    }

    private async ValueTask<string> HandshakeAsync(Socket socket, CancellationToken cancellationToken = default)
    {
        using var connection = new SamConnection(socket);

        await connection.HandshakeAsync(cancellationToken);

        (string destination, string privateKey) = await connection.DestinationGenerateAsync(cancellationToken);

        await connection.SessionCreateAsync(_sessionId, privateKey, _caption, cancellationToken);

        var destinationBytes = I2pConverter.Base64.FromString(destination);
        return I2pConverter.Base32Address.FromDestination(destinationBytes);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        _cancellationTokenSource.Cancel();
        await Task.WhenAll(_sendLoopTask!, _receiveLoopTask!, _acceptLoopTask!);
        _cancellationTokenSource.Dispose();

        _sessionSocket?.Dispose();
        _acceptResultPipe.Dispose();
    }

    public bool IsConnected { get { return (_sessionSocket != null && _sessionSocket.Connected); } }

    public string? Base32Address => _base32Address;

    private async Task SendLoopAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = new SamConnection(_sessionSocket!);

            for (; ; )
            {
                await Task.Delay(1000 * 30, cancellationToken);

                _lastPingMessage = Guid.NewGuid().ToString("N");
                await connection.SendCommandAsync(new SamCommand(new[] { "PING", _lastPingMessage }), cancellationToken);
            }
        }
        catch (OperationCanceledException e)
        {
            _logger.LogDebug(e, "Operation Canceled");
        }
        catch (Exception e)
        {
            _logger.LogDebug(e, "Unexpected Exception");
        }
    }

    private async Task ReceiveLoopAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = new SamConnection(_sessionSocket!);

            for (; ; )
            {
                await Task.Delay(1000, cancellationToken);

                var samCommand = await connection.ReceiveCommandAsync(cancellationToken);

                if (samCommand.Commands[0] == "PING")
                {
                    await connection.SendCommandAsync(new SamCommand(new[] { "PONG", samCommand.Commands[1] }), cancellationToken);
                }
                else if (samCommand.Commands[0] == "PONG")
                {
                    if (samCommand.Commands[1] != _lastPingMessage)
                    {
                        _logger.LogDebug("Invalid PONG");
                    }

                    _lastPingMessage = null;
                }
            }
        }
        catch (OperationCanceledException e)
        {
            _logger.LogDebug(e, "Operation Canceled");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected Exception");
        }
    }

    private async Task AcceptLoopAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            for (; ; )
            {
                await _acceptResultPipe.Writer.WaitToWriteAsync(cancellationToken);

                Socket? socket = null;

                try
                {
                    socket = await SocketConnectAsync(new IPEndPoint(_ipAddress!, _port));

                    string destination = await this.InternalAcceptAsync(socket, cancellationToken);
                    _acceptResultPipe.Writer.TryWrite(new SamBridgeAcceptResult(socket, destination));
                }
                catch (Exception)
                {
                    if (socket != null) socket.Dispose();
                }
            }
        }
        catch (OperationCanceledException e)
        {
            _logger.LogDebug(e, "Operation Canceled");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected Exception");
        }
    }

    private async ValueTask<string> InternalAcceptAsync(Socket socket, CancellationToken cancellationToken = default)
    {
        using var connection = new SamConnection(socket!);
        await connection.HandshakeAsync(cancellationToken);

        var destination = await connection.StreamAcceptAsync(_sessionId, cancellationToken);
        if (destination is null) throw new SamBridgeException($"Failed to Stream Accept {_ipAddress}");

        var destinationBytes = I2pConverter.Base64.FromString(destination);
        return I2pConverter.Base32Address.FromDestination(destinationBytes);
    }

    private static async ValueTask<IPAddress?> GetIpAddressAsync(string host, CancellationToken cancellationToken = default)
    {
        if (IPAddress.TryParse(host, out var ipAddress)) return ipAddress;

        var hostEntry = await Dns.GetHostEntryAsync(host, cancellationToken);
        return hostEntry.AddressList.FirstOrDefault();
    }

    private static async ValueTask<Socket> SocketConnectAsync(IPEndPoint remoteEndPoint, CancellationToken cancellationToken = default)
    {
        Socket? socket = null;

        try
        {
            socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.SendTimeout = 60 * 1000;
            socket.ReceiveTimeout = 60 * 1000;
            await socket.ConnectAsync(remoteEndPoint, cancellationToken);

            return socket;
        }
        catch (SocketException)
        {
            socket?.Dispose();

            throw;
        }
    }

    public async ValueTask<Socket> ConnectAsync(string base32Address, CancellationToken cancellationToken = default)
    {
        Socket? socket = null;

        try
        {
            socket = await SocketConnectAsync(new IPEndPoint(_ipAddress!, _port));
            if (socket is null) throw new SamBridgeException($"Failed to connect {_ipAddress}");

            await this.InternalConnectAsync(socket, base32Address, cancellationToken);
            return socket;
        }
        catch (Exception)
        {
            if (socket != null) socket.Dispose();
            throw;
        }
    }

    private async ValueTask<string> InternalConnectAsync(Socket socket, string base32Address, CancellationToken cancellationToken = default)
    {
        using var connection = new SamConnection(socket);

        await connection.HandshakeAsync(cancellationToken);

        var destination = await connection.NamingLookupAsync(base32Address, cancellationToken);

        await connection.StreamConnectAsync(_sessionId, destination, cancellationToken);

        var destinationBytes = I2pConverter.Base64.FromString(destination);
        return I2pConverter.Base32Address.FromDestination(destinationBytes);
    }

    public async ValueTask<SamBridgeAcceptResult?> AcceptAsync(CancellationToken cancellationToken = default)
    {
        if (!_acceptResultPipe.Reader.TryRead(out var result)) return null;
        return result;
    }
}
