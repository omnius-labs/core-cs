using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Omnix.Serialization.RocketPack
{
    // https://developers.google.com/protocol-buffers/docs/encoding

    /// <summary>
    /// <see cref="long"/>と<see cref="byte"/>[]の変換機能を提供します。
    /// </summary>
    public static unsafe class Varint
    {
        // bit count | first byte (in binary) | first byte (in hex)
        // --------- | ---------------------- | -------------------
        // 7 bit     | 0xxx xxxx              | 0x00 - 0x7F
        // 8 bit     | 1000 0000              | 0x80
        // 16 bit    | 1000 0001              | 0x81
        // 32 bit    | 1000 0010              | 0x82
        // 64 bit    | 1000 0011              | 0x83

        public const byte MinInt7 = 0x00; // 0
        public const byte MaxInt7 = 0x7F; // 127

        private const byte Int8Code = 0x80;
        private const byte Int16Code = 0x81;
        private const byte Int32Code = 0x82;
        private const byte Int64Code = 0x83;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetUInt8(byte value, IBufferWriter<byte> writer)
        {
            unchecked
            {
                if (value <= MaxInt7)
                {
                    var span = writer.GetSpan(1);
                    span[0] = value;

                    writer.Advance(1);
                }
                else
                {
                    var span = writer.GetSpan(2);
                    span[0] = Int8Code;
                    span[1] = value;

                    writer.Advance(2);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetUInt16(ushort value, IBufferWriter<byte> writer)
        {
            unchecked
            {
                if (value <= MaxInt7)
                {
                    var span = writer.GetSpan(1);
                    span[0] = (byte)value;

                    writer.Advance(1);
                }
                else if (value <= byte.MaxValue)
                {
                    var span = writer.GetSpan(2);
                    span[0] = Int8Code;
                    span[1] = (byte)value;

                    writer.Advance(2);
                }
                else
                {
                    var span = writer.GetSpan(3);

                    fixed (byte* p = span)
                    {
                        p[0] = Int16Code;

                        ushort* uint_p = (ushort*)(p + 1);
                        *uint_p = value;
                    }

                    writer.Advance(3);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetUInt32(uint value, IBufferWriter<byte> writer)
        {
            unchecked
            {
                if (value <= MaxInt7)
                {
                    var span = writer.GetSpan(1);
                    span[0] = (byte)value;

                    writer.Advance(1);
                }
                else if (value <= byte.MaxValue)
                {
                    var span = writer.GetSpan(2);
                    span[0] = Int8Code;
                    span[1] = (byte)value;

                    writer.Advance(2);
                }
                else if (value <= ushort.MaxValue)
                {
                    var span = writer.GetSpan(3);

                    fixed (byte* p = span)
                    {
                        p[0] = Int16Code;

                        ushort* uint_p = (ushort*)(p + 1);
                        *uint_p = (ushort)value;
                    }

                    writer.Advance(3);
                }
                else
                {
                    var span = writer.GetSpan(5);

                    fixed (byte* p = span)
                    {
                        span[0] = Int32Code;

                        uint* uint_p = (uint*)(p + 1);
                        *uint_p = value;
                    }

                    writer.Advance(5);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetUInt64(ulong value, IBufferWriter<byte> writer)
        {
            unchecked
            {
                if (value <= MaxInt7)
                {
                    var span = writer.GetSpan(1);
                    span[0] = (byte)value;

                    writer.Advance(1);
                }
                else if (value <= byte.MaxValue)
                {
                    var span = writer.GetSpan(2);
                    span[0] = Int8Code;
                    span[1] = (byte)value;

                    writer.Advance(2);
                }
                else if (value <= ushort.MaxValue)
                {
                    var span = writer.GetSpan(3);

                    fixed (byte* p = span)
                    {
                        span[0] = Int16Code;

                        ushort* uint_p = (ushort*)(p + 1);
                        *uint_p = (ushort)value;
                    }

                    writer.Advance(3);
                }
                else if (value <= uint.MaxValue)
                {
                    var span = writer.GetSpan(5);

                    fixed (byte* p = span)
                    {
                        span[0] = Int32Code;

                        uint* uint_p = (uint*)(p + 1);
                        *uint_p = (uint)value;
                    }

                    writer.Advance(5);
                }
                else
                {
                    var span = writer.GetSpan(5);

                    fixed (byte* p = span)
                    {
                        span[0] = Int64Code;

                        ulong* ulong_p = (ulong*)(p + 1);
                        *ulong_p = value;
                    }

                    writer.Advance(9);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetInt8(sbyte value, IBufferWriter<byte> writer)
        {
            unchecked
            {
                SetUInt8((byte)((value << 1) ^ (value >> 7)), writer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetInt16(short value, IBufferWriter<byte> writer)
        {
            unchecked
            {
                SetUInt16((ushort)((value << 1) ^ (value >> 15)), writer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetInt32(int value, IBufferWriter<byte> writer)
        {
            unchecked
            {
                SetUInt32((uint)((value << 1) ^ (value >> 31)), writer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetInt64(long value, IBufferWriter<byte> writer)
        {
            unchecked
            {
                SetUInt64((ulong)((value << 1) ^ (value >> 63)), writer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InternalTryGetUInt8(ReadOnlySpan<byte> span, out byte value, out int consumed)
        {
            unchecked
            {
                fixed (byte* fixed_p = span)
                {
                    var p = fixed_p;

                    if (MinInt7 <= *p && *p <= MaxInt7)
                    {
                        value = *p;
                        consumed = 1;
                        return true;
                    }
                    else
                    {
                        switch (*p)
                        {
                            case Int8Code:
                                value = p[1];
                                consumed = 2;
                                return true;
                            default:
                                value = 0;
                                consumed = 0;
                                return false;
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InternalTryGetUInt16(ReadOnlySpan<byte> span, out ushort value, out int consumed)
        {
            unchecked
            {
                fixed (byte* fixed_p = span)
                {
                    var p = fixed_p;

                    if (MinInt7 <= *p && *p <= MaxInt7)
                    {
                        value = *p;
                        consumed = 1;
                        return true;
                    }
                    else
                    {
                        switch (*p)
                        {
                            case Int8Code:
                                value = p[1];
                                consumed = 2;
                                return true;
                            case Int16Code:
                                value = *(ushort*)(p + 1);
                                consumed = 3;
                                return true;
                            default:
                                value = 0;
                                consumed = 0;
                                return false;
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InternalTryGetUInt32(ReadOnlySpan<byte> span, out uint value, out int consumed)
        {
            unchecked
            {
                fixed (byte* fixed_p = span)
                {
                    var p = fixed_p;

                    if (MinInt7 <= *p && *p <= MaxInt7)
                    {
                        value = *p;
                        consumed = 1;
                        return true;
                    }
                    else
                    {
                        switch (*p)
                        {
                            case Int8Code:
                                value = p[1];
                                consumed = 2;
                                return true;
                            case Int16Code:
                                value = *(ushort*)(p + 1);
                                consumed = 3;
                                return true;
                            case Int32Code:
                                value = *(uint*)(p + 1);
                                consumed = 5;
                                return true;
                            default:
                                value = 0;
                                consumed = 0;
                                return false;
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InternalTryGetUInt64(ReadOnlySpan<byte> span, out ulong value, out int consumed)
        {
            unchecked
            {
                fixed (byte* fixed_p = span)
                {
                    var p = fixed_p;

                    if (MinInt7 <= *p && *p <= MaxInt7)
                    {
                        value = *p;
                        consumed = 1;
                        return true;
                    }
                    else
                    {
                        switch (*p)
                        {
                            case Int8Code:
                                value = p[1];
                                consumed = 2;
                                return true;
                            case Int16Code:
                                value = *(ushort*)(p + 1);
                                consumed = 3;
                                return true;
                            case Int32Code:
                                value = *(uint*)(p + 1);
                                consumed = 5;
                                return true;
                            case Int64Code:
                                value = *(ulong*)(p + 1);
                                consumed = 9;
                                return true;
                            default:
                                value = 0;
                                consumed = 0;
                                return false;
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetUInt8(ReadOnlySequence<byte> sequence, out byte value, out SequencePosition consumed)
        {
            unchecked
            {

                if (sequence.First.Length >= 2)
                {
                    bool flag = InternalTryGetUInt8(sequence.First.Span, out value, out int int_consumed);
                    consumed = sequence.GetPosition(int_consumed);

                    return flag;
                }
                else
                {
                    Span<byte> span = stackalloc byte[(int)Math.Min(2, sequence.Length)];
                    sequence.CopyTo(span);

                    bool flag = InternalTryGetUInt8(span, out value, out int int_consumed);
                    consumed = sequence.GetPosition(int_consumed);

                    return flag;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetUInt16(ReadOnlySequence<byte> sequence, out ushort value, out SequencePosition consumed)
        {
            unchecked
            {

                if (sequence.First.Length >= 3)
                {
                    bool flag = InternalTryGetUInt16(sequence.First.Span, out value, out int int_consumed);
                    consumed = sequence.GetPosition(int_consumed);

                    return flag;
                }
                else
                {
                    Span<byte> span = stackalloc byte[(int)Math.Min(3, sequence.Length)];
                    sequence.CopyTo(span);

                    bool flag = InternalTryGetUInt16(span, out value, out int int_consumed);
                    consumed = sequence.GetPosition(int_consumed);

                    return flag;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetUInt32(ReadOnlySequence<byte> sequence, out uint value, out SequencePosition consumed)
        {
            unchecked
            {

                if (sequence.First.Length >= 5)
                {
                    bool flag = InternalTryGetUInt32(sequence.First.Span, out value, out int int_consumed);
                    consumed = sequence.GetPosition(int_consumed);

                    return flag;
                }
                else
                {
                    Span<byte> span = stackalloc byte[(int)Math.Min(5, sequence.Length)];
                    sequence.CopyTo(span);

                    bool flag = InternalTryGetUInt32(span, out value, out int int_consumed);
                    consumed = sequence.GetPosition(int_consumed);

                    return flag;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetUInt64(ReadOnlySequence<byte> sequence, out ulong value, out SequencePosition consumed)
        {
            unchecked
            {
                if (sequence.First.Length >= 9)
                {
                    bool flag = InternalTryGetUInt64(sequence.First.Span, out value, out int int_consumed);
                    consumed = sequence.GetPosition(int_consumed);

                    return flag;
                }
                else
                {
                    Span<byte> span = stackalloc byte[(int)Math.Min(9, sequence.Length)];
                    sequence.CopyTo(span);

                    bool flag = InternalTryGetUInt64(span, out value, out int int_consumed);
                    consumed = sequence.GetPosition(int_consumed);

                    return flag;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetInt8(ReadOnlySequence<byte> sequence, out sbyte result, out SequencePosition consumed)
        {
            unchecked
            {
                result = 0;

                if (!TryGetUInt8(sequence, out byte byte_result, out consumed))
                {
                    return false;
                }

                result = (sbyte)((byte_result >> 1) ^ (-((sbyte)byte_result & 1)));
                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetInt16(ReadOnlySequence<byte> sequence, out short result, out SequencePosition consumed)
        {
            unchecked
            {
                result = 0;

                if (!TryGetUInt16(sequence, out ushort ushort_result, out consumed))
                {
                    return false;
                }

                result = (short)((ushort_result >> 1) ^ (-((short)ushort_result & 1)));
                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetInt32(ReadOnlySequence<byte> sequence, out int result, out SequencePosition consumed)
        {
            unchecked
            {
                result = 0;

                if (!TryGetUInt32(sequence, out uint uint_result, out consumed))
                {
                    return false;
                }

                result = (int)(uint_result >> 1) ^ (-((int)uint_result & 1));
                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetInt64(ReadOnlySequence<byte> sequence, out long result, out SequencePosition consumed)
        {
            unchecked
            {
                result = 0;

                if (!TryGetUInt64(sequence, out ulong ulong_result, out consumed))
                {
                    return false;
                }

                result = (long)(ulong_result >> 1) ^ (-((long)ulong_result & 1));
                return true;
            }
        }
    }
}
