using System;
using System.Buffers;
using System.IO;

namespace Omnius.Core
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

        public static Memory<byte> ToMemory(this ReadOnlySequence<byte> sequence, IBytesPool bytesPool)
        {
            var bytes = new byte[sequence.Length];
            sequence.CopyTo(bytes.AsSpan());
            return bytes;
        }

        public static IMemoryOwner<byte> ToMemoryOwner(this ReadOnlySequence<byte> sequence, IBytesPool bytesPool)
        {
            var memoryOwner = bytesPool.Memory.Rent((int)sequence.Length);
            sequence.CopyTo(memoryOwner.Memory.Span);
            return memoryOwner;
        }
    }
}
