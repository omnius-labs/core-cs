using System;

namespace Omnius.Core.Net.Connections
{
    public interface IMultiplexer : IConnectionConnector, IConnectionAccepter, IAsyncDisposable
    {
        bool IsConnected { get; }
    }
}
