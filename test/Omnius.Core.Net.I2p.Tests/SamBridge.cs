using System.Net;
using Omnius.Core.Net.I2p.Internal;
using Xunit;

namespace Omnius.Core.Net.I2p;

public class SamBridgeTest
{
    [Fact(Skip = "I2p is required")]
    public async Task SimpleTest()
    {
        var random = new Random(0);

        var samBridge1 = await SamBridge.CreateAsync(IPAddress.Parse("127.0.0.1"), 7656, "Test Omnius.Core.Net.I2p 1");
        var samBridge2 = await SamBridge.CreateAsync(IPAddress.Parse("127.0.0.1"), 7656, "Test Omnius.Core.Net.I2p 2");

        var acceptResultTask = samBridge2.AcceptAsync();
        var socket1 = await samBridge1.ConnectAsync(samBridge2.Base32Address!);
        var acceptResult = await acceptResultTask;

        await ConnectionTestHelper.RandomSendAndReceiveAsync(random, socket1, acceptResult.Socket);
    }
}
