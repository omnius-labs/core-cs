using System;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Secure.V1.Internal
{
    public partial class SecureConnection
    {
        internal class BatchAction : IBatchAction
        {
            private readonly ConnectionSender _sender;
            private readonly ConnectionReceiver _receiver;
            private readonly Action<Exception> _exceptionCallback;

            public BatchAction(ConnectionSender sender, ConnectionReceiver receiver, Action<Exception> exceptionCallback)
            {
                _sender = sender;
                _receiver = receiver;
                _exceptionCallback = exceptionCallback;
            }

            public TimeSpan Interval { get; } = TimeSpan.FromMilliseconds(50);

            public void Execute()
            {
                try
                {
                    _sender.InternalSend();
                }
                catch (Exception e)
                {
                    _exceptionCallback.Invoke(e);
                }

                try
                {
                    _receiver.InternalReceive();
                }
                catch (Exception e)
                {
                    _exceptionCallback.Invoke(e);
                }
            }
        }
    }
}
