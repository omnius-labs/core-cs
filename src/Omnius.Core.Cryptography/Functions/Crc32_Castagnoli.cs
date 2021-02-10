using System;
using System.Buffers;
using System.Text;
using System.Threading;

namespace Omnius.Core.Cryptography.Functions
{
    public static unsafe class Crc32_Castagnoli
    {
        private static readonly ThreadLocal<Encoding> _utf8Encoding = new(() => new UTF8Encoding(false));
        private static readonly uint[] _table;

        static Crc32_Castagnoli()
        {
            // uint poly = 0xedb88320;
            uint poly = 0x82F63B78;
            _table = new uint[256];

            for (uint i = 0; i < 256; i++)
            {
                uint x = i;

                for (int j = 0; j < 8; j++)
                {
                    if ((x & 1) != 0)
                    {
                        x = (x >> 1) ^ poly;
                    }
                    else
                    {
                        x >>= 1;
                    }
                }

                _table[i] = x;
            }
        }

        private static uint InternalCompute(uint x, byte* src, int len)
        {
            fixed (uint* p_table = _table)
            {
                var t_src = src;

                for (int i = 0; i < len; i++)
                {
                    x = (x >> 8) ^ p_table[(x & 0xff) ^ *t_src++];
                }

                return x;
            }
        }

        public static int ComputeHash(ReadOnlySpan<byte> memory)
        {
            uint x = 0xFFFFFFFF;

            fixed (byte* p_buffer = memory)
            {
                var t_buffer = p_buffer;

                x = InternalCompute(x, t_buffer, memory.Length);
            }

            return (int)(x ^ 0xFFFFFFFF);
        }

        public static int ComputeHash(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

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
                    x = InternalCompute(x, p_segment, segment.Length);
                }
            }

            return (int)(x ^ 0xFFFFFFFF);
        }
    }
}
