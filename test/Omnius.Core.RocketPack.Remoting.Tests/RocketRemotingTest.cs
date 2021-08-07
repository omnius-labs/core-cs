using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Moq;
using Omnius.Core.Net.Caps;
using Omnius.Core.Net.Connections.Bridge;
using Omnius.Core.Net.Connections.Multiplexer;
using Omnius.Core.Net.Connections.Multiplexer.V1;
using Omnius.Core.RocketPack.Remoting.Tests.Internal;
using Omnius.Core.Tasks;
using Xunit;

namespace Omnius.Core.RocketPack.Remoting
{
    public class RocketRemotingTest
    {
        [Fact]
        public async Task SimpleFunctionTest()
        {
            var (clientSocket, serverSocket) = SocketHelper.GetSocketPair();

            var bytesPool = BytesPool.Shared;
            var batchActionDispatcher = new BatchActionDispatcher();

            var bridgeConnectionOptions = new BridgeConnectionOptions(1024 * 1024 * 256, null, null, batchActionDispatcher, bytesPool);
            await using var clientBridgeConnection = new BridgeConnection(new SocketCap(clientSocket), bridgeConnectionOptions);
            await using var serverBridgeConnection = new BridgeConnection(new SocketCap(serverSocket), bridgeConnectionOptions);

            var clientMultiplexerOption = new ConnectionMultiplexerOptions(OmniConnectionMultiplexerType.Connected, TimeSpan.FromMinutes(1), 3, 1024 * 1024 * 4, 3);
            await using var clientMultiplexer = new OmniConnectionMultiplexer(clientBridgeConnection, batchActionDispatcher, bytesPool, clientMultiplexerOption);

            var serverMultiplexerOption = new ConnectionMultiplexerOptions(OmniConnectionMultiplexerType.Accepted, TimeSpan.FromMinutes(1), 3, 1024 * 1024 * 4, 3);
            await using var serverMultiplexer = new OmniConnectionMultiplexer(serverBridgeConnection, batchActionDispatcher, bytesPool, serverMultiplexerOption);

            var clientMultiplexerHandshakeTask = clientMultiplexer.HandshakeAsync().AsTask();
            var serverMultiplexerHandshakeTask = serverMultiplexer.HandshakeAsync().AsTask();
            await Task.WhenAll(clientMultiplexerHandshakeTask, serverMultiplexerHandshakeTask);

            var mockTestService = new Mock<ITestService>();
            mockTestService.Setup(n => n.Unary1Async(It.IsAny<TestParam>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<TestResult>(new TestResult(1)));

            var remotingConnector = new RocketRemotingCallerFactory<DefaultErrorMessage>(clientMultiplexer, bytesPool);
            var remotingAccepter = new RocketRemotingListenerFactory<DefaultErrorMessage>(serverMultiplexer, DefaultErrorMessageFactory.Default, bytesPool);

            var client = new TestService.Client<DefaultErrorMessage>(remotingConnector, bytesPool);
            var server = new TestService.Server<DefaultErrorMessage>(mockTestService.Object, remotingAccepter, bytesPool);

            var cancellationTokenSource = new CancellationTokenSource();
            var eventLoop = server.EventLoopAsync(cancellationTokenSource.Token);

            var testResult1 = await client.Unary1Async(new TestParam(1), default);
            mockTestService.Verify(n => n.Unary1Async(new TestParam(1), default), Times.AtMostOnce());

            cancellationTokenSource.Cancel();
            await Assert.ThrowsAsync<TaskCanceledException>(async () => await eventLoop);
            cancellationTokenSource.Dispose();
        }
    }
}
