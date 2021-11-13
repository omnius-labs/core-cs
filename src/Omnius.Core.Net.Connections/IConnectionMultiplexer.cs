namespace Omnius.Core.Net.Connections;

public interface IConnectionMultiplexer : IAsyncDisposable
{
    bool IsConnected { get; }

    ValueTask<IConnection> ConnectAsync(CancellationToken cancellationToken = default);

    ValueTask<IConnection> AcceptAsync(CancellationToken cancellationToken = default);
}