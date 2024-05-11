using System.Buffers;
using System.Runtime.CompilerServices;
using Core.Base;
using Core.RocketPack.Internal;

namespace Core.RocketPack;

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
    public void Write(scoped in Utf8String value)
    {
        Varint.SetUInt32((uint)value.Length, _bufferWriter);
        _bufferWriter.Write(value.Span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped in ReadOnlySpan<byte> value)
    {
        Varint.SetUInt32((uint)value.Length, _bufferWriter);
        _bufferWriter.Write(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped in Timestamp64 value)
    {
        this.Write(value.Seconds);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped in Timestamp96 value)
    {
        this.Write(value.Seconds);
        this.Write((uint)value.Nanos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped in bool value)
    {
        _bufferWriter.GetSpan(1)[0] = !value ? (byte)0x00 : (byte)0x01;
        _bufferWriter.Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped in byte value)
    {
        Varint.SetUInt64(value, _bufferWriter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped in ushort value)
    {
        Varint.SetUInt64(value, _bufferWriter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped in uint value)
    {
        Varint.SetUInt64(value, _bufferWriter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped in ulong value)
    {
        Varint.SetUInt64(value, _bufferWriter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped in sbyte value)
    {
        Varint.SetInt64(value, _bufferWriter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped in short value)
    {
        Varint.SetInt64(value, _bufferWriter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped in int value)
    {
        Varint.SetInt64(value, _bufferWriter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped in long value)
    {
        Varint.SetInt64(value, _bufferWriter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped in float value)
    {
        var f = new Float32Bits(value);
        var tempSpan = _bufferWriter.GetSpan(4);
        f.CopyTo(ref tempSpan);
        _bufferWriter.Advance(4);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped in double value)
    {
        var f = new Float64Bits(value);
        var tempSpan = _bufferWriter.GetSpan(8);
        f.CopyTo(ref tempSpan);
        _bufferWriter.Advance(8);
    }
}
