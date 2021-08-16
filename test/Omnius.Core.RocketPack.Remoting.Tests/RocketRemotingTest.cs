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
            var batchActionDispatcher = new BatchActionDispatcher(TimeSpan.FromMilliseconds(10));

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
}
