using Omnius.Core.Net.Caps;
using Omnius.Core.Net.Connections.Internal;
using Omnius.Core.Tasks;
using Xunit;

namespace Omnius.Core.Net.Connections.Bridge;

public class BridgeConnectionTest
{
    [Fact]
    public async Task RandomSendAndReceiveTest()
    {
        var random = new Random();

        var (socket1, socket2) = SocketHelper.GetSocketPair();

        await using var batchActionDispatcher = new BatchActionDispatcher(TimeSpan.FromMilliseconds(10));

        var options = new BridgeConnectionOptions(1024 * 1024 * 256);
        await using var connection1 = new BridgeConnection(new SocketCap(socket1), null, null, batchActionDispatcher, BytesPool.Shared, options);
        await using var connection2 = new BridgeConnection(new SocketCap(socket2), null, null, batchActionDispatcher, BytesPool.Shared, options);

        await ConnectionTestHelper.RandomSendAndReceive(random, connection1, connection2);
        await ConnectionTestHelper.RandomSendAndReceive(random, connection2, connection1);
    }

    [Fact]
    public async Task DisconnectTest()
    {
        var random = new Random();

        var (socket1, socket2) = SocketHelper.GetSocketPair();

        await using var batchActionDispatcher = new BatchActionDispatcher(TimeSpan.FromMilliseconds(10));

        var options = new BridgeConnectionOptions(1024 * 1024 * 256);
        var connection1 = new BridgeConnection(new SocketCap(socket1), null, null, batchActionDispatcher, BytesPool.Shared, options);
        var connection2 = new BridgeConnection(new SocketCap(socket2), null, null, batchActionDispatcher, BytesPool.Shared, options);

        await connection1.DisposeAsync();

        await Assert.ThrowsAsync<ObjectDisposedException>(async () => { await connection1.Sender.SendAsync(_ => { }); });
        Assert.Throws<ObjectDisposedException>(() => { connection1.Sender.TrySend(_ => { }); });
        await Assert.ThrowsAsync<ObjectDisposedException>(async () => { await connection1.Receiver.ReceiveAsync(_ => { }); });
        Assert.Throws<ObjectDisposedException>(() => { connection1.Receiver.TryReceive(_ => { }); });

        socket2.Dispose();

        await Assert.ThrowsAsync<ConnectionException>(async () => { await connection2.Sender.SendAsync(_ => { }); });
        Assert.Throws<ConnectionException>(() => { connection2.Sender.TrySend(_ => { }); });
        await Assert.ThrowsAsync<ConnectionException>(async () => { await connection2.Receiver.ReceiveAsync(_ => { }); });
        Assert.Throws<ConnectionException>(() => { connection2.Receiver.TryReceive(_ => { }); });

        await connection2.DisposeAsync();

        await Assert.ThrowsAsync<ObjectDisposedException>(async () => { await connection2.Sender.SendAsync(_ => { }); });
        Assert.Throws<ObjectDisposedException>(() => { connection2.Sender.TrySend(_ => { }); });
        await Assert.ThrowsAsync<ObjectDisposedException>(async () => { await connection2.Receiver.ReceiveAsync(_ => { }); });
        Assert.Throws<ObjectDisposedException>(() => { connection2.Receiver.TryReceive(_ => { }); });
    }
}
