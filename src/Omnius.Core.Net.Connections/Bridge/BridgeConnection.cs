using Omnius.Core.Net.Caps;
using Omnius.Core.Net.Connections.Internal;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Bridge;

public sealed partial class BridgeConnection : AsyncDisposableBase, IConnection
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly ICap _cap;
    private readonly IBandwidthLimiter? _senderBandwidthLimiter;
    private readonly IBandwidthLimiter? _receiverBandwidthLimiter;
    private readonly IBatchActionDispatcher _batchActionDispatcher;
    private readonly IBytesPool _bytesPool;
    private readonly BridgeConnectionOptions _options;

    private readonly ConnectionSender _sender;
    private readonly ConnectionReceiver _receiver;
    private readonly ConnectionEvents _subscribers;
    private readonly BatchAction _batchAction;

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private readonly object _lockObject = new();
    private bool _canceled = false;

    public BridgeConnection(ICap cap, IBandwidthLimiter? senderBandwidthLimiter, IBandwidthLimiter? receiverBandwidthLimiter, IBatchActionDispatcher batchActionDispatcher,
        IBytesPool bytesPool, BridgeConnectionOptions options)
    {
        _cap = cap;
        _senderBandwidthLimiter = senderBandwidthLimiter;
        _receiverBandwidthLimiter = receiverBandwidthLimiter;
        _batchActionDispatcher = batchActionDispatcher;
        _bytesPool = bytesPool;
        _options = options;

        _sender = new ConnectionSender(_cap, _bytesPool, _cancellationTokenSource.Token);
        _receiver = new ConnectionReceiver(_cap, _options.MaxReceiveByteCount, _bytesPool, _cancellationTokenSource.Token);
        _subscribers = new ConnectionEvents(_cancellationTokenSource.Token);
        _batchAction = new BatchAction(_sender, _receiver, _senderBandwidthLimiter, _receiverBandwidthLimiter, this.HandleException);
        _batchActionDispatcher.Register(_batchAction);
    }

    private void HandleException(Exception e)
    {
        _logger.Debug(e);
        this.Cancel();
    }

    protected override async ValueTask OnDisposeAsync()
    {
        this.Cancel();

        _subscribers.Dispose();
        _sender.Dispose();
        _receiver.Dispose();
        _cap.Dispose();
        _cancellationTokenSource.Dispose();
    }

    private void Cancel()
    {
        lock (_lockObject)
        {
            if (_canceled) return;
            _canceled = true;

            _batchActionDispatcher.Unregister(_batchAction);
            _cancellationTokenSource.Cancel();
        }
    }

    public bool IsConnected => _cap.IsConnected;

    public IConnectionSender Sender => _sender;

    public IConnectionReceiver Receiver => _receiver;

    public IConnectionEvents Events => _subscribers;
}