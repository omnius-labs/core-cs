using Microsoft.Extensions.Logging;

namespace Omnius.Yamux.Internal;

internal sealed class FrameReceiver
{
    private readonly FrameReader _reader;
    private readonly StreamMessageHandler _streamHandler;
    private readonly Func<Header, CancellationToken, ValueTask> _pingHandler;
    private readonly Func<Header, CancellationToken, ValueTask> _goAwayHandler;
    private readonly ILogger _logger;
    private readonly Func<YamuxErrorCode, ValueTask> _exitAsync;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private Task? _runTask;
    private int _started;

    public FrameReceiver(FrameReader reader, StreamMessageHandler streamHandler, Func<Header, CancellationToken, ValueTask> pingHandler, Func<Header, CancellationToken, ValueTask> goAwayHandler, ILogger logger, Func<YamuxErrorCode, ValueTask> exitAsync, CancellationToken cancellationToken)
    {
        _reader = reader;
        _streamHandler = streamHandler;
        _pingHandler = pingHandler;
        _goAwayHandler = goAwayHandler;
        _logger = logger;
        _exitAsync = exitAsync;
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    }

    public void Complete()
    {
        _cancellationTokenSource.Cancel();
    }

    public Task Completion => _runTask ?? Task.CompletedTask;

    public void Start()
    {
        if (Interlocked.Exchange(ref _started, 1) == 1) return;
        _runTask = this.RunAsync(_cancellationTokenSource.Token);
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            for (; ; )
            {
                await this.ReceiveAndDispatchAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException e)
        {
            _logger.LogInformation(e, "yamux: receive loop canceled");
        }
        catch (YamuxException e)
        {
            _logger.LogWarning(e, "yamux: receive loop error {ErrorCode}", e.ErrorCode);
            await _exitAsync(e.ErrorCode);
        }
    }

    private async ValueTask ReceiveAndDispatchAsync(CancellationToken cancellationToken)
    {
        Header header = await _reader.ReadHeaderAsync(cancellationToken);

        if (header.Version != Constants.PROTO_VERSION)
        {
            throw new YamuxException(YamuxErrorCode.InvalidVersion);
        }

        switch (header.Type)
        {
            case MessageType.Data:
            case MessageType.WindowUpdate:
                await _streamHandler.HandleAsync(header, cancellationToken);
                break;
            case MessageType.Ping:
                await _pingHandler(header, cancellationToken);
                break;
            case MessageType.GoAway:
                await _goAwayHandler(header, cancellationToken);
                break;
            default:
                throw new YamuxException(YamuxErrorCode.InvalidFrameType);
        }
    }
}
