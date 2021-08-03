using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections
{
    public partial class BaseConnection
    {
        internal class BatchAction : IBatchAction
        {
            private readonly ConnectionSender _sender;
            private readonly ConnectionReceiver _receiver;
            private readonly IBandwidthLimiter? _senderBandwidthLimiter;
            private readonly IBandwidthLimiter? _receiverBandwidthLimiter;

            public BatchAction(ConnectionSender sender, ConnectionReceiver receiver, IBandwidthLimiter? senderBandwidthLimiter, IBandwidthLimiter? receiverBandwidthLimiter)
            {
                _sender = sender;
                _receiver = receiver;
                _senderBandwidthLimiter = senderBandwidthLimiter;
                _receiverBandwidthLimiter = receiverBandwidthLimiter;
            }

            public async ValueTask WaitAsync(CancellationToken cancellationToken = default)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _logger.Debug(e);
                }
            }

            public void Run()
            {
                this.Send();
                this.Receive();
            }

            private void Send()
            {
                try
                {
                    if (_senderBandwidthLimiter is null)
                    {
                        _sender.InternalSend(int.MaxValue);
                        return;
                    }

                    lock (_senderBandwidthLimiter.LockObject)
                    {
                        var freeBytes = _senderBandwidthLimiter.ComputeFreeBytes();
                        var consumedBytes = _sender.InternalSend(freeBytes);
                        _senderBandwidthLimiter.AddConsumedBytes(consumedBytes);
                    }
                }
                catch (Exception e)
                {
                    _logger.Debug(e);
                }
            }

            private void Receive()
            {
                try
                {
                    if (_receiverBandwidthLimiter is null)
                    {
                        _receiver.InternalReceive(int.MaxValue);
                        return;
                    }

                    lock (_receiverBandwidthLimiter.LockObject)
                    {
                        var freeBytes = _receiverBandwidthLimiter.ComputeFreeBytes();
                        var consumedBytes = _receiver.InternalReceive(freeBytes);
                        _receiverBandwidthLimiter.AddConsumedBytes(consumedBytes);
                    }
                }
                catch (Exception e)
                {
                    _logger.Debug(e);
                }
            }
        }
    }
}
