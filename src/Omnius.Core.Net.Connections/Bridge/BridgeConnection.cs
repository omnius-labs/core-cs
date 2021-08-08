using System;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Net.Caps;
using Omnius.Core.Net.Connections.Internal;
using Omnius.Core.Pipelines;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Bridge
{
    public sealed partial class BridgeConnection : AsyncDisposableBase, IConnection
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly ICap _cap;
        private readonly int _maxReceiveByteCount;
        private readonly IBandwidthLimiter? _senderBandwidthLimiter;
        private readonly IBandwidthLimiter? _receiverBandwidthLimiter;
        private readonly IBatchActionDispatcher _batchActionDispatcher;
        private readonly IBytesPool _bytesPool;

        private readonly ConnectionSender _sender;
        private readonly ConnectionReceiver _receiver;
        private readonly ConnectionEvents _subscribers;
        private readonly BatchAction _batchAction;

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public BridgeConnection(ICap cap, BridgeConnectionOptions options)
        {
            _cap = cap;
            _maxReceiveByteCount = options.MaxReceiveByteCount;
            _senderBandwidthLimiter = options.SenderBandwidthLimiter;
            _receiverBandwidthLimiter = options.ReceiverBandwidthLimiter;
            _batchActionDispatcher = options.BatchActionDispatcher;
            _bytesPool = options.BytesPool;

            _sender = new ConnectionSender(_cap, _bytesPool, _cancellationTokenSource);
            _receiver = new ConnectionReceiver(_cap, _maxReceiveByteCount, _bytesPool, _cancellationTokenSource);
            _subscribers = new ConnectionEvents(_cancellationTokenSource.Token);
            _batchAction = new BatchAction(_sender, _receiver, _senderBandwidthLimiter, _receiverBandwidthLimiter);
            _batchActionDispatcher.Register(_batchAction);
        }

        protected override async ValueTask OnDisposeAsync()
        {
            _batchActionDispatcher.Unregister(_batchAction);

            _cancellationTokenSource.Cancel();
            _subscribers.Dispose();
            _sender.Dispose();
            _receiver.Dispose();
            _cap.Dispose();
            _cancellationTokenSource.Dispose();
        }

        public bool IsConnected => _cap.IsConnected;

        public IConnectionSender Sender => _sender;

        public IConnectionReceiver Receiver => _receiver;

        public IConnectionEvents Events => _subscribers;
    }
}
