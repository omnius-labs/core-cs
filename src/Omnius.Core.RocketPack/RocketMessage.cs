using System.Buffers;
using Omnius.Core.Pipelines;

namespace Omnius.Core.RocketPack;

public static class RocketMessage
{
    public static T FromStream<T>(Stream inStream)
        where T : IRocketMessage<T>
    {
        using var bytesPipe = new BytesPipe();

        const int bufferSize = 4096;

        while (inStream.Position < inStream.Length)
        {
            var readLength = inStream.Read(bytesPipe.Writer.GetSpan(bufferSize));
            if (readLength < 0) break;

            bytesPipe.Writer.Advance(readLength);
        }

        return IRocketMessage<T>.Import(bytesPipe.Reader.GetSequence(), BytesPool.Shared);
    }

    public static void ToStream<T>(T message, Stream stream)
        where T : IRocketMessage<T>
    {
        using var bytesPipe = new BytesPipe();

        message.Export(bytesPipe.Writer, BytesPool.Shared);

        var sequence = bytesPipe.Reader.GetSequence();
        var position = sequence.Start;

        while (sequence.TryGet(ref position, out var memory))
        {
            stream.Write(memory.Span);
        }
    }

    public static T FromBytes<T>(ReadOnlyMemory<byte> memory)
        where T : IRocketMessage<T>
    {
        return IRocketMessage<T>.Import(new ReadOnlySequence<byte>(memory), BytesPool.Shared);
    }

    public static IMemoryOwner<byte> ToBytes<T>(T message)
        where T : IRocketMessage<T>
    {
        using var hub = new BytesPipe();
        message.Export(hub.Writer, BytesPool.Shared);

        var sequence = hub.Reader.GetSequence();
        var memoryOwner = BytesPool.Shared.Memory.Rent((int)sequence.Length).Shrink((int)sequence.Length);
        sequence.CopyTo(memoryOwner.Memory.Span);

        return memoryOwner;
    }
}
