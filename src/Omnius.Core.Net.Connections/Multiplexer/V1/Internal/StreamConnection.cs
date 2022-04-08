using Omnius.Core.Net.Connections.Internal;
using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal;

internal sealed partial class StreamConnection : IConnection
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly int _maxSendDataQueueSize;
    private readonly int _maxReceiveDataQueueSize;
    private readonly IBytesPool _bytesPool;

    private readonly BoundedMessagePipe<ArraySegment<byte>> _sendDataMessagePipe;
    private readonly BoundedMessagePipe<ArraySegment<byte>> _receiveDataMessagePipe;
    private readonly BoundedMessagePipe _sendDataAcceptedMessagePipe;
    private readonly ActionPipe _receiveDataAcceptedActionPipe;
    private readonly ConnectionSender _sender;
    private readonly ConnectionReceiver _receiver;
    private readonly ConnectionEvents _events;

    private readonly BoundedMessagePipe _sendFinishMessagePipe;

    private readonly CancellationTokenSource _cancellationTokenSource;
    private bool _disposed = false;

    private readonly object _lockObject = new();

    public StreamConnection(int maxSendDataQueueSize, int maxReceiveDataQueueSize, IBytesPool bytesPool, CancellationToken cancellationToken)
    {
        _maxSendDataQueueSize = maxSendDataQueueSize;
        _maxReceiveDataQueueSize = maxReceiveDataQueueSize;
        _bytesPool = bytesPool;

        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _sendDataMessagePipe = new BoundedMessagePipe<ArraySegment<byte>>(_maxSendDataQueueSize);
        _receiveDataMessagePipe = new BoundedMessagePipe<ArraySegment<byte>>(_maxReceiveDataQueueSize);
        _sendDataAcceptedMessagePipe = new BoundedMessagePipe(_maxReceiveDataQueueSize);
        _receiveDataAcceptedActionPipe = new ActionPipe();

        _sender = new ConnectionSender(maxSendDataQueueSize, _sendDataMessagePipe.Writer, _receiveDataAcceptedActionPipe.Listener, _bytesPool, _cancellationTokenSource.Token);
        _receiver = new ConnectionReceiver(_receiveDataMessagePipe.Reader, _sendDataAcceptedMessagePipe.Writer, _bytesPool, _cancellationTokenSource.Token);
        _events = new ConnectionEvents(_cancellationTokenSource.Token);

        _sendFinishMessagePipe = new BoundedMessagePipe(1);
    }

    public async ValueTask DisposeAsync()
    {
        this.InternalStop();
    }

    internal void InternalStop()
    {
        lock (_lockObject)
        {
            if (_disposed) return;

            _sendFinishMessagePipe.Writer.TryWrite();

            if (!_cancellationTokenSource.IsCancellationRequested) _cancellationTokenSource.Cancel();
        }
    }

    internal void InternalDispose()
    {
        lock (_lockObject)
        {
            if (_disposed) return;
            _disposed = true;

            if (!_cancellationTokenSource.IsCancellationRequested) _cancellationTokenSource.Cancel();

            _cancellationTokenSource.Dispose();
            _sendDataMessagePipe.Dispose();
            _receiveDataMessagePipe.Dispose();
            _sendDataAcceptedMessagePipe.Dispose();

            _sender.Dispose();
            _receiver.Dispose();
            _events.Dispose();

            _sendFinishMessagePipe.Dispose();
        }
    }

    public bool IsConnected => !_disposed;

    public IConnectionSender Sender => _sender;

    public IConnectionReceiver Receiver => _receiver;

    public IConnectionEvents Events => _events;

    internal IMessagePipeReader<ArraySegment<byte>> SendDataReader => _sendDataMessagePipe.Reader;

    internal IMessagePipeWriter<ArraySegment<byte>> ReceiveDataWriter => _receiveDataMessagePipe.Writer;

    internal IMessagePipeReader SendDataAcceptedReader => _sendDataAcceptedMessagePipe.Reader;

    internal IActionCaller ReceiveDataAcceptedCaller => _receiveDataAcceptedActionPipe.Caller;

    internal IMessagePipeReader SendFinishReader => _sendFinishMessagePipe.Reader;
}
