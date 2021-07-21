using System;
using System.Buffers;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Omnius.Core.Helpers;
using Omnius.Core.Net.Connections.Extensions;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal
{
    internal partial class ConnectionMultiplexer
    {
        internal sealed class StreamConnection : IConnection
        {
            private readonly ChannelReader<ConnectionMultiplexerMessage> _inputReader;
            private readonly ChannelWriter<ConnectionMultiplexerMessage> _outputWriter;

            public StreamConnection(ChannelReader<ConnectionMultiplexerMessage> inputReader, ChannelWriter<ConnectionMultiplexerMessage> outputWriter)
            {
                _inputReader = inputReader;
                _outputWriter = outputWriter;
            }

            public async ValueTask ConnectAsync(CancellationToken cancellationToken = default)
            {
                var connectionRequestMessage = new ConnectionMultiplexerMessage(ConnectionMultiplexerMessageType.ConnectionRequest, null);
                await _outputWriter.WriteAsync(connectionRequestMessage, cancellationToken);

                var receivedMessage = await _inputReader.ReadAsync(cancellationToken);
                if (receivedMessage.Type != ConnectionMultiplexerMessageType.ConnectionRequestAccepted) throw new Exception(); // TODO
            }

            public async ValueTask AcceptAsync(CancellationToken cancellationToken = default)
            {
                var receivedMessage = await _inputReader.ReadAsync(cancellationToken);
                if (receivedMessage.Type != ConnectionMultiplexerMessageType.ConnectionRequest) throw new Exception(); // TODO

                var connectionRequestMessage = new ConnectionMultiplexerMessage(ConnectionMultiplexerMessageType.ConnectionRequestAccepted, null);
                await _outputWriter.WriteAsync(connectionRequestMessage, cancellationToken);
            }
        }
    }
}
