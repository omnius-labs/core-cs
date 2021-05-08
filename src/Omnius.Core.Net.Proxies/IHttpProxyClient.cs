namespace Omnius.Core.Net.Proxies
{
    public interface IHttpProxyClientFactory
    {
        IHttpProxyClient Create(string destinationHost, int destinationPort);
    }

    public interface IHttpProxyClient : IProxyClient
    {
    }
}
