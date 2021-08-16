using System;
using System.Threading.Tasks;
using Omnius.Core.Net.Caps;
using Omnius.Core.Net.Connections.Bridge;
using Omnius.Core.Net.Connections.Internal;
using Omnius.Core.Net.Connections.Multiplexer.V1;
using Omnius.Core.Tasks;
using Xunit;

namespace Omnius.Core.Net.Connections.Multiplexer
{
    public class OmniConnectionMultiplexerTest
    {
        [Fact]
        public async Task SimpleTest()
        {
            var random = new Random();

            var (clientSocket, serverSocket) = SocketHelper.GetSocketPair();

            await using var batchActionDispatcher = new BatchActionDispatcher(TimeSpan.FromMilliseconds(10));

            var bridgeConnectionOptions = new BridgeConnectionOptions
            {
                MaxReceiveByteCount = 1024 * 1024 * 256,
                BatchActionDispatcher = batchActionDispatcher,
                BytesPool = BytesPool.Shared,
            };
            await using var clientBridgeConnection = new BridgeConnection(new SocketCap(clientSocket), bridgeConnectionOptions);
            await using var serverBridgeConnection = new BridgeConnection(new SocketCap(serverSocket), bridgeConnectionOptions);

            var clientMultiplexerOption = new OmniConnectionMultiplexerOptions
            {
                Type = OmniConnectionMultiplexerType.Connected,
                PacketReceiveTimeout = TimeSpan.FromMinutes(1),
                MaxStreamRequestQueueSize = 3,
                MaxStreamDataSize = 1024 * 1024 * 4,
                MaxStreamDataQueueSize = 3,
                BatchActionDispatcher = batchActionDispatcher,
                BytesPool = BytesPool.Shared,
            };
            await using var clientMultiplexer = new OmniConnectionMultiplexer(clientBridgeConnection, clientMultiplexerOption);

            var serverMultiplexerOption = new OmniConnectionMultiplexerOptions
            {
                Type = OmniConnectionMultiplexerType.Connected,
                PacketReceiveTimeout = TimeSpan.FromMinutes(1),
                MaxStreamRequestQueueSize = 3,
                MaxStreamDataSize = 1024 * 1024 * 4,
                MaxStreamDataQueueSize = 3,
                BatchActionDispatcher = batchActionDispatcher,
                BytesPool = BytesPool.Shared,
            };
            await using var serverMultiplexer = new OmniConnectionMultiplexer(serverBridgeConnection, serverMultiplexerOption);

            var clientMultiplexerHandshakeTask = clientMultiplexer.HandshakeAsync().AsTask();
            var serverMultiplexerHandshakeTask = serverMultiplexer.HandshakeAsync().AsTask();
            await Task.WhenAll(clientMultiplexerHandshakeTask, serverMultiplexerHandshakeTask);

            var connectTask = clientMultiplexer.ConnectAsync().AsTask();
            var acceptTask = serverMultiplexer.AcceptAsync().AsTask();

            await TestHelper.RandomSendAndReceive(random, connectTask.Result, acceptTask.Result);
            await TestHelper.RandomSendAndReceive(random, acceptTask.Result, connectTask.Result);
        }
    }
}
