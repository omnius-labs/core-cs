using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;

public sealed class DuplexStream : Stream
{
    private readonly PipeReader _reader;
    private readonly PipeWriter _writer;
    private int _disposeState;

    public static (DuplexStream first, DuplexStream second) CreatePair(int maxBufferSize = 64 * 1024)
    {
        var options = new PipeOptions(
            pauseWriterThreshold: maxBufferSize,
            resumeWriterThreshold: maxBufferSize / 2,
            useSynchronizationContext: false);
        var x = new Pipe(options);
        var y = new Pipe(options);
        return (new DuplexStream(x.Reader, y.Writer), new DuplexStream(y.Reader, x.Writer));
    }

    private DuplexStream(PipeReader reader, PipeWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    private bool IsDisposed => (Volatile.Read(ref _disposeState) == 1);

    private void ThrowIfDisposed()
    {
        if (this.IsDisposed) throw new ObjectDisposedException(nameof(DuplexStream));
    }

    private bool TryBeginDispose()
    {
        return Interlocked.Exchange(ref _disposeState, 1) == 0;
    }

    protected override void Dispose(bool disposing)
    {
        if (this.TryBeginDispose())
        {
            _writer.Complete();
            _reader.Complete();
        }

        base.Dispose(disposing);
    }

    public override async ValueTask DisposeAsync()
    {
        if (this.TryBeginDispose())
        {
            await _writer.CompleteAsync().ConfigureAwait(false);
            await _reader.CompleteAsync().ConfigureAwait(false);
        }

        await base.DisposeAsync().ConfigureAwait(false);
    }

    public override bool CanRead => !this.IsDisposed;
    public override bool CanWrite => !this.IsDisposed;
    public override bool CanSeek => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return this.ReadAsync(buffer.AsMemory(offset, count)).AsTask().GetAwaiter().GetResult();
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken = default)
    {
        this.ThrowIfDisposed();

        if (destination.Length == 0) return 0;

        while (true)
        {
            var result = await _reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            var source = result.Buffer;
            var length = (int)Math.Min(source.Length, destination.Length);

            Debug.WriteLine("ReadAsync: {0}", length);

            if (length > 0)
            {
                var slice = source.Slice(0, length);
                slice.CopyTo(destination.Span);
                _reader.AdvanceTo(slice.End);
                return length;
            }

            if (result.IsCompleted)
            {
                return 0;
            }
        }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        this.WriteAsync(buffer.AsMemory(offset, count)).AsTask().GetAwaiter().GetResult();
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default)
    {
        this.ThrowIfDisposed();
        await _writer.WriteAsync(source, cancellationToken).ConfigureAwait(false);
        await this.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public override void Flush()
    {
        this.FlushAsync().GetAwaiter().GetResult();
    }

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        this.ThrowIfDisposed();

        var flush = await _writer.FlushAsync(cancellationToken).ConfigureAwait(false);

        if (flush.IsCanceled)
        {
            throw new OperationCanceledException(cancellationToken);
        }
        else if (flush.IsCompleted)
        {
            throw new IOException("stream is completed");
        }
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
}
