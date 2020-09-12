using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core;
using Omnius.Core.Network.Caps;
using Omnius.Core.Network.Connections;
using Omnius.Core.RocketPack.Remoting.Tests.Internal;
using Xunit;

namespace Omnius.Core.RocketPack.Remoting
{
    public class RocketPackRpcTests
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

            static async ValueTask<RpcResult> square(RpcParam param, CancellationToken _) => new RpcResult(param.P1 * param.P1);

            var listenTask = receiverStream.ListenFunctionAsync<RpcParam, RpcResult>(square);
            var result = await senderStream.CallFunctionAsync<RpcParam, RpcResult>(new RpcParam(11));
            await listenTask;

            Assert.Equal(121, result.R1);
        }
    }
}
