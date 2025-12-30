using Microsoft.Extensions.Logging;
using Omnius.Yamux;

namespace Omnius.Yamux.Internal;

internal sealed class StreamMessageHandler
{
    private readonly StreamRegistry _registry;
    private readonly FrameReader _reader;
    private readonly Stream _networkStream;
    private readonly Func<GoAwayCode> _localGoAwayCode;
    private readonly Func<Header, ReadOnlyMemory<byte>, CancellationToken, ValueTask> _sendFrameFireAndForget;
    private readonly ILogger _logger;

    public StreamMessageHandler(
        StreamRegistry registry,
        FrameReader reader,
        Stream networkStream,
        Func<GoAwayCode> localGoAwayCode,
        Func<Header, ReadOnlyMemory<byte>, CancellationToken, ValueTask> sendFrameFireAndForget,
        ILogger logger)
    {
        _registry = registry;
        _reader = reader;
        _networkStream = networkStream;
        _localGoAwayCode = localGoAwayCode;
        _sendFrameFireAndForget = sendFrameFireAndForget;
        _logger = logger;
    }

    public async ValueTask HandleAsync(Header header, CancellationToken cancellationToken)
    {
        if (header.Flags.HasFlag(MessageFlag.SYN))
        {
            await this.HandleIncomingAsync(header.StreamId, cancellationToken);
        }

        if (!_registry.TryGet(header.StreamId, out YamuxStream stream))
        {
            if (header.Type == MessageType.Data && header.Length > 0)
            {
                _logger.LogWarning("yamux: discarding data for stream: {0}", header.StreamId);
                await _reader.DiscardPayloadAsync(header.Length, header.StreamId, cancellationToken);
            }
            else
            {
                _logger.LogWarning("yamux: frame for missing stream: {0}", header);
            }

            return;
        }

        try
        {
            if (header.Type == MessageType.WindowUpdate)
            {
                stream.AddSendWindow(header);
            }
            else if (header.Type == MessageType.Data)
            {
                await stream.EnqueueReadBytesAsync(header, _networkStream, cancellationToken);
            }
        }
        catch (YamuxException e)
        {
            _logger.LogWarning(e, "yamux: failed to send go away");
            Header header2 = new Header(MessageType.GoAway, MessageFlag.None, 0, (uint)GoAwayCode.ProtocolError);
            await _sendFrameFireAndForget(header2, ReadOnlyMemory<byte>.Empty, cancellationToken);
        }
    }

    private async ValueTask HandleIncomingAsync(uint streamId, CancellationToken cancellationToken)
    {
        if (!_registry.AcceptEnabled || _localGoAwayCode() != GoAwayCode.None)
        {
            Header header = new Header(MessageType.WindowUpdate, MessageFlag.RST, streamId, 0);
            await _sendFrameFireAndForget(header, ReadOnlyMemory<byte>.Empty, cancellationToken);
            return;
        }

        YamuxStream stream;
        try
        {
            stream = _registry.CreateInbound(streamId);
        }
        catch (YamuxException)
        {
            Header header = new Header(MessageType.GoAway, MessageFlag.None, 0, (uint)GoAwayCode.ProtocolError);
            await _sendFrameFireAndForget(header, ReadOnlyMemory<byte>.Empty, cancellationToken);
            throw;
        }

        if (!_registry.TryEnqueueAccepted(stream))
        {
            _registry.Remove(streamId);
            Header header = new Header(MessageType.WindowUpdate, MessageFlag.RST, streamId, 0);
            await _sendFrameFireAndForget(header, ReadOnlyMemory<byte>.Empty, cancellationToken);
        }
    }
}
