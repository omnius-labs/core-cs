using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Net.Proxies
{
    public interface IProxyClient
    {
        ValueTask ConnectAsync(Socket socket, CancellationToken cancellationToken = default);
    }

    public class ProxyClientException : Exception
    {
        public ProxyClientException()
        {
        }

        public ProxyClientException(string message)
            : base(message)
        {
        }

        public ProxyClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
