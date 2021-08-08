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

            var (clientMultiplexer, serverMultiplexer) = await GetMultiplexerPair();

            var connectTask = clientMultiplexer.ConnectAsync().AsTask();
            var acceptTask = serverMultiplexer.AcceptAsync().AsTask();

            await TestHelper.RandomSendAndReceive(random, connectTask.Result, acceptTask.Result);
            await TestHelper.RandomSendAndReceive(random, acceptTask.Result, connectTask.Result);
        }

        public static async Task<(IMultiplexer, IMultiplexer)> GetMultiplexerPair()
        {
            var (clientSocket, serverSocket) = SocketHelper.GetSocketPair();

            var bytesPool = BytesPool.Shared;
            var batchActionDispatcher = new BatchActionDispatcher();

            var bridgeConnectionOptions = new BridgeConnectionOptions(1024 * 1024 * 256, null, null, batchActionDispatcher, bytesPool);
            var clientBridgeConnection = new BridgeConnection(new SocketCap(clientSocket), bridgeConnectionOptions);
            var serverBridgeConnection = new BridgeConnection(new SocketCap(serverSocket), bridgeConnectionOptions);

            var clientMultiplexerOption = new ConnectionMultiplexerOptions(OmniConnectionMultiplexerType.Connected, TimeSpan.FromMinutes(1), 3, 1024 * 1024 * 4, 3);
            var clientMultiplexer = new OmniConnectionMultiplexer(clientBridgeConnection, batchActionDispatcher, bytesPool, clientMultiplexerOption);

            var serverMultiplexerOption = new ConnectionMultiplexerOptions(OmniConnectionMultiplexerType.Accepted, TimeSpan.FromMinutes(1), 3, 1024 * 1024 * 4, 3);
            var serverMultiplexer = new OmniConnectionMultiplexer(serverBridgeConnection, batchActionDispatcher, bytesPool, serverMultiplexerOption);

            var clientMultiplexerHandshakeTask = clientMultiplexer.HandshakeAsync().AsTask();
            var serverMultiplexerHandshakeTask = serverMultiplexer.HandshakeAsync().AsTask();
            await Task.WhenAll(clientMultiplexerHandshakeTask, serverMultiplexerHandshakeTask);

            return (clientMultiplexer, serverMultiplexer);
        }
    }
}
