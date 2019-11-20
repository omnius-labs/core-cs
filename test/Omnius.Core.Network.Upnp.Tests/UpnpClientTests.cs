using System;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Net.Upnp;
using Xunit;

namespace Omnius.Core.Network.Upnp
{
    public class UpnpClientTests
    {
        [Fact]
        public async Task GetExternalIpAddressTest()
        {
            using (var tokenSource = new CancellationTokenSource(10 * 1000))
            {
                var upnp = new UpnpClient();

                try
                {
                    await upnp.ConnectAsync(tokenSource.Token);
                }
                catch (Exception)
                {
                    // UPnPに接続できない環境だった場合
                    return;
                }

                var ip = await upnp.GetExternalIpAddressAsync(tokenSource.Token);
                Assert.True(ip != null);
            }
        }
    }
}
