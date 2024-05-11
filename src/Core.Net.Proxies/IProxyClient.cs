using System.Net.Sockets;

namespace Core.Net.Proxies;

public interface IProxyClient
{
    ValueTask ConnectAsync(Socket socket, CancellationToken cancellationToken = default);
}

public class ProxyClientException : Exception
{
    public ProxyClientException()
    {
    }

    public ProxyClientException(string? message)
        : base(message)
    {
    }

    public ProxyClientException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
