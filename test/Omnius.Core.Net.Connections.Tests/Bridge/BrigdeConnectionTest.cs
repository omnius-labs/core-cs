using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Omnius.Core.Net.Caps;
using Omnius.Core.Net.Connections.Internal;
using Omnius.Core.Tasks;
using Xunit;

namespace Omnius.Core.Net.Connections.Bridge
{
    public class BridgeConnectionTest
    {
        [Fact]
        public async Task RandomSendAndReceiveTest()
        {
            var random = new Random();

            var (socket1, socket2) = SocketHelper.GetSocketPair();

            var batchActionDispatcher = new BatchActionDispatcher(TimeSpan.FromMilliseconds(10));
            var options = new BridgeConnectionOptions(1024 * 1024 * 256, null, null, batchActionDispatcher, BytesPool.Shared);

            await using var connection1 = new BridgeConnection(new SocketCap(socket1), options);
            await using var connection2 = new BridgeConnection(new SocketCap(socket2), options);

            await TestHelper.RandomSendAndReceive(random, connection1, connection2);
            await TestHelper.RandomSendAndReceive(random, connection2, connection1);
        }
    }
}
