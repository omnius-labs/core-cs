using System.Net;
using Xunit;

namespace Omnius.Core.Net;

public class OmniAddressTest
{
    [Fact]
    public async Task SimpleParseTest()
    {
        var sample = OmniAddress.CreateTcpEndpoint(IPAddress.Loopback, 32321);
        Assert.True(sample.TryGetTcpEndpoint(out var ipAddress, out var port));
        Assert.Equal(IPAddress.Loopback, ipAddress);
        Assert.Equal(32321, port);
    }
}
