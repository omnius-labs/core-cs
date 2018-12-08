using System;
using System.Threading.Tasks;
using Omnix.Net.Upnp;
using Xunit;

namespace Omnix.Network.Upnp.Tests
{
    public class UpnpClientTests
    {
        [Fact]
        public async Task GetExternalIpAddressTest()
        {
            var upnp = new UpnpClient();

            await upnp.Connect();

            if (!upnp.IsConnected) return;

            var ip = await upnp.GetExternalIpAddress();
            Assert.True(ip != null);
        }
    }
}
