using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Omnius.Core.RocketPack
{
    /// <summary>
    /// <see cref="long"/>と<see cref="byte"/>[]の変換機能を提供します。
    /// </summary>
    public static unsafe class Varint
    {
        public const byte MinInt7 = 0x00; // 0
        public const byte MaxInt7 = 0x7F; // 127

        private const byte Int8Code = 0x80;
        private const byte Int16Code = 0x81;
        private const byte Int32Code = 0x82;
        private const byte Int64Code = 0x83;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetUInt8(in byte value, in IBufferWriter<byte> writer)
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
        public static void SetUInt16(in ushort value, in IBufferWriter<byte> writer)
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
        public static void SetUInt32(in uint value, in IBufferWriter<byte> writer)
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
        public static void SetUInt64(in ulong value, in IBufferWriter<byte> writer)
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
        public static void SetInt8(in sbyte value, in IBufferWriter<byte> writer)
        {
            unchecked
            {
                SetUInt8((byte)((value << 1) ^ (value >> 7)), writer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetInt16(in short value, in IBufferWriter<byte> writer)
        {
            unchecked
            {
                SetUInt16((ushort)((value << 1) ^ (value >> 15)), writer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetInt32(in int value, in IBufferWriter<byte> writer)
        {
            unchecked
            {
                SetUInt32((uint)((value << 1) ^ (value >> 31)), writer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetInt64(in long value, in IBufferWriter<byte> writer)
        {
            unchecked
            {
                SetUInt64((ulong)((value << 1) ^ (value >> 63)), writer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InternalTryGetUInt8(in ReadOnlySpan<byte> span, out byte value, out int consumed)
        {
            unchecked
            {
                fixed (byte* fixed_p = span)
                {
                    var p = fixed_p;

                    if ((*p & 0x80) == 0 && span.Length >= 1)
                    {
                        value = *p;
                        consumed = 1;
                        return true;
                    }
                    else
                    {
                        switch (*p)
                        {
                            case Int8Code when span.Length >= 2:
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
        private static bool InternalTryGetUInt16(in ReadOnlySpan<byte> span, out ushort value, out int consumed)
        {
            unchecked
            {
                fixed (byte* fixed_p = span)
                {
                    var p = fixed_p;

                    if ((*p & 0x80) == 0 && span.Length >= 1)
                    {
                        value = *p;
                        consumed = 1;
                        return true;
                    }
                    else
                    {
                        switch (*p)
                        {
                            case Int8Code when span.Length >= 2:
                                value = p[1];
                                consumed = 2;
                                return true;
                            case Int16Code when span.Length >= 3:
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
        private static bool InternalTryGetUInt32(in ReadOnlySpan<byte> span, out uint value, out int consumed)
        {
            unchecked
            {
                fixed (byte* fixed_p = span)
                {
                    var p = fixed_p;

                    if ((*p & 0x80) == 0 && span.Length >= 1)
                    {
                        value = *p;
                        consumed = 1;
                        return true;
                    }
                    else
                    {
                        switch (*p)
                        {
                            case Int8Code when span.Length >= 2:
                                value = p[1];
                                consumed = 2;
                                return true;
                            case Int16Code when span.Length >= 3:
                                value = *(ushort*)(p + 1);
                                consumed = 3;
                                return true;
                            case Int32Code when span.Length >= 5:
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
        private static bool InternalTryGetUInt64(in ReadOnlySpan<byte> span, out ulong value, out int consumed)
        {
            unchecked
            {
                fixed (byte* fixed_p = span)
                {
                    var p = fixed_p;

                    if ((*p & 0x80) == 0 && span.Length >= 1)
                    {
                        value = *p;
                        consumed = 1;
                        return true;
                    }
                    else
                    {
                        switch (*p)
                        {
                            case Int8Code when span.Length >= 2:
                                value = p[1];
                                consumed = 2;
                                return true;
                            case Int16Code when span.Length >= 3:
                                value = *(ushort*)(p + 1);
                                consumed = 3;
                                return true;
                            case Int32Code when span.Length >= 5:
                                value = *(uint*)(p + 1);
                                consumed = 5;
                                return true;
                            case Int64Code when span.Length >= 9:
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
        public static bool TryGetUInt8(ref SequenceReader<byte> reader, out byte value)
        {
            value = 0;
            if (reader.Remaining == 0) return false;

            unchecked
            {
                if (reader.UnreadSpan.Length >= 2)
                {
                    if (!InternalTryGetUInt8(reader.UnreadSpan, out value, out int consumed)) return false;

                    reader.Advance(consumed);
                    return true;
                }
                else
                {
                    int size = (int)Math.Min(2, reader.Remaining);
                    byte* buffer = stackalloc byte[size];
                    var tempSpan = new Span<byte>(buffer, size);

                    if (!reader.TryCopyTo(tempSpan)) return false;
                    if (!InternalTryGetUInt8(tempSpan, out value, out int consumed)) return false;

                    reader.Advance(consumed);
                    return true;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetUInt8(ref ReadOnlySequence<byte> sequence, out byte value)
        {
            var reader = new SequenceReader<byte>(sequence);
            var result = TryGetUInt8(ref reader, out value);
            sequence = sequence.Slice(reader.Consumed);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetUInt16(ref SequenceReader<byte> reader, out ushort value)
        {
            value = 0;
            if (reader.Remaining == 0) return false;

            unchecked
            {
                if (reader.UnreadSpan.Length >= 3)
                {
                    if (!InternalTryGetUInt16(reader.UnreadSpan, out value, out int consumed)) return false;

                    reader.Advance(consumed);
                    return true;
                }
                else
                {
                    int size = (int)Math.Min(3, reader.Remaining);
                    byte* buffer = stackalloc byte[size];
                    var tempSpan = new Span<byte>(buffer, size);

                    if (!reader.TryCopyTo(tempSpan)) return false;
                    if (!InternalTryGetUInt16(tempSpan, out value, out int consumed)) return false;

                    reader.Advance(consumed);
                    return true;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetUInt16(ref ReadOnlySequence<byte> sequence, out ushort value)
        {
            var reader = new SequenceReader<byte>(sequence);
            var result = TryGetUInt16(ref reader, out value);
            sequence = sequence.Slice(reader.Consumed);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetUInt32(ref SequenceReader<byte> reader, out uint value)
        {
            value = 0;
            if (reader.Remaining == 0) return false;

            unchecked
            {
                if (reader.UnreadSpan.Length >= 5)
                {
                    if (!InternalTryGetUInt32(reader.UnreadSpan, out value, out int consumed)) return false;

                    reader.Advance(consumed);
                    return true;
                }
                else
                {
                    int size = (int)Math.Min(5, reader.Remaining);
                    byte* buffer = stackalloc byte[size];
                    var tempSpan = new Span<byte>(buffer, size);

                    if (!reader.TryCopyTo(tempSpan)) return false;
                    if (!InternalTryGetUInt32(tempSpan, out value, out int consumed)) return false;

                    reader.Advance(consumed);
                    return true;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetUInt32(ref ReadOnlySequence<byte> sequence, out uint value)
        {
            var reader = new SequenceReader<byte>(sequence);
            var result = TryGetUInt32(ref reader, out value);
            sequence = sequence.Slice(reader.Consumed);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetUInt64(ref SequenceReader<byte> reader, out ulong value)
        {
            value = 0;
            if (reader.Remaining == 0) return false;

            unchecked
            {
                if (reader.UnreadSpan.Length >= 9)
                {
                    if (!InternalTryGetUInt64(reader.UnreadSpan, out value, out int consumed)) return false;

                    reader.Advance(consumed);
                    return true;
                }
                else
                {
                    int size = (int)Math.Min(9, reader.Remaining);
                    byte* buffer = stackalloc byte[size];
                    var tempSpan = new Span<byte>(buffer, size);

                    if (!reader.TryCopyTo(tempSpan)) return false;
                    if (!InternalTryGetUInt64(tempSpan, out value, out int consumed)) return false;

                    reader.Advance(consumed);
                    return true;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetUInt64(ref ReadOnlySequence<byte> sequence, out ulong value)
        {
            var reader = new SequenceReader<byte>(sequence);
            var result = TryGetUInt64(ref reader, out value);
            sequence = sequence.Slice(reader.Consumed);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetInt8(ref SequenceReader<byte> reader, out sbyte value)
        {
            unchecked
            {
                value = 0;

                if (!TryGetUInt8(ref reader, out byte byte_result)) return false;

                value = (sbyte)((byte_result >> 1) ^ (-((sbyte)byte_result & 1)));
                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetInt8(ref ReadOnlySequence<byte> sequence, out sbyte value)
        {
            var reader = new SequenceReader<byte>(sequence);
            var result = TryGetInt8(ref reader, out value);
            sequence = sequence.Slice(reader.Consumed);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetInt16(ref SequenceReader<byte> reader, out short value)
        {
            unchecked
            {
                value = 0;

                if (!TryGetUInt16(ref reader, out ushort ushort_result)) return false;

                value = (short)((ushort_result >> 1) ^ (-((short)ushort_result & 1)));
                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetInt16(ref ReadOnlySequence<byte> sequence, out short value)
        {
            var reader = new SequenceReader<byte>(sequence);
            var result = TryGetInt16(ref reader, out value);
            sequence = sequence.Slice(reader.Consumed);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetInt32(ref SequenceReader<byte> reader, out int value)
        {
            unchecked
            {
                value = 0;

                if (!TryGetUInt32(ref reader, out uint uint_result)) return false;

                value = (int)(uint_result >> 1) ^ (-((int)uint_result & 1));
                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetInt32(ref ReadOnlySequence<byte> sequence, out int value)
        {
            var reader = new SequenceReader<byte>(sequence);
            var result = TryGetInt32(ref reader, out value);
            sequence = sequence.Slice(reader.Consumed);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetInt64(ref SequenceReader<byte> reader, out long result)
        {
            unchecked
            {
                result = 0;

                if (!TryGetUInt64(ref reader, out ulong ulong_result)) return false;

                result = (long)(ulong_result >> 1) ^ (-((long)ulong_result & 1));
                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetInt64(ref ReadOnlySequence<byte> sequence, out long value)
        {
            var reader = new SequenceReader<byte>(sequence);
            var result = TryGetInt64(ref reader, out value);
            sequence = sequence.Slice(reader.Consumed);
            return result;
        }
    }
}
