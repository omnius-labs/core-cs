using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Network.Proxies
{
    public abstract class ProxyClientBase
    {
        public abstract void Create(Socket socket, CancellationToken cancellationToken = default);

        public virtual Task CreateAsync(Socket socket, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                this.Create(socket, cancellationToken);
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
