using Core.Tasks;

namespace Core.Net.Connections.Bridge;

public partial class BridgeConnection
{
    internal class BatchAction : IBatchAction
    {
        private readonly ConnectionSender _sender;
        private readonly ConnectionReceiver _receiver;
        private readonly IBandwidthLimiter? _senderBandwidthLimiter;
        private readonly IBandwidthLimiter? _receiverBandwidthLimiter;
        private readonly Action<Exception> _exceptionCallback;

        public BatchAction(ConnectionSender sender, ConnectionReceiver receiver, IBandwidthLimiter? senderBandwidthLimiter, IBandwidthLimiter? receiverBandwidthLimiter, Action<Exception> exceptionCallback)
        {
            _sender = sender;
            _receiver = receiver;
            _senderBandwidthLimiter = senderBandwidthLimiter;
            _receiverBandwidthLimiter = receiverBandwidthLimiter;
            _exceptionCallback = exceptionCallback;
        }

        public TimeSpan Interval { get; } = TimeSpan.FromMilliseconds(10);

        public void Execute()
        {
            try
            {
                this.Send();
            }
            catch (Exception e)
            {
                _logger.Debug(e, "Send Exception");
                _exceptionCallback.Invoke(e);
            }

            try
            {
                this.Receive();
            }
            catch (Exception e)
            {
                _logger.Debug(e, "Receive Exception");
                _exceptionCallback.Invoke(e);
            }
        }

        private void Send()
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

        private void Receive()
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
    }
}
