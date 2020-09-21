using System.Buffers;
using System.IO;

namespace Omnius.Core.Extensions
{
    public static class ReadOnlySequenceExtensions
    {
        public static void CopyTo(this ReadOnlySequence<byte> sequence, Stream stream)
        {
            foreach (var memory in sequence)
            {
                stream.Write(memory.Span);
            }
        }
    }
}
