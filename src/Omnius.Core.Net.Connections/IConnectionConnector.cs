using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Net.Connections
{
    public interface IConnectionConnector
    {
        ValueTask<IConnection> ConnectAsync(CancellationToken cancellationToken = default);
    }
}
