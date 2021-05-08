namespace Omnius.Core.Net.Proxies
{
    public interface ISocks5ProxyClientFactory
    {
        public ISocks5ProxyClient Create(string destinationHost, int destinationPort);

        public ISocks5ProxyClient Create(string proxyUsername, string proxyPassword, string destinationHost, int destinationPort);
    }

    public interface ISocks5ProxyClient : IProxyClient
    {
    }
}
