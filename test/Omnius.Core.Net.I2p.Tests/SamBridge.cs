using Xunit;

namespace Omnius.Core.Net.I2p;

public class SamBridgeTest
{
    [Fact(Skip = "I2p is required")]
    public async Task SimpleTest()
    {
        var random = new Random(0);

        var samBridge1 = await SamBridge.CreateAsync("127.0.0.1", 7656, "Test Omnius.Core.Net.I2p 1");
        var samBridge2 = await SamBridge.CreateAsync("127.0.0.1", 7656, "Test Omnius.Core.Net.I2p 2");
    }
}
