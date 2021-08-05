using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Net.Connections
{
    public interface IConnectionAccepter
    {
        ValueTask<IConnection> AcceptAsync(CancellationToken cancellationToken = default);
    }
}
