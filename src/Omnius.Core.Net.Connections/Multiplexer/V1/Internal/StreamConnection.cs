using Omnius.Core.Helpers;
using Omnius.Core.Net.Connections.Internal;
using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal;

internal sealed partial class StreamConnection : AsyncDisposableBase, IConnection
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
    private readonly ActionPipe _receiveFinishActionPipe;
    private readonly IDisposable _receiveFinishActionPipeListenerRegister;

    private readonly CancellationTokenSource _cancellationTokenSource;

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
        _receiveFinishActionPipe = new ActionPipe();
        _receiveFinishActionPipeListenerRegister = _receiveFinishActionPipe.Listener.Listen(() => ExceptionHelper.TryCatch<ObjectDisposedException>(() => this.OnReceiveFinish()));
    }

    internal void InternalFinish()
    {
        _cancellationTokenSource.Dispose();
        _sendDataMessagePipe.Dispose();
        _receiveDataMessagePipe.Dispose();
        _sendDataAcceptedMessagePipe.Dispose();

        _sender.Dispose();
        _receiver.Dispose();
        _events.Dispose();

        _sendFinishMessagePipe.Dispose();
        _receiveFinishActionPipeListenerRegister.Dispose();
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

    internal IActionCaller ReceiveDataAcceptedCaller => _receiveDataAcceptedActionPipe.Caller;

    internal IMessagePipeReader SendFinishReader => _sendFinishMessagePipe.Reader;

    internal IActionCaller ReceiveFinishCaller => _receiveFinishActionPipe.Caller;
}
