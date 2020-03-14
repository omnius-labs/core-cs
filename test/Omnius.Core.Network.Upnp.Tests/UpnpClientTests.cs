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
            using (var cancellationTokenSource = new CancellationTokenSource(10 * 1000))
            {
                var upnp = new UpnpClient();

                try
                {
                    await upnp.ConnectAsync(cancellationTokenSource.Token);
                }
                catch (Exception)
                {
                    // UPnPに接続できない環境だった場合
                    return;
                }

                var ip = await upnp.GetExternalIpAddressAsync(cancellationTokenSource.Token);
                Assert.NotNull(ip);
            }
        }
    }
}
