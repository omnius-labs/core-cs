using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Bridge
{
    public partial class BridgeConnection
    {
        internal class BatchAction : IBatchAction
        {
            private readonly ConnectionSender _sender;
            private readonly ConnectionReceiver _receiver;
            private readonly IBandwidthLimiter? _senderBandwidthLimiter;
            private readonly IBandwidthLimiter? _receiverBandwidthLimiter;
            private readonly Stopwatch _stopwatch;

            private static readonly TimeSpan _interval = TimeSpan.FromMilliseconds(100);

            public BatchAction(ConnectionSender sender, ConnectionReceiver receiver, IBandwidthLimiter? senderBandwidthLimiter, IBandwidthLimiter? receiverBandwidthLimiter)
            {
                _sender = sender;
                _receiver = receiver;
                _senderBandwidthLimiter = senderBandwidthLimiter;
                _receiverBandwidthLimiter = receiverBandwidthLimiter;
                _stopwatch = Stopwatch.StartNew();
            }

            public async ValueTask WaitAsync(CancellationToken cancellationToken = default)
            {
                try
                {
                    var delay = _interval - _stopwatch.Elapsed;
                    if (delay <= TimeSpan.Zero) return;

                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _logger.Debug(e);
                }
            }

            public void Run()
            {
                _stopwatch.Restart();

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
