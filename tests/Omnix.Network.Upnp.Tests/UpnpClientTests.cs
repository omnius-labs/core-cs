using System;
using System.Threading;
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
            using (var tokenSource = new CancellationTokenSource(10 * 1000))
            {
                var upnp = new UpnpClient();

                await upnp.Connect(tokenSource.Token);

                if (!upnp.IsConnected) return;

                var ip = await upnp.GetExternalIpAddress(tokenSource.Token);
                Assert.True(ip != null);
            }
        }
    }
}
