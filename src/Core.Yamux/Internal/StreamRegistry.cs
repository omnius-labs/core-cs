using System.Collections.Concurrent;
using System.Threading.Channels;
using Omnius.Core.Base;
using Omnius.Yamux;

namespace Omnius.Yamux.Internal;

internal sealed class StreamRegistry
{
    private readonly ConcurrentDictionary<uint, YamuxStream> _streams = new();
    private readonly Channel<YamuxStream>? _acceptedStreams;
    private readonly AsyncLock _connectLock = new();
    private readonly Func<uint, YamuxStreamState, YamuxStream> _streamFactory;
    private uint _nextStreamId;

    public StreamRegistry(YamuxSessionType sessionType, Func<uint, YamuxStreamState, YamuxStream> streamFactory, YamuxOptions options)
    {
        _streamFactory = streamFactory;

        _nextStreamId = sessionType == YamuxSessionType.Client ? 1u : 2u;

        if (options.AcceptBacklog > 0)
        {
            _acceptedStreams = Channel.CreateBounded<YamuxStream>(options.AcceptBacklog);
        }
    }

    public int Count => _streams.Count;

    public IEnumerable<YamuxStream> Streams => _streams.Values;

    public bool AcceptEnabled => _acceptedStreams != null;

    public async ValueTask<YamuxStream> CreateOutboundAsync(CancellationToken cancellationToken)
    {
        using (await _connectLock.LockAsync(cancellationToken))
        {
            if (_nextStreamId >= uint.MaxValue - 1) throw new YamuxException(YamuxErrorCode.StreamsExhausted);
            uint streamId = _nextStreamId;
            _nextStreamId += 2;

            YamuxStream stream = _streamFactory(streamId, YamuxStreamState.Init);
            if (!_streams.TryAdd(streamId, stream)) throw new YamuxException(YamuxErrorCode.DuplicateStreamId);

            return stream;
        }
    }

    public YamuxStream CreateInbound(uint streamId)
    {
        YamuxStream stream = _streamFactory(streamId, YamuxStreamState.SYNReceived);
        if (!_streams.TryAdd(streamId, stream)) throw new YamuxException(YamuxErrorCode.DuplicateStreamId);
        return stream;
    }

    public bool TryGet(uint streamId, out YamuxStream stream)
    {
        if (_streams.TryGetValue(streamId, out YamuxStream? value))
        {
            stream = value;
            return true;
        }

        stream = default!;
        return false;
    }

    public bool TryEnqueueAccepted(YamuxStream stream)
    {
        return _acceptedStreams?.Writer.TryWrite(stream) ?? false;
    }

    public async ValueTask<YamuxStream> AcceptAsync(CancellationToken cancellationToken)
    {
        if (_acceptedStreams == null) throw new InvalidOperationException("AcceptStreamAsync is disabled because AcceptBacklog <= 0.");

        try
        {
            return await _acceptedStreams.Reader.ReadAsync(cancellationToken);
        }
        catch (ChannelClosedException)
        {
            throw new YamuxException(YamuxErrorCode.ConnectionShutdown);
        }
    }

    public void CompleteAccepting()
    {
        _acceptedStreams?.Writer.Complete();
    }

    public void Remove(uint streamId)
    {
        _streams.TryRemove(streamId, out _);
    }
}
