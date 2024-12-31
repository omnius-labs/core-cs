using System.Buffers;
using System.Runtime.CompilerServices;
using Omnius.Core.Base;
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
    public IMemoryOwner<byte> GetRecyclableMemory(scoped in int limit)
    {
        var length = this.GetUInt32();
        if (length > limit) throw new RocketMessageException(RocketMessageErrorCode.LimitExceeded);

        if (length == 0) return MemoryOwner<byte>.Empty;

        if (_reader.Remaining < length) throw new RocketMessageException(RocketMessageErrorCode.TooSmallBody);

        var memoryOwner = _bytesPool.Memory.Rent((int)length).Shrink((int)length);
        _reader.TryCopyTo(memoryOwner.Memory.Span);
        _reader.Advance(length);

        return memoryOwner;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] GetBytes(scoped in int limit)
    {
        var length = this.GetUInt32();
        if (length > limit) throw new RocketMessageException(RocketMessageErrorCode.LimitExceeded);

        if (length == 0) return Array.Empty<byte>();

        if (_reader.Remaining < length) throw new RocketMessageException(RocketMessageErrorCode.TooSmallBody);

        var result = new byte[(int)length];
        _reader.TryCopyTo(result.AsSpan());
        _reader.Advance(length);

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8String GetString(scoped in int limit)
    {
        var bytes = this.GetBytes(limit);
        return new Utf8String(bytes);
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
        uint nanos = this.GetUInt32();
        return new Timestamp96(seconds, nanos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GetBool()
    {
        var v = this.GetUInt64();
        return (v != 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte GetUInt8()
    {
        return Varint.GetUInt8(ref _reader);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort GetUInt16()
    {
        return Varint.GetUInt16(ref _reader);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint GetUInt32()
    {
        return Varint.GetUInt32(ref _reader);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong GetUInt64()
    {
        return Varint.GetUInt64(ref _reader);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte GetInt8()
    {
        return Varint.GetInt8(ref _reader);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short GetInt16()
    {
        return Varint.GetInt16(ref _reader);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetInt32()
    {
        return Varint.GetInt32(ref _reader);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetInt64()
    {
        return Varint.GetInt64(ref _reader);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float GetFloat32()
    {
        if (_reader.Remaining < sizeof(float)) throw new RocketMessageException(RocketMessageErrorCode.EndOfInput);

        const int Size = 4;
        Span<byte> buffer = stackalloc byte[Size];
        _reader.TryCopyTo(buffer);

        var f = new Float32Bits(buffer);
        return f.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double GetFloat64()
    {
        if (_reader.Remaining < sizeof(double)) throw new RocketMessageException(RocketMessageErrorCode.EndOfInput);

        const int Size = 8;
        Span<byte> buffer = stackalloc byte[Size];
        _reader.TryCopyTo(buffer);

        var f = new Float64Bits(buffer);
        return f.Value;
    }
}
