using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal
{
    internal sealed partial class ConnectionMultiplexer
    {
        private sealed class BatchAction : IBatchAction
        {
            private readonly ConnectionMultiplexer _connectionMultiplexer;

            public BatchAction(ConnectionMultiplexer connectionMultiplexer)
            {
                _connectionMultiplexer = connectionMultiplexer;
            }

            public TimeSpan Interval { get; } = TimeSpan.FromMilliseconds(100);

            public void Execute()
            {
                _connectionMultiplexer.InternalSend();
                _connectionMultiplexer.InternalReceive();
            }
        }
    }
}
