using Xunit;

namespace Omnius.Core.Net.Upnp;

public class UpnpClientTest
{
    [Fact(Skip = "UPnP-enabled router is required")]
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
