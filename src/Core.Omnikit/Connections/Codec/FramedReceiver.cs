using System.Buffers;
using Omnius.Core.Base;

namespace Omnius.Core.Omnikit.Connections.Codec;

public class FramedReceiver
{
    private readonly Stream _stream;
    private readonly int _maxFrameLength;
    private readonly IBytesPool _bytesPool;

    private const int HeaderSize = 4;

    public FramedReceiver(Stream stream, int maxFrameLength, IBytesPool bytesPool)
    {
        _stream = stream;
        _maxFrameLength = maxFrameLength;
        _bytesPool = bytesPool;
    }

    public async ValueTask<IMemoryOwner<byte>> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        var headerBuffer = new byte[HeaderSize];
        int remain = HeaderSize;

        while (remain > 0)
        {
            var readSize = await _stream.ReadAsync(headerBuffer.AsMemory(headerBuffer.Length - remain, remain), cancellationToken);
            if (readSize == 0) throw new EndOfStreamException();
            remain -= readSize;
        }

        var bodySize = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(headerBuffer.AsSpan());
        if (bodySize > _maxFrameLength) throw new InvalidDataException("The frame size is too large.");

        var bodyBuffer = _bytesPool.Memory.Rent((int)bodySize).Shrink((int)bodySize);

        remain = (int)bodySize;
        while (remain > 0)
        {
            var readSize = await _stream.ReadAsync(bodyBuffer.Memory.Slice(bodyBuffer.Memory.Length - remain, remain), cancellationToken);
            if (readSize == 0) throw new EndOfStreamException();
            remain -= readSize;
        }

        return bodyBuffer;
    }
}
