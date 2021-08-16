using System;

namespace Omnius.Core.Net.Connections
{
    public interface IConnectionMultiplexer : IConnectionConnector, IConnectionAccepter, IAsyncDisposable
    {
        bool IsConnected { get; }
    }
}
