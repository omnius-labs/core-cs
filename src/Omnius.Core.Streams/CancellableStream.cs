namespace Omnius.Core.Streams;

public class CancellableStream : Stream
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly Stream _stream;
    private readonly CancellationToken _cancellationToken;

    private bool _disposed;

    public CancellableStream(Stream stream, CancellationToken cancellationToken)
    {
        _stream = stream;
        _cancellationToken = cancellationToken;
    }

    public override bool CanRead
    {
        get
        {
            _cancellationToken.ThrowIfCancellationRequested();
            return _stream.CanRead;
        }
    }

    public override bool CanWrite
    {
        get
        {
            _cancellationToken.ThrowIfCancellationRequested();
            return _stream.CanWrite;
        }
    }

    public override bool CanSeek
    {
        get
        {
            _cancellationToken.ThrowIfCancellationRequested();
            return _stream.CanSeek;
        }
    }

    public override long Position
    {
        get
        {
            _cancellationToken.ThrowIfCancellationRequested();
            return _stream.Position;
        }

        set
        {
            _cancellationToken.ThrowIfCancellationRequested();
            _stream.Position = value;
        }
    }

    public override long Length
    {
        get
        {
            _cancellationToken.ThrowIfCancellationRequested();
            return _stream.Length;
        }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        _cancellationToken.ThrowIfCancellationRequested();
        return _stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _cancellationToken.ThrowIfCancellationRequested();
        _stream.SetLength(value);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        _cancellationToken.ThrowIfCancellationRequested();
        return _stream.Read(buffer, offset, count);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _cancellationToken.ThrowIfCancellationRequested();
        _stream.Write(buffer, offset, count);
    }

    public override void Flush()
    {
        _cancellationToken.ThrowIfCancellationRequested();
        _stream.Flush();
    }

    protected override void Dispose(bool disposing)
    {
        try
        {
            if (_disposed) return;
            _disposed = true;

            _stream.Dispose();
        }
        finally
        {
            base.Dispose(disposing);
        }
    }
}
