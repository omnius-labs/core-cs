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

            public TimeSpan Interval { get; } = TimeSpan.FromMilliseconds(50);

            public void Execute()
            {
                _sender.InternalSend();
                _receiver.InternalReceive();
            }
        }
    }
}
