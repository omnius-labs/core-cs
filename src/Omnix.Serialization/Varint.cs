using System;
using System.Buffers;
using System.IO;
using System.Threading;

namespace Omnix.Serialization
{
    // https://developers.google.com/protocol-buffers/docs/encoding

    /// <summary>
    /// <see cref="long"/>と<see cref="byte"/>[]の変換機能を提供します。
    /// </summary>
    public unsafe static class Varint
    {
        public static bool IsEnd(byte value)
        {
            return ((value & 0x80) != 0x80);
        }

        public static int ComputeSize(long value)
        {
            return ComputeSize((ulong)((value << 1) ^ (value >> 63)));
        }

        public static int ComputeSize(ulong value)
        {
            if (value <= 0x7F)
            {
                return 1;
            }
            else if (value <= 0x3FFF)
            {
                return 2;
            }
            else if (value <= 0x1FFFFF)
            {
                return 3;
            }
            else if (value <= 0xFFFFFFF)
            {
                return 4;
            }
            else if (value <= 0x7FFFFFFFF)
            {
                return 5;
            }
            else if (value <= 0x3FFFFFFFFFF)
            {
                return 6;
            }
            else if (value <= 0x1FFFFFFFFFFFF)
            {
                return 7;
            }
            else if (value <= 0xFFFFFFFFFFFFFF)
            {
                return 8;
            }
            else if (value <= 0x7FFFFFFFFFFFFFFF)
            {
                return 9;
            }
            else
            {
                return 10;
            }
        }

        public static void SetInt64(long value, IBufferWriter<byte> writer)
        {
            SetUInt64((ulong)((value << 1) ^ (value >> 63)), writer);
        }

        public static void SetUInt64(ulong value, IBufferWriter<byte> writer)
        {
            if (value <= 0x7F)
            {
                var buffer = writer.GetSpan(1);
                buffer[0] = (byte)value;

                writer.Advance(1);
            }
            else if (value <= 0x3FFF)
            {
                var buffer = writer.GetSpan(2);

                fixed (byte* p = buffer)
                {
                    p[0] = (byte)((value >> 8 - 1) & 0x7F | 0x80);
                    p[1] = (byte)((value >> 0 - 0) & 0x7F);
                }

                writer.Advance(2);
            }
            else if (value <= 0x1FFFFF)
            {
                var buffer = writer.GetSpan(3);

                fixed (byte* p = buffer)
                {
                    p[0] = (byte)((value >> 16 - 2) & 0x7F | 0x80);
                    p[1] = (byte)((value >> 8 - 1) & 0x7F | 0x80);
                    p[2] = (byte)((value >> 0 - 0) & 0x7F);
                }

                writer.Advance(3);
            }
            else if (value <= 0xFFFFFFF)
            {
                var buffer = writer.GetSpan(4);

                fixed (byte* p = buffer)
                {
                    p[0] = (byte)((value >> 24 - 3) & 0x7F | 0x80);
                    p[1] = (byte)((value >> 16 - 2) & 0x7F | 0x80);
                    p[2] = (byte)((value >> 8 - 1) & 0x7F | 0x80);
                    p[3] = (byte)((value >> 0 - 0) & 0x7F);
                }

                writer.Advance(4);
            }
            else if (value <= 0x7FFFFFFFF)
            {
                var buffer = writer.GetSpan(5);

                fixed (byte* p = buffer)
                {
                    p[0] = (byte)((value >> 32 - 4) & 0x7F | 0x80);
                    p[1] = (byte)((value >> 24 - 3) & 0x7F | 0x80);
                    p[2] = (byte)((value >> 16 - 2) & 0x7F | 0x80);
                    p[3] = (byte)((value >> 8 - 1) & 0x7F | 0x80);
                    p[4] = (byte)((value >> 0 - 0) & 0x7F);
                }

                writer.Advance(5);
            }
            else if (value <= 0x3FFFFFFFFFF)
            {
                var buffer = writer.GetSpan(6);

                fixed (byte* p = buffer)
                {
                    p[0] = (byte)((value >> 40 - 5) & 0x7F | 0x80);
                    p[1] = (byte)((value >> 32 - 4) & 0x7F | 0x80);
                    p[2] = (byte)((value >> 24 - 3) & 0x7F | 0x80);
                    p[3] = (byte)((value >> 16 - 2) & 0x7F | 0x80);
                    p[4] = (byte)((value >> 8 - 1) & 0x7F | 0x80);
                    p[5] = (byte)((value >> 0 - 0) & 0x7F);
                }

                writer.Advance(6);
            }
            else if (value <= 0x1FFFFFFFFFFFF)
            {
                var buffer = writer.GetSpan(7);

                fixed (byte* p = buffer)
                {
                    p[0] = (byte)((value >> 48 - 6) & 0x7F | 0x80);
                    p[1] = (byte)((value >> 40 - 5) & 0x7F | 0x80);
                    p[2] = (byte)((value >> 32 - 4) & 0x7F | 0x80);
                    p[3] = (byte)((value >> 24 - 3) & 0x7F | 0x80);
                    p[4] = (byte)((value >> 16 - 2) & 0x7F | 0x80);
                    p[5] = (byte)((value >> 8 - 1) & 0x7F | 0x80);
                    p[6] = (byte)((value >> 0 - 0) & 0x7F);
                }

                writer.Advance(7);
            }
            else if (value <= 0xFFFFFFFFFFFFFF)
            {
                var buffer = writer.GetSpan(8);

                fixed (byte* p = buffer)
                {
                    p[0] = (byte)((value >> 56 - 7) & 0x7F | 0x80);
                    p[1] = (byte)((value >> 48 - 6) & 0x7F | 0x80);
                    p[2] = (byte)((value >> 40 - 5) & 0x7F | 0x80);
                    p[3] = (byte)((value >> 32 - 4) & 0x7F | 0x80);
                    p[4] = (byte)((value >> 24 - 3) & 0x7F | 0x80);
                    p[5] = (byte)((value >> 16 - 2) & 0x7F | 0x80);
                    p[6] = (byte)((value >> 8 - 1) & 0x7F | 0x80);
                    p[7] = (byte)((value >> 0 - 0) & 0x7F);
                }

                writer.Advance(8);
            }
            else if (value <= 0x7FFFFFFFFFFFFFFF)
            {
                var buffer = writer.GetSpan(9);

                fixed (byte* p = buffer)
                {
                    p[0] = (byte)((value >> 64 - 8) & 0x7F | 0x80);
                    p[1] = (byte)((value >> 56 - 7) & 0x7F | 0x80);
                    p[2] = (byte)((value >> 48 - 6) & 0x7F | 0x80);
                    p[3] = (byte)((value >> 40 - 5) & 0x7F | 0x80);
                    p[4] = (byte)((value >> 32 - 4) & 0x7F | 0x80);
                    p[5] = (byte)((value >> 24 - 3) & 0x7F | 0x80);
                    p[6] = (byte)((value >> 16 - 2) & 0x7F | 0x80);
                    p[7] = (byte)((value >> 8 - 1) & 0x7F | 0x80);
                    p[8] = (byte)((value >> 0 - 0) & 0x7F);
                }

                writer.Advance(9);
            }
            else
            {
                var buffer = writer.GetSpan(10);

                fixed (byte* p = buffer)
                {
                    p[0] = (byte)((value >> 72 - 9) & 0x7F | 0x80);
                    p[1] = (byte)((value >> 64 - 8) & 0x7F | 0x80);
                    p[2] = (byte)((value >> 56 - 7) & 0x7F | 0x80);
                    p[3] = (byte)((value >> 48 - 6) & 0x7F | 0x80);
                    p[4] = (byte)((value >> 40 - 5) & 0x7F | 0x80);
                    p[5] = (byte)((value >> 32 - 4) & 0x7F | 0x80);
                    p[6] = (byte)((value >> 24 - 3) & 0x7F | 0x80);
                    p[7] = (byte)((value >> 16 - 2) & 0x7F | 0x80);
                    p[8] = (byte)((value >> 8 - 1) & 0x7F | 0x80);
                    p[9] = (byte)((value >> 0 - 0) & 0x7F);
                }

                writer.Advance(10);
            }
        }

        public static bool TryGetInt64(ReadOnlySequence<byte> sequence, out long result, out SequencePosition consumed)
        {
            result = 0;
            consumed = sequence.Start;

            if (!TryGetUInt64(sequence, out ulong ulong_result, out consumed)) return false;

            result = (long)(ulong_result >> 1) ^ (-((long)ulong_result & 1));
            return true;
        }

        public static bool TryGetUInt64(ReadOnlySequence<byte> sequence, out ulong result, out SequencePosition consumed)
        {
            result = 0;
            consumed = sequence.Start;

            var position = sequence.Start;
            int count = 0;

            while (sequence.TryGet(ref position, out var memory))
            {
                for (int i = 0; i < memory.Length; i++)
                {
                    var b = memory.Span[i];

                    result = (result << 7) | (byte)(b & 0x7F);
                    count++;

                    if ((b & 0x80) != 0x80) break;
                    if (count > 10) return false;
                }
            }

            consumed = sequence.GetPosition(count);

            return true;
        }
    }
}
