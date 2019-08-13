using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Omnix.Algorithms.Cryptography.Internal;
using Omnix.Algorithms.Internal;
using Omnix.Base;

namespace Omnix.Algorithms.Cryptography
{
    public static unsafe class Crc32_Castagnoli
    {
        private static readonly ThreadLocal<Encoding> _utf8Encoding = new ThreadLocal<Encoding>(() => new UTF8Encoding(false));

        public static int ComputeHash(ReadOnlySpan<byte> memory)
        {
            uint x = 0xFFFFFFFF;

            fixed (byte* p_buffer = memory)
            {
                var t_buffer = p_buffer;

                x = NativeMethods.Crc32_Castagnoli.Compute(x, t_buffer, memory.Length);
            }

            return (int)(x ^ 0xFFFFFFFF);
        }

        public static int ComputeHash(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (var recyclableMemory = MemoryPool<byte>.Shared.Rent(_utf8Encoding.Value!.GetMaxByteCount(value.Length)))
            {
                var length = _utf8Encoding.Value!.GetBytes(value, recyclableMemory.Memory.Span);

                return ComputeHash(recyclableMemory.Memory.Span.Slice(0, length));
            }
        }

        public static int ComputeHash(ReadOnlySequence<byte> sequence)
        {
            uint x = 0xFFFFFFFF;

            foreach (var segment in sequence)
            {
                fixed (byte* p_segment = segment.Span)
                {
                    x = NativeMethods.Crc32_Castagnoli.Compute(x, p_segment, segment.Length);
                }
            }

            return (int)(x ^ 0xFFFFFFFF);
        }
    }
}
