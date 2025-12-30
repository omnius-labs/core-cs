namespace Omnius.Yamux.Internal;

internal sealed class FrameWriter
{
    private readonly Stream _stream;

    public FrameWriter(Stream stream)
    {
        _stream = stream;
    }

    public async ValueTask WriteAsync(byte[] header, byte[] payload, CancellationToken cancellationToken)
    {
        await _stream.WriteAsync(header, cancellationToken);

        if (payload.Length > 0)
        {
            await _stream.WriteAsync(payload, cancellationToken);
        }

        await _stream.FlushAsync(cancellationToken);
    }
}
