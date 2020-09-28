using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Network.Caps;
using Omnius.Core.Network.Connections;
using Omnius.Core.RocketPack.Remoting.Tests.Internal;
using Xunit;

namespace Omnius.Core.RocketPack.Remoting
{
    public class RocketPackRpcTest
    {
        [Fact]
        public async Task SimpleFunctionTest()
        {
            var (senderSocket, receiverSocket) = SocketHelper.GetSocketPair();

            await using var dispacher = new BaseConnectionDispatcher(new BaseConnectionDispatcherOptions());
            using var senderConnection = new BaseConnection(new SocketCap(senderSocket), dispacher, new BaseConnectionOptions());
            using var receiverConnection = new BaseConnection(new SocketCap(receiverSocket), dispacher, new BaseConnectionOptions());

            await using var senderRpc = new RocketPackRpc(senderConnection, BytesPool.Shared);
            await using var receiverRpc = new RocketPackRpc(receiverConnection, BytesPool.Shared);

            var receiverAcceptTask = receiverRpc.AcceptAsync();
            var senderConnectTask = senderRpc.ConnectAsync(0);

            using var senderStream = await senderConnectTask;
            using var receiverStream = await receiverAcceptTask;

            static async ValueTask<TestResult> square(TestParam param, CancellationToken _) => new TestResult(param.Value * param.Value);

            var listenTask = receiverStream.ListenFunctionAsync<TestParam, TestResult>(square);
            var result = await senderStream.CallFunctionAsync<TestParam, TestResult>(new TestParam(11));
            await listenTask;

            Assert.Equal(121, result.Value);
        }
    }
}
