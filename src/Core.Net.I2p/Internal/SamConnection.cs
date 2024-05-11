using System.Net.Sockets;
using System.Text;
using Core.Base;

namespace Core.Net.I2p.Internal;

internal sealed class SamConnection : DisposableBase
{
    private SocketLineReader _reader;
    private Stream _stream;
    private StreamWriter _writer;

    public SamConnection(Socket socket)
    {
        _reader = new SocketLineReader(socket, new UTF8Encoding(false));

        _stream = new NetworkStream(socket);
        _stream.WriteTimeout = Timeout.Infinite;

        _writer = new StreamWriter(_stream, new UTF8Encoding(false), 1024 * 32);
        _writer.NewLine = "\n";
    }
    protected override void OnDispose(bool disposing)
    {
        _stream.Dispose();
        _writer.Dispose();
    }

    public async ValueTask HandshakeAsync(CancellationToken cancellationToken = default)
    {
        {
            var commands = new List<string>();
            commands.Add("HELLO");
            commands.Add("VERSION");

            var parameters = new Dictionary<string, string>();
            parameters.Add("MIN", "3.2");
            parameters.Add("MAX", "3.2");

            var samCommand = new SamCommand(commands, parameters);
            await this.SendCommandAsync(samCommand, cancellationToken);
        }

        {
            var (commands, parameters) = await this.ReceiveCommandAsync(cancellationToken);

            if (commands[0] != "HELLO" || commands[1] != "REPLY" || parameters["RESULT"] != "OK")
            {
                throw new SamBridgeException($"Handshake failed because of {parameters["RESULT"]}");
            }
        }
    }

    public async ValueTask SessionCreateAsync(string sessionId, string privateKey, string caption, CancellationToken cancellationToken = default)
    {
        {
            var commands = new List<string>();
            commands.Add("SESSION");
            commands.Add("CREATE");

            var parameters = new Dictionary<string, string>();
            parameters.Add("STYLE", "STREAM");
            parameters.Add("ID", sessionId);
            parameters.Add("DESTINATION", privateKey);
            parameters.Add("inbound.nickname", caption);
            parameters.Add("outbound.nickname", caption);

            var samCommand = new SamCommand(commands, parameters);
            await this.SendCommandAsync(samCommand, cancellationToken);
        }

        for (; ; )
        {
            var (commands, parameters) = await this.ReceiveCommandAsync(cancellationToken);

            if (commands[0] == "SESSION" && commands[1] == "STATUS" && parameters["RESULT"] == "OK")
            {
                return;
            }
            else if (commands[0] == "PING")
            {
                await this.SendCommandAsync(new SamCommand(new[] { "PONG", commands[1] }), cancellationToken);
            }
            else
            {
                throw new SamBridgeException($"Session Create failed because of {parameters["RESULT"]}");
            }
        }
    }

    public async ValueTask<(string destination, string privateKey)> DestinationGenerateAsync(CancellationToken cancellationToken = default)
    {
        {
            var commands = new List<string>();
            commands.Add("DEST");
            commands.Add("GENERATE");

            var parameters = new Dictionary<string, string>();
            parameters.Add("SIGNATURE_TYPE", "EdDSA_SHA512_Ed25519");

            var samCommand = new SamCommand(commands, parameters);
            await this.SendCommandAsync(samCommand, cancellationToken);
        }

        for (; ; )
        {
            var (commands, parameters) = await this.ReceiveCommandAsync(cancellationToken);

            if (commands[0] == "DEST" && commands[1] == "REPLY")
            {
                return (parameters["PUB"], parameters["PRIV"]);
            }
            else if (commands[0] == "PING")
            {
                await this.SendCommandAsync(new SamCommand(new[] { "PONG", commands[1] }), cancellationToken);
            }
            else
            {
                throw new SamBridgeException($"Naming Lookup failed because of {parameters["RESULT"]}");
            }
        }
    }

    public async ValueTask<string> NamingLookupAsync(string name, CancellationToken cancellationToken = default)
    {
        {
            var commands = new List<string>();
            commands.Add("NAMING");
            commands.Add("LOOKUP");

            var parameters = new Dictionary<string, string>();
            parameters.Add("NAME", name);

            var samCommand = new SamCommand(commands, parameters);
            await this.SendCommandAsync(samCommand, cancellationToken);
        }

        for (; ; )
        {
            var (commands, parameters) = await this.ReceiveCommandAsync(cancellationToken);

            if (commands[0] == "NAMING" && commands[1] == "REPLY" && parameters["RESULT"] == "OK")
            {
                return parameters["VALUE"];
            }
            else if (commands[0] == "PING")
            {
                await this.SendCommandAsync(new SamCommand(new[] { "PONG", commands[1] }), cancellationToken);
            }
            else
            {
                throw new SamBridgeException($"Naming Lookup failed because of {parameters["RESULT"]}");
            }
        }
    }

    public async ValueTask StreamConnectAsync(string sessionId, string destination, CancellationToken cancellationToken = default)
    {
        {
            var commands = new List<string>();
            commands.Add("STREAM");
            commands.Add("CONNECT");

            var parameters = new Dictionary<string, string>();
            parameters.Add("ID", sessionId);
            parameters.Add("DESTINATION", destination);
            parameters.Add("SILENCE", "false");

            var samCommand = new SamCommand(commands, parameters);
            await this.SendCommandAsync(samCommand, cancellationToken);
        }

        for (; ; )
        {
            var (commands, parameters) = await this.ReceiveCommandAsync(cancellationToken);

            if (commands[0] == "STREAM" && commands[1] == "STATUS" && parameters["RESULT"] == "OK")
            {
                return;
            }
            else if (commands[0] == "PING")
            {
                await this.SendCommandAsync(new SamCommand(new[] { "PONG", commands[1] }), cancellationToken);
            }
            else
            {
                throw new SamBridgeException($"Stream Connect failed because of {parameters["RESULT"]}");
            }
        }
    }

    public async ValueTask<string?> StreamAcceptAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        {
            var commands = new List<string>();
            commands.Add("STREAM");
            commands.Add("ACCEPT");

            var parameters = new Dictionary<string, string>();
            parameters.Add("ID", sessionId);
            parameters.Add("SILENCE", "false");

            var samCommand = new SamCommand(commands, parameters);
            await this.SendCommandAsync(samCommand, cancellationToken);
        }

        for (; ; )
        {
            var (commands, parameters) = await this.ReceiveCommandAsync(cancellationToken);

            if (commands[0] == "STREAM" && commands[1] == "STATUS" && parameters["RESULT"] == "OK")
            {
                var line = await _reader.ReadLineAsync(cancellationToken);
                if (line is null || line.Length <= 2) return null;

                return line.Split(' ')[0];
            }
            else if (commands[0] == "PING")
            {
                await this.SendCommandAsync(new SamCommand(new[] { "PONG", commands[1] }), cancellationToken);
            }
            else
            {
                throw new SamBridgeException($"Stream Accept failed because of {parameters["RESULT"]}");
            }
        }
    }

    public async ValueTask SendCommandAsync(SamCommand samCommand, CancellationToken cancellationToken = default)
    {
        await _writer.WriteLineAsync(samCommand.ToString().ToCharArray(), cancellationToken);
        await _writer.FlushAsync();
    }

    public async ValueTask<SamCommand> ReceiveCommandAsync(CancellationToken cancellationToken = default)
    {
        var line = await _reader.ReadLineAsync(cancellationToken);
        if (line == null) return SamCommand.Empty;

        return SamCommand.Parse(line);
    }
}
