using Omnius.Core.Net.Connections.Internal;
using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal;

internal sealed partial class StreamConnection : AsyncDisposableBase, IConnection
{
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
    private readonly ActionPipe _receiveFinishActionPipe;

    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly List<IDisposable> _disposables = new();

    public StreamConnection(int maxSendDataQueueSize, int maxReceiveDataQueueSize, IBytesPool bytesPool, CancellationToken cancellationToken)
    {
        _maxSendDataQueueSize = maxSendDataQueueSize;
        _maxReceiveDataQueueSize = maxReceiveDataQueueSize;
        _bytesPool = bytesPool;

        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _cancellationTokenSource.ToAdd(_disposables);

        _sendDataMessagePipe = new BoundedMessagePipe<ArraySegment<byte>>(_maxSendDataQueueSize).ToAdd(_disposables);
        _receiveDataMessagePipe = new BoundedMessagePipe<ArraySegment<byte>>(_maxReceiveDataQueueSize).ToAdd(_disposables);
        _sendDataAcceptedMessagePipe = new BoundedMessagePipe(_maxReceiveDataQueueSize).ToAdd(_disposables);
        _receiveDataAcceptedActionPipe = new ActionPipe();

        _sender = new ConnectionSender(maxSendDataQueueSize, _sendDataMessagePipe.Writer, _receiveDataAcceptedActionPipe.Subscriber, _bytesPool, _cancellationTokenSource.Token).ToAdd(_disposables);
        _receiver = new ConnectionReceiver(_receiveDataMessagePipe.Reader, _sendDataAcceptedMessagePipe.Writer, _bytesPool, _cancellationTokenSource.Token).ToAdd(_disposables);
        _events = new ConnectionEvents(_cancellationTokenSource.Token);

        _sendFinishMessagePipe = new BoundedMessagePipe(1).ToAdd(_disposables);
        _receiveFinishActionPipe = new ActionPipe();
        _receiveFinishActionPipe.Subscriber.Subscribe(() => this.OnReceiveFinish()).ToAdd(_disposables);
    }

    internal void InternalDispose()
    {
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }
    }

    protected override async ValueTask OnDisposeAsync()
    {
        this.OnSendFinish();
    }

    private void OnSendFinish()
    {
        _cancellationTokenSource.Cancel();
        _sendFinishMessagePipe.Writer.TryWrite();
    }

    private void OnReceiveFinish()
    {
        _cancellationTokenSource.Cancel();
    }

    public bool IsConnected => !this.IsDisposed;

    public IConnectionSender Sender => _sender;

    public IConnectionReceiver Receiver => _receiver;

    public IConnectionEvents Events => _events;

    internal IMessagePipeReader<ArraySegment<byte>> SendDataReader => _sendDataMessagePipe.Reader;

    internal IMessagePipeWriter<ArraySegment<byte>> ReceiveDataWriter => _receiveDataMessagePipe.Writer;

    internal IMessagePipeReader SendDataAcceptedReader => _sendDataAcceptedMessagePipe.Reader;

    internal IActionPublicher ReceiveDataAcceptedPublicher => _receiveDataAcceptedActionPipe.Publicher;

    internal IMessagePipeReader SendFinishReader => _sendFinishMessagePipe.Reader;

    internal IActionPublicher ReceiveFinishPublicher => _receiveFinishActionPipe.Publicher;
}
