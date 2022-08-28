using System.Buffers;
using System.Runtime.CompilerServices;
using Omnius.Core.RocketPack.Internal;

namespace Omnius.Core.RocketPack;

/// <summary>
/// RocketPackフォーマットのデシリアライズ機能を提供します。
/// </summary>
public unsafe ref struct RocketMessageReader
{
    private SequenceReader<byte> _reader;
    private IBytesPool _bytesPool;

    public RocketMessageReader(in ReadOnlySequence<byte> sequence, in IBytesPool bytesPool)
    {
        _reader = new SequenceReader<byte>(sequence);
        _bytesPool = bytesPool;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IMemoryOwner<byte> GetRecyclableMemory(in int limit)
    {
        if (!Varint.TryGetUInt32(ref _reader, out uint length)) throw new FormatException();
        if (length > limit) throw new FormatException();

        if (length == 0) return MemoryOwner<byte>.Empty;

        var memoryOwner = _bytesPool.Memory.Rent((int)length).Shrink((int)length);

        _reader.TryCopyTo(memoryOwner.Memory.Span);
        _reader.Advance(length);

        return memoryOwner;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyMemory<byte> GetMemory(in int limit)
    {
        if (!Varint.TryGetUInt32(ref _reader, out uint length)) throw new FormatException();
        if (length > limit) throw new FormatException();

        if (length == 0) return ReadOnlyMemory<byte>.Empty;

        var result = new byte[(int)length];

        _reader.TryCopyTo(result.AsSpan());
        _reader.Advance(length);

        return new ReadOnlyMemory<byte>(result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8String GetString(in int limit)
    {
        if (!Varint.TryGetUInt32(ref _reader, out uint length)) throw new FormatException();
        if (length > limit) throw new FormatException();

        if (length == 0) return Utf8String.Empty;

        var result = new byte[(int)length];

        _reader.TryCopyTo(result.AsSpan());
        _reader.Advance(length);

        return new Utf8String(result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Timestamp64 GetTimestamp64()
    {
        long seconds = this.GetInt64();

        return new Timestamp64(seconds);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Timestamp96 GetTimestamp96()
    {
        long seconds = this.GetInt64();
        int nanos = (int)this.GetUInt32();

        return new Timestamp96(seconds, nanos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GetBoolean()
    {
        if (!Varint.TryGetUInt64(ref _reader, out ulong result)) throw new FormatException();

        return (result != 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong GetUInt64()
    {
        if (!Varint.TryGetUInt64(ref _reader, out ulong result)) throw new FormatException();

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte GetUInt8()
    {
        if (!Varint.TryGetUInt8(ref _reader, out byte result)) throw new FormatException();

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort GetUInt16()
    {
        if (!Varint.TryGetUInt16(ref _reader, out ushort result)) throw new FormatException();

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint GetUInt32()
    {
        if (!Varint.TryGetUInt32(ref _reader, out uint result)) throw new FormatException();

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte GetInt8()
    {
        if (!Varint.TryGetInt8(ref _reader, out sbyte result)) throw new FormatException();

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short GetInt16()
    {
        if (!Varint.TryGetInt16(ref _reader, out short result)) throw new FormatException();

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetInt32()
    {
        if (!Varint.TryGetInt32(ref _reader, out int result)) throw new FormatException();

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetInt64()
    {
        if (!Varint.TryGetInt64(ref _reader, out long result)) throw new FormatException();

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float GetFloat32()
    {
        const int Size = 4;
        byte* buffer = stackalloc byte[Size];
        var tempSpan = new Span<byte>(buffer, Size);

        _reader.TryCopyTo(tempSpan);

        var f = new Float32Bits(tempSpan);

        return f.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double GetFloat64()
    {
        const int Size = 8;
        byte* buffer = stackalloc byte[Size];
        var tempSpan = new Span<byte>(buffer, Size);

        _reader.TryCopyTo(tempSpan);

        var f = new Float64Bits(tempSpan);

        return f.Value;
    }
}
