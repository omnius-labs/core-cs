using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Net.Connections
{
    public interface IConnectionMultiplexer : IAsyncDisposable
    {
        bool IsConnected { get; }

        ValueTask<IConnection> ConnectAsync(CancellationToken cancellationToken = default);

        ValueTask<IConnection> AcceptAsync(CancellationToken cancellationToken = default);
    }
}
