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

            public async ValueTask WaitAsync(CancellationToken cancellationToken = default)
            {
                var tasks = new List<Task>();
                tasks.Add(_connectionMultiplexer.InternalWaitToSendAsync(cancellationToken).AsTask());
                tasks.Add(_connectionMultiplexer.InternalWaitToReceiveAsync(cancellationToken).AsTask());
                await Task.WhenAny(tasks.ToArray());
            }

            public void Run()
            {
                _connectionMultiplexer.InternalSend();
                _connectionMultiplexer.InternalReceive();
            }
        }
    }
}
