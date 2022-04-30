using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Omnius.Core.Net.I2p.Internal;
using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.I2p;

public sealed partial class SamBridge : AsyncDisposableBase
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly IPAddress _ipAddress;
    private readonly int _port;
    private readonly string _caption;
    private readonly BoundedMessagePipe<SamBridgeAcceptResult> _acceptResultPipe = new(3);
    private string _sessionId;

    private string? _base32Address;
    private Socket? _sessionSocket;
    private string? _lastPingMessage;

    private Task? _sendLoopTask;
    private Task? _receiveLoopTask;
    private Task? _acceptLoopTask;

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public static async ValueTask<SamBridge> CreateAsync(IPAddress ipAddress, int port, string caption, CancellationToken cancellationToken = default)
    {
        var result = new SamBridge(ipAddress, port, caption);
        await result.InitAsync(cancellationToken);
        return result;
    }

    private SamBridge(IPAddress ipAddress, int port, string caption)
    {
        _ipAddress = ipAddress;
        _port = port;
        _caption = caption;
        _sessionId = GenSessionId();
    }

    private static string GenSessionId()
    {
        var buffer = new byte[32];

        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(buffer);
        }

        return I2pConverter.Base64.ToString(buffer);
    }

    public async ValueTask InitAsync(CancellationToken cancellationToken = default)
    {
        Socket? socket = null;

        try
        {
            socket = await SocketConnectAsync(new IPEndPoint(_ipAddress, _port));
            if (socket is null) throw new SamBridgeException($"Failed to connect {_ipAddress}");

            using var timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            using var callbackRegister = timeoutTokenSource.Token.Register(() =>
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Dispose();
            });
            timeoutTokenSource.CancelAfter(60 * 1000);

            _base32Address = await this.HandshakeAsync(socket, timeoutTokenSource.Token);
            _sessionSocket = socket;

            _sendLoopTask = this.SendLoopAsync();
            _receiveLoopTask = this.ReceiveLoopAsync();
            _acceptLoopTask = this.AcceptLoopAsync();
        }
        catch (Exception)
        {
            if (socket != null) socket.Dispose();
            throw;
        }
    }

    private async ValueTask<string> HandshakeAsync(Socket socket, CancellationToken cancellationToken = default)
    {
        using var mediator = new SamCommandMediator(socket);
        await mediator.HandshakeAsync(cancellationToken);

        await mediator.SessionCreateAsync(_sessionId, _caption, cancellationToken);
        var destinationBase64 = await mediator.NamingLookupAsync("ME", cancellationToken);
        if (destinationBase64 is null) throw new SamBridgeException($"Failed to Naming Lookup {_ipAddress}");

        var destinationBytes = I2pConverter.Base64.FromString(destinationBase64);
        return I2pConverter.Base32Address.FromDestination(destinationBytes);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        _cancellationTokenSource.Cancel();
        await Task.WhenAll(_sendLoopTask!, _receiveLoopTask!, _acceptLoopTask!);
        _cancellationTokenSource.Dispose();

        _sessionSocket?.Dispose();
    }

    public bool IsConnected { get { return (_sessionSocket != null && _sessionSocket.Connected); } }

    public string? Base32Address => _base32Address;

    private async Task SendLoopAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var stream = new NetworkStream(_sessionSocket!);
            stream.ReadTimeout = 60 * 1000;
            stream.WriteTimeout = 60 * 1000;

            using var reader = new StreamReader(stream, new UTF8Encoding(false), false, 1024 * 32);
            using var writer = new StreamWriter(stream, new UTF8Encoding(false), 1024 * 32);
            writer.NewLine = "\n";

            for (; ; )
            {
                await Task.Delay(1000 * 30, cancellationToken);

                _lastPingMessage = I2pConverter.Base64.ToString(Random.Shared.GetBytes(32));
                writer.WriteLine(string.Format("PING {0}", _lastPingMessage));
                writer.Flush();
            }
        }
        catch (OperationCanceledException e)
        {
            _logger.Debug(e, "Operation Canceled");
        }
        catch (Exception e)
        {
            _logger.Error(e, "Unexpected Exception");
        }
    }

    private async Task ReceiveLoopAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var stream = new NetworkStream(_sessionSocket!);
            stream.ReadTimeout = 60 * 1000;
            stream.WriteTimeout = 60 * 1000;

            using var reader = new StreamReader(stream, new UTF8Encoding(false), false, 1024 * 32);
            using var writer = new StreamWriter(stream, new UTF8Encoding(false), 1024 * 32);
            writer.NewLine = "\n";

            for (; ; )
            {
                await Task.Delay(1000, cancellationToken);

                var line = await reader.ReadLineAsync();
                if (line == null) break;

                if (line.StartsWith("PING"))
                {
                    writer.WriteLine(string.Format("PONG {0}", line.Substring(5)));
                    writer.Flush();
                }
                else if (line.StartsWith("PONG"))
                {
                    if (_lastPingMessage != line.Substring(5)) break;
                    _lastPingMessage = null;
                }
            }
        }
        catch (OperationCanceledException e)
        {
            _logger.Debug(e, "Operation Canceled");
        }
        catch (Exception e)
        {
            _logger.Error(e, "Unexpected Exception");
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
                    if (socket is null) throw new SamBridgeException($"Failed to connect {_ipAddress}");

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
            _logger.Debug(e, "Operation Canceled");
        }
        catch (Exception e)
        {
            _logger.Error(e, "Unexpected Exception");
        }
    }

    private async ValueTask<string> InternalAcceptAsync(Socket socket, CancellationToken cancellationToken = default)
    {
        using var mediator = new SamCommandMediator(socket!);
        await mediator.HandshakeAsync(cancellationToken);

        var destinationBase64 = await mediator.StreamAcceptAsync(_sessionId, cancellationToken);
        if (destinationBase64 is null) throw new SamBridgeException($"Failed to Stream Accept {_ipAddress}");

        var destinationBytes = I2pConverter.Base64.FromString(destinationBase64);
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
            socket.SendTimeout = 1000 * 10;
            socket.ReceiveTimeout = 1000 * 10;
            await socket.ConnectAsync(remoteEndPoint, cancellationToken);

            return socket;
        }
        catch (SocketException)
        {
            if (socket != null) socket.Dispose();

            throw;
        }
    }

    public async ValueTask<Socket> ConnectAsync(string destination, CancellationToken cancellationToken = default)
    {
        Socket? socket = null;

        try
        {
            socket = await SocketConnectAsync(new IPEndPoint(_ipAddress!, _port));
            if (socket is null) throw new SamBridgeException($"Failed to connect {_ipAddress}");

            await this.InternalConnectAsync(socket, destination, cancellationToken);
            return socket;
        }
        catch (Exception)
        {
            if (socket != null) socket.Dispose();
            throw;
        }
    }

    private async ValueTask<string> InternalConnectAsync(Socket socket, string destination, CancellationToken cancellationToken = default)
    {
        using var mediator = new SamCommandMediator(socket);
        await mediator.HandshakeAsync(cancellationToken);

        var destinationBase64 = await mediator.NamingLookupAsync(destination, cancellationToken);
        if (destinationBase64 is null) throw new SamBridgeException($"Failed to Stream Accept {_ipAddress}");
        await mediator.StreamConnectAsync(_sessionId, destinationBase64, cancellationToken);

        var destinationBytes = I2pConverter.Base64.FromString(destinationBase64);
        return I2pConverter.Base32Address.FromDestination(destinationBytes);
    }

    public async ValueTask<SamBridgeAcceptResult> AcceptAsync(CancellationToken cancellationToken = default)
    {
        return await _acceptResultPipe.Reader.ReadAsync(cancellationToken);
    }
}
