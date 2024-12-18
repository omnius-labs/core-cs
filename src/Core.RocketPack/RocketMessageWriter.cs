using System.Buffers;
using System.Runtime.CompilerServices;
using Omnius.Core.Base;
using Omnius.Core.RocketPack.Internal;

namespace Omnius.Core.RocketPack;

/// <summary>
/// RocketPackフォーマットのシリアライズ機能を提供します。
/// </summary>
public ref struct RocketMessageWriter
{
    private IBufferWriter<byte> _bufferWriter;
    private IBytesPool _bytesPool;

    public RocketMessageWriter(in IBufferWriter<byte> bufferWriter, in IBytesPool bytesPool)
    {
        _bufferWriter = bufferWriter;
        _bytesPool = bytesPool;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Put(scoped in Utf8String value)
    {
        Varint.PutUInt32((uint)value.Length, _bufferWriter);
        _bufferWriter.Write(value.Span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Put(scoped in ReadOnlySpan<byte> value)
    {
        Varint.PutUInt32((uint)value.Length, _bufferWriter);
        _bufferWriter.Write(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Put(scoped in Timestamp64 value)
    {
        this.Put(value.Seconds);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Put(scoped in Timestamp96 value)
    {
        this.Put(value.Seconds);
        this.Put((uint)value.Nanos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Put(scoped in bool value)
    {
        _bufferWriter.GetSpan(1)[0] = !value ? (byte)0x00 : (byte)0x01;
        _bufferWriter.Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Put(scoped in byte value)
    {
        Varint.PutUInt64(value, _bufferWriter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Put(scoped in ushort value)
    {
        Varint.PutUInt64(value, _bufferWriter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Put(scoped in uint value)
    {
        Varint.PutUInt64(value, _bufferWriter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Put(scoped in ulong value)
    {
        Varint.PutUInt64(value, _bufferWriter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Put(scoped in sbyte value)
    {
        Varint.PutInt64(value, _bufferWriter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Put(scoped in short value)
    {
        Varint.PutInt64(value, _bufferWriter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Put(scoped in int value)
    {
        Varint.PutInt64(value, _bufferWriter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Put(scoped in long value)
    {
        Varint.PutInt64(value, _bufferWriter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Put(scoped in float value)
    {
        var f = new Float32Bits(value);
        var tempSpan = _bufferWriter.GetSpan(4);
        f.CopyTo(ref tempSpan);
        _bufferWriter.Advance(4);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Put(scoped in double value)
    {
        var f = new Float64Bits(value);
        var tempSpan = _bufferWriter.GetSpan(8);
        f.CopyTo(ref tempSpan);
        _bufferWriter.Advance(8);
    }
}
