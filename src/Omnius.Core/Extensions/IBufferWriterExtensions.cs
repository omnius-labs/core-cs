using System.Buffers;
using System.IO;

namespace Omnius.Core.Extensions
{
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

        public static void Write(this IBufferWriter<byte> bufferWriter, Stream stream)
        {
            long remain = stream.Length - stream.Position;
            while (remain > 0)
            {
                var span = bufferWriter.GetSpan();
                int readLength = stream.Read(span);
                bufferWriter.Advance(readLength);
            }
        }
    }
}
