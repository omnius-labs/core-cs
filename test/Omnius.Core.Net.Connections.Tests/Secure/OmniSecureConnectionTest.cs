using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Omnius.Core.Net.Caps;
using Omnius.Core.Net.Connections.Bridge;
using Omnius.Core.Net.Connections.Internal;
using Omnius.Core.Tasks;
using Xunit;

namespace Omnius.Core.Net.Connections.Secure
{
    public class OmniSecureConnectionTest
    {
        [Fact]
        public async Task RandomSendAndReceiveTest()
        {
            var random = new Random();

            var (socket1, socket2) = SocketHelper.GetSocketPair();

            var batchActionDispatcher = new BatchActionDispatcher();
            var options = new BridgeConnectionOptions(1024 * 1024 * 256, null, null, batchActionDispatcher, BytesPool.Shared);

            await using var bridgeConnection1 = new BridgeConnection(new SocketCap(socket1), options);
            await using var bridgeConnection2 = new BridgeConnection(new SocketCap(socket2), options);
            await using var connection1 = new OmniSecureConnection(bridgeConnection1, new OmniSecureConnectionOptions(1024 * 1024 * 256, OmniSecureConnectionType.Connected, null, batchActionDispatcher, BytesPool.Shared));
            await using var connection2 = new OmniSecureConnection(bridgeConnection2, new OmniSecureConnectionOptions(1024 * 1024 * 256, OmniSecureConnectionType.Accepted, null, batchActionDispatcher, BytesPool.Shared));

            // ハンドシェイクを行う
            var valueTask1 = connection1.HandshakeAsync();
            var valueTask2 = connection2.HandshakeAsync();
            await Task.WhenAll(valueTask1.AsTask(), valueTask2.AsTask());

            await TestHelper.RandomSendAndReceive(random, connection1, connection2);
            await TestHelper.RandomSendAndReceive(random, connection2, connection1);
        }
    }
}
