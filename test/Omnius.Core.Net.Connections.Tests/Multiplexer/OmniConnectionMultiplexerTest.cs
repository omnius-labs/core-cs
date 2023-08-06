using Omnius.Core.Net.Caps;
using Omnius.Core.Net.Connections.Bridge;
using Omnius.Core.Net.Connections.Internal;
using Omnius.Core.Net.Connections.Multiplexer.V1;
using Omnius.Core.Tasks;
using Xunit;

namespace Omnius.Core.Net.Connections.Multiplexer;

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

        await ConnectionTestHelper.RandomSendAndReceive(random, connectTask.Result, acceptTask.Result);
        await ConnectionTestHelper.RandomSendAndReceive(random, acceptTask.Result, connectTask.Result);
    }
}
