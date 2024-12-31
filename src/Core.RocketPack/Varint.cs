using System.Buffers;
using System.Runtime.CompilerServices;
using Omnius.Core.Base;

namespace Omnius.Core.RocketPack;

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
    public static void PutUInt8(scoped in byte value, scoped in IBufferWriter<byte> writer)
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
    public static void PutUInt16(scoped in ushort value, scoped in IBufferWriter<byte> writer)
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
    public static void PutUInt32(scoped in uint value, scoped in IBufferWriter<byte> writer)
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
    public static void PutUInt64(scoped in ulong value, scoped in IBufferWriter<byte> writer)
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
                var span = writer.GetSpan(9);

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
    public static void PutInt8(scoped in sbyte value, scoped in IBufferWriter<byte> writer)
    {
        unchecked
        {
            PutUInt8((byte)((value << 1) ^ (value >> 7)), writer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PutInt16(scoped in short value, scoped in IBufferWriter<byte> writer)
    {
        unchecked
        {
            PutUInt16((ushort)((value << 1) ^ (value >> 15)), writer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PutInt32(scoped in int value, scoped in IBufferWriter<byte> writer)
    {
        unchecked
        {
            PutUInt32((uint)((value << 1) ^ (value >> 31)), writer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PutInt64(scoped in long value, scoped in IBufferWriter<byte> writer)
    {
        unchecked
        {
            PutUInt64((ulong)((value << 1) ^ (value >> 63)), writer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetUInt8(ref ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);
        var result = GetUInt8(ref reader);
        sequence = sequence.Slice(reader.Consumed);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetUInt8(ref SequenceReader<byte> reader)
    {
        if (reader.Remaining == 0) throw new VarintException(VarintErrorCode.EndOfInput);

        return InternalGetValue(ref reader, 2, InternalGetUInt8);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort GetUInt16(ref ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);
        var result = GetUInt16(ref reader);
        sequence = sequence.Slice(reader.Consumed);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort GetUInt16(ref SequenceReader<byte> reader)
    {
        if (reader.Remaining == 0) throw new VarintException(VarintErrorCode.EndOfInput);

        return InternalGetValue(ref reader, 3, InternalGetUInt16);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetUInt32(ref ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);
        var result = GetUInt32(ref reader);
        sequence = sequence.Slice(reader.Consumed);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetUInt32(ref SequenceReader<byte> reader)
    {
        if (reader.Remaining == 0) throw new VarintException(VarintErrorCode.EndOfInput);

        return InternalGetValue(ref reader, 5, InternalGetUInt32);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetUInt64(ref ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);
        var result = GetUInt64(ref reader);
        sequence = sequence.Slice(reader.Consumed);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetUInt64(ref SequenceReader<byte> reader)
    {
        if (reader.Remaining == 0) throw new VarintException(VarintErrorCode.EndOfInput);

        return InternalGetValue(ref reader, 9, InternalGetUInt64);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte GetInt8(ref ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);
        var result = GetInt8(ref reader);
        sequence = sequence.Slice(reader.Consumed);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte GetInt8(ref SequenceReader<byte> reader)
    {
        unchecked
        {
            var byte_result = GetUInt8(ref reader);
            return (sbyte)((byte_result >> 1) ^ (-((sbyte)byte_result & 1)));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short GetInt16(ref ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);
        var result = GetInt16(ref reader);
        sequence = sequence.Slice(reader.Consumed);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short GetInt16(ref SequenceReader<byte> reader)
    {
        unchecked
        {
            var ushort_result = GetUInt16(ref reader);
            return (short)((ushort_result >> 1) ^ (-((short)ushort_result & 1)));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetInt32(ref ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);
        var result = GetInt32(ref reader);
        sequence = sequence.Slice(reader.Consumed);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetInt32(ref SequenceReader<byte> reader)
    {
        unchecked
        {
            var uint_result = GetUInt32(ref reader);
            return (int)(uint_result >> 1) ^ (-((int)uint_result & 1));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long GetInt64(ref ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);
        var result = GetInt64(ref reader);
        sequence = sequence.Slice(reader.Consumed);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long GetInt64(ref SequenceReader<byte> reader)
    {
        unchecked
        {
            var ulong_result = GetUInt64(ref reader);
            return (long)(ulong_result >> 1) ^ (-((long)ulong_result & 1));
        }
    }

    private delegate TResult ConvertReadOnlySpan<TResult>(scoped in ReadOnlySpan<byte> span, out int consumed);

    private static TResult InternalGetValue<TResult>(ref SequenceReader<byte> reader, int maxLength, ConvertReadOnlySpan<TResult> action)
    {
        if (reader.UnreadSpan.Length >= maxLength)
        {
            var readOnlySpan = reader.UnreadSpan;
            var result = action(readOnlySpan, out int consumed);
            reader.Advance(consumed);
            return result;
        }
        else
        {
            var length = (int)Math.Min(reader.Remaining, maxLength);
            Span<byte> buffer = stackalloc byte[length];
            _ = reader.TryCopyTo(buffer);

            var readOnlySpan = (ReadOnlySpan<byte>)buffer;
            var result = action(readOnlySpan, out int consumed);
            reader.Advance(consumed);
            return result;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte InternalGetUInt8(scoped in ReadOnlySpan<byte> span, out int consumed)
    {
        unchecked
        {
            fixed (byte* fixed_p = span)
            {
                var p = fixed_p;

                if ((*p & 0x80) == 0)
                {
                    consumed = 1;
                    return *p;
                }
                else if (*p == Int8Code)
                {
                    if (span.Length < 2) throw new VarintException(VarintErrorCode.TooSmallBody);
                    consumed = 2;
                    return p[1];
                }
                else
                {
                    throw new VarintException(VarintErrorCode.InvalidHeader);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ushort InternalGetUInt16(scoped in ReadOnlySpan<byte> span, out int consumed)
    {
        unchecked
        {
            fixed (byte* fixed_p = span)
            {
                var p = fixed_p;

                if ((*p & 0x80) == 0)
                {
                    consumed = 1;
                    return *p;
                }
                else if (*p == Int8Code)
                {
                    if (span.Length < 2) throw new VarintException(VarintErrorCode.TooSmallBody);
                    consumed = 2;
                    return p[1];
                }
                else if (*p == Int16Code)
                {
                    if (span.Length < 3) throw new VarintException(VarintErrorCode.TooSmallBody);
                    consumed = 3;
                    return *(ushort*)(p + 1);
                }
                else
                {
                    throw new VarintException(VarintErrorCode.InvalidHeader);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint InternalGetUInt32(scoped in ReadOnlySpan<byte> span, out int consumed)
    {
        unchecked
        {
            fixed (byte* fixed_p = span)
            {
                var p = fixed_p;

                if ((*p & 0x80) == 0)
                {
                    consumed = 1;
                    return *p;
                }
                else if (*p == Int8Code)
                {
                    if (span.Length < 2) throw new VarintException(VarintErrorCode.TooSmallBody);
                    consumed = 2;
                    return p[1];
                }
                else if (*p == Int16Code)
                {
                    if (span.Length < 3) throw new VarintException(VarintErrorCode.TooSmallBody);
                    consumed = 3;
                    return *(ushort*)(p + 1);
                }
                else if (*p == Int32Code)
                {
                    if (span.Length < 5) throw new VarintException(VarintErrorCode.TooSmallBody);
                    consumed = 5;
                    return *(uint*)(p + 1);
                }
                else
                {
                    throw new VarintException(VarintErrorCode.InvalidHeader);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong InternalGetUInt64(scoped in ReadOnlySpan<byte> span, out int consumed)
    {
        unchecked
        {
            fixed (byte* fixed_p = span)
            {
                var p = fixed_p;

                if ((*p & 0x80) == 0)
                {
                    consumed = 1;
                    return *p;
                }
                else if (*p == Int8Code)
                {
                    if (span.Length < 2) throw new VarintException(VarintErrorCode.TooSmallBody);
                    consumed = 2;
                    return p[1];
                }
                else if (*p == Int16Code)
                {
                    if (span.Length < 3) throw new VarintException(VarintErrorCode.TooSmallBody);
                    consumed = 3;
                    return *(ushort*)(p + 1);
                }
                else if (*p == Int32Code)
                {
                    if (span.Length < 5) throw new VarintException(VarintErrorCode.TooSmallBody);
                    consumed = 5;
                    return *(uint*)(p + 1);
                }
                else if (*p == Int64Code)
                {
                    if (span.Length < 9) throw new VarintException(VarintErrorCode.TooSmallBody);
                    consumed = 9;
                    return *(ulong*)(p + 1);
                }
                else
                {
                    throw new VarintException(VarintErrorCode.InvalidHeader);
                }
            }
        }
    }
}
