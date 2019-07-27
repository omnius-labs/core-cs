using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Omnix.Network.Proxies
{
    public abstract class ProxyClientBase
    {
        public abstract void Create(Socket socket, CancellationToken token = default);

        public virtual Task CreateAsync(Socket socket, CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                this.Create(socket, token);
            });
        }
    }

    public class ProxyClientException : Exception
    {
        public ProxyClientException() { }
        public ProxyClientException(string message) : base(message) { }
        public ProxyClientException(string message, Exception innerException) : base(message, innerException) { }
    }
}
