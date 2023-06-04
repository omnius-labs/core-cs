using System.Buffers;

namespace Omnius.Core;

public static class IBufferWriterExtensions
{
    public static void Write<T>(this IBufferWriter<T> bufferWriter, ReadOnlySequence<T> sequence)
    {
        var position = sequence.Start;

        while (sequence.TryGet(ref position, out var memory, true))
        {
            if (memory.Length == 0) break;

            bufferWriter.Write(memory.Span);
        }
    }

    public static async Task WriteAsync(this IBufferWriter<byte> bufferWriter, Stream stream, CancellationToken cancellationToken = default)
    {
        long remain = stream.Length - stream.Position;
        while (remain > 0)
        {
            var memory = bufferWriter.GetMemory();
            int readLength = await stream.ReadAsync(memory, cancellationToken);
            bufferWriter.Advance(readLength);
        }
    }
}
