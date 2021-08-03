using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Secure.V1.Internal
{
    public partial class SecureConnection
    {
        internal class BatchAction : IBatchAction
        {
            private readonly ConnectionSender _sender;
            private readonly ConnectionReceiver _receiver;

            public BatchAction(ConnectionSender sender, ConnectionReceiver receiver)
            {
                _sender = sender;
                _receiver = receiver;
            }

            public async ValueTask WaitAsync(CancellationToken cancellationToken = default)
            {
                try
                {
                    var tasks = new List<Task>();
                    tasks.Add(_sender.InternalWaitAsync(cancellationToken).AsTask());
                    tasks.Add(_receiver.InternalWaitAsync(cancellationToken).AsTask());
                    await Task.WhenAny(tasks);
                }
                catch (Exception e)
                {
                    _logger.Debug(e);
                }
            }

            public void Run()
            {
                _sender.InternalSend();
                _receiver.InternalReceive();
            }
        }
    }
}
