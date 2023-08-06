using Moq;
using Omnius.Core.Net.Caps;
using Omnius.Core.Net.Connections.Bridge;
using Omnius.Core.Net.Connections.Multiplexer;
using Omnius.Core.Net.Connections.Multiplexer.V1;
using Omnius.Core.RocketPack.Remoting.Tests.Internal;
using Omnius.Core.Tasks;
using Xunit;

namespace Omnius.Core.RocketPack.Remoting;

public class RocketRemotingTest
{
    [Fact]
    public async Task SimpleFunctionTest()
    {
        var (clientSocket, serverSocket) = SocketHelper.GetSocketPair();

        var bytesPool = BytesPool.Shared;
        var batchActionDispatcher = new BatchActionDispatcher(TimeSpan.FromMilliseconds(10));
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

        var mockTestService = new Mock<ITestService>();
        mockTestService.Setup(n => n.Unary1Async(It.IsAny<TestParam>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<TestResult>(new TestResult(1)));

        var remotingConnector = new RocketRemotingCallerFactory<DefaultErrorMessage>(clientMultiplexer, bytesPool);
        var remotingAccepter = new RocketRemotingListenerFactory<DefaultErrorMessage>(serverMultiplexer, DefaultErrorMessageFactory.Default, bytesPool);

        var client = new TestServiceRemoting.Client<DefaultErrorMessage>(remotingConnector, bytesPool);
        var server = new TestServiceRemoting.Server<DefaultErrorMessage>(mockTestService.Object, remotingAccepter, bytesPool);

        var cancellationTokenSource = new CancellationTokenSource();
        var eventLoop = server.EventLoopAsync(cancellationTokenSource.Token);

        var testResult1 = await client.Unary1Async(new TestParam(1), default);
        mockTestService.Verify(n => n.Unary1Async(new TestParam(1), default), Times.AtMostOnce());

        cancellationTokenSource.Cancel();
        await Assert.ThrowsAsync<TaskCanceledException>(async () => await eventLoop);
        cancellationTokenSource.Dispose();
    }
}
