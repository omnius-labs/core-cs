using Core.Base;
using Core.Net.Caps;
using Core.Net.Connections.Bridge;
using Core.Net.Connections.Internal;
using Core.Net.Connections.Multiplexer.V1;
using Core.Tasks;
using Xunit;

namespace Core.Net.Connections.Multiplexer;

public class OmniConnectionMultiplexerTest
{
    [Fact]
    public async Task SimpleTest()
    {
        var random = new Random();

        var (clientSocket, serverSocket) = SocketHelper.GetSocketPair();

        await using var batchActionDispatcher = new BatchActionDispatcher(TimeSpan.FromMilliseconds(10));
        var systemClock = new SystemClock();

        var bridgeConnectionOptions = new BridgeConnectionOptions(1024 * 1024 * 256);
        await using var clientBridgeConnection = new BridgeConnection(new SocketCap(clientSocket), null, null, batchActionDispatcher, BytesPool.Shared, bridgeConnectionOptions);
        await using var serverBridgeConnection = new BridgeConnection(new SocketCap(serverSocket), null, null, batchActionDispatcher, BytesPool.Shared, bridgeConnectionOptions);

        var clientMultiplexerOption = new OmniConnectionMultiplexerOptions
        {
            Type = OmniConnectionMultiplexerType.Connected,
            PacketReceiveTimeout = TimeSpan.FromMinutes(1),
            MaxStreamRequestQueueSize = 3,
            MaxStreamDataSize = 1024 * 1024 * 256,
            MaxStreamDataQueueSize = 3
        };
        await using var clientMultiplexer = OmniConnectionMultiplexer.CreateV1(clientBridgeConnection, batchActionDispatcher, systemClock, BytesPool.Shared, clientMultiplexerOption);

        var serverMultiplexerOption = new OmniConnectionMultiplexerOptions
        {
            Type = OmniConnectionMultiplexerType.Accepted,
            PacketReceiveTimeout = TimeSpan.FromMinutes(1),
            MaxStreamRequestQueueSize = 3,
            MaxStreamDataSize = 1024 * 1024 * 256,
            MaxStreamDataQueueSize = 3
        };
        await using var serverMultiplexer = OmniConnectionMultiplexer.CreateV1(serverBridgeConnection, batchActionDispatcher, systemClock, BytesPool.Shared, serverMultiplexerOption);

        var clientMultiplexerHandshakeTask = clientMultiplexer.HandshakeAsync().AsTask();
        var serverMultiplexerHandshakeTask = serverMultiplexer.HandshakeAsync().AsTask();
        await Task.WhenAll(clientMultiplexerHandshakeTask, serverMultiplexerHandshakeTask);

        var connectTask = clientMultiplexer.ConnectAsync().AsTask();
        var acceptTask = serverMultiplexer.AcceptAsync().AsTask();

        var connect = await connectTask;
        var accept = await acceptTask;

        await ConnectionTestHelper.RandomSendAndReceive(random, connect, accept);
        await ConnectionTestHelper.RandomSendAndReceive(random, accept, connect);
    }
}
