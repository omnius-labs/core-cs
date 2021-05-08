using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Moq;
using Omnius.Core.Net.Caps;
using Omnius.Core.Net.Connections;
using Omnius.Core.Remoting.Tests.Internal;
using Xunit;

namespace Omnius.Core.Remoting
{
    public class RemotingTest
    {
        [Fact]
        public async Task SimpleFunctionTest()
        {
            var (senderSocket, receiverSocket) = SocketHelper.GetSocketPair();

            await using var dispacher = new BaseConnectionDispatcher(new BaseConnectionDispatcherOptions());
            using var senderConnection = new BaseConnection(new SocketCap(senderSocket), dispacher, new BaseConnectionOptions());
            using var receiverConnection = new BaseConnection(new SocketCap(receiverSocket), dispacher, new BaseConnectionOptions());

            var mockTestService = new Mock<ITestService>();
            mockTestService.Setup(n => n.Unary1Async(It.IsAny<TestParam>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<TestResult>(new TestResult(1)));

            var client = new TestService.Client(senderConnection, BytesPool.Shared);
            var server = new TestService.Server(mockTestService.Object, receiverConnection, BytesPool.Shared);

            var eventLoop = server.EventLoopAsync();

            var testResult1 = await client.Unary1Async(new TestParam(1), default);
            mockTestService.Verify(n => n.Unary1Async(new TestParam(1), default), Times.AtMostOnce());

            await client.DisposeAsync();

            await Assert.ThrowsAsync<ChannelClosedException>(() => eventLoop);
        }
    }
}
