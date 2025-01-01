using Omnius.Core.Base;

namespace Omnius.Core.Omnikit.Connections.Codec;

public class FramedSender : AsyncDisposableBase
{
    private readonly Stream _stream;
    private readonly int _maxFrameLength;
    private readonly IBytesPool _bytesPool;

    private const int HeaderSize = 4;

    public FramedSender(Stream stream, int maxFrameLength, IBytesPool bytesPool)
    {
        _stream = stream;
        _maxFrameLength = maxFrameLength;
        _bytesPool = bytesPool;
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await _stream.FlushAsync();
        await _stream.DisposeAsync();
    }

    public async ValueTask SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        if (data.Length > _maxFrameLength) throw new InvalidDataException("The frame size is too large.");

        var headerBuffer = new byte[HeaderSize];
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(headerBuffer, (uint)data.Length);

        await _stream.WriteAsync(headerBuffer, cancellationToken);
        await _stream.WriteAsync(data, cancellationToken);
        await _stream.FlushAsync(cancellationToken);
    }
}
