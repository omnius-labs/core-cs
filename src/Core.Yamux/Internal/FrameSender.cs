using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Omnius.Yamux;

namespace Omnius.Yamux.Internal;

internal sealed class FrameSender
{
    private readonly FrameWriter _writer;
    private readonly YamuxOptions _options;
    private readonly ILogger _logger;
    private readonly Func<YamuxErrorCode, ValueTask> _exitAsync;
    private readonly Channel<SendCommand> _sendCommands = Channel.CreateBounded<SendCommand>(64);
    private readonly Task _runTask;

    public FrameSender(FrameWriter writer, YamuxOptions options, ILogger logger, Func<YamuxErrorCode, ValueTask> exitAsync, CancellationToken cancellationToken)
    {
        _writer = writer;
        _options = options;
        _logger = logger;
        _exitAsync = exitAsync;
        _runTask = this.RunAsync(cancellationToken);
    }

    public Task Completion => _runTask;

    public async ValueTask<YamuxErrorCode> EnqueueAsync(byte[] header, byte[] payload, CancellationToken cancellationToken)
    {
        SendCommand command = new SendCommand(header, payload);

        try
        {
            await _sendCommands.Writer.WriteAsync(command, cancellationToken);
        }
        catch (ChannelClosedException)
        {
            return YamuxErrorCode.ConnectionShutdown;
        }

        return await command.TaskCompletionSource.Task;
    }

    public async ValueTask EnqueueFireAndForgetAsync(byte[] header, byte[] payload, CancellationToken cancellationToken)
    {
        SendCommand command = new SendCommand(header, payload);

        try
        {
            await _sendCommands.Writer.WriteAsync(command, cancellationToken);
        }
        catch (ChannelClosedException)
        {
        }
    }

    public void Complete()
    {
        _sendCommands.Writer.Complete();

        while (_sendCommands.Reader.TryRead(out SendCommand? command))
        {
            command.TaskCompletionSource.SetResult(YamuxErrorCode.ConnectionShutdown);
        }
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            for (; ; )
            {
                SendCommand command = await _sendCommands.Reader.ReadAsync(cancellationToken);

                using (CancellationTokenSource timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    if (_options.StreamWriteTimeout != Timeout.InfiniteTimeSpan)
                    {
                        timeoutCts.CancelAfter(_options.StreamWriteTimeout);
                    }

                    try
                    {
                        await _writer.WriteAsync(command.Header, command.Payload, timeoutCts.Token);
                        command.TaskCompletionSource.SetResult(YamuxErrorCode.None);
                    }
                    catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogWarning("yamux: send timeout");
                        command.TaskCompletionSource.SetResult(YamuxErrorCode.Timeout);
                        await _exitAsync(YamuxErrorCode.Timeout);
                    }
                    catch (OperationCanceledException e)
                    {
                        _logger.LogInformation(e, "yamux: send canceled");
                        command.TaskCompletionSource.SetResult(YamuxErrorCode.ConnectionShutdown);
                        await _exitAsync(YamuxErrorCode.ConnectionShutdown);
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e, "yamux: failed to send frame");
                        command.TaskCompletionSource.SetResult(YamuxErrorCode.ConnectionSendError);
                    }
                }
            }
        }
        catch (ChannelClosedException)
        {
            _logger.LogInformation("yamux: send loop channel closed");
        }
        catch (OperationCanceledException e)
        {
            _logger.LogInformation(e, "yamux: send loop canceled");
        }
    }

    private sealed record SendCommand
    {
        public SendCommand(byte[] header, byte[] payload)
        {
            Header = header;
            Payload = payload;
        }

        public byte[] Header { get; }
        public byte[] Payload { get; }
        public TaskCompletionSource<YamuxErrorCode> TaskCompletionSource { get; } = new();
    }
}
