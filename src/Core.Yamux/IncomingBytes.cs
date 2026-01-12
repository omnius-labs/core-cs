using System.Threading;
using System.Threading.Channels;
using Omnius.Core.Base;

namespace Omnius.Yamux;

internal sealed class IncomingBytes
{
    private readonly Channel<OwnedBytes> _channel;

    private OwnedBytes _current = OwnedBytes.Empty;
    private int _currentOffset;

    private readonly AsyncLock _asyncLock = new();

    public IncomingBytes()
    {
        _channel = Channel.CreateUnbounded<OwnedBytes>(
            new UnboundedChannelOptions
            {
                SingleWriter = false,
                SingleReader = false,
                AllowSynchronousContinuations = true
            });
    }

    public bool TryWrite(OwnedBytes data)
    {
        if (!_channel.Writer.TryWrite(data))
        {
            data.Dispose();
            return false;
        }

        return true;
    }

    public void Complete()
    {
        _channel.Writer.TryComplete();
    }

    public void CompleteAndDrain()
    {
        this.Complete();
        this.Drain();
    }

    public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        if (buffer.Length == 0) return 0;

        for (; ; )
        {
            using (await _asyncLock.LockAsync(cancellationToken))
            {
                OwnedBytes current;
                int offset;
                {
                    current = _current;
                    offset = _currentOffset;
                }

                if (current.Memory.Length > offset)
                {
                    var available = current.Memory.Slice(offset);
                    var toCopy = Math.Min(available.Length, buffer.Length);
                    available.Slice(0, toCopy).CopyTo(buffer);
                    bool shouldDispose = false;

                    _currentOffset += toCopy;
                    if (_currentOffset >= current.Memory.Length)
                    {
                        _current = OwnedBytes.Empty;
                        _currentOffset = 0;
                        shouldDispose = true;
                    }

                    if (shouldDispose)
                    {
                        current.Dispose();
                    }

                    return toCopy;
                }

                if (_channel.Reader.TryRead(out var next))
                {
                    _current = next;
                    _currentOffset = 0;

                    continue;
                }
            }

            if (!await _channel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
            {
                return 0;
            }
        }
    }

    private void Drain()
    {
        using (_asyncLock.Lock())
        {
            _current.Dispose();
            _current = OwnedBytes.Empty;
            _currentOffset = 0;

            while (_channel.Reader.TryRead(out var next))
            {
                next.Dispose();
            }
        }
    }
}
