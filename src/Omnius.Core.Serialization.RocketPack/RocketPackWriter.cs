using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using Omnius.Core;
using Omnius.Core.Serialization.RocketPack.Internal;

namespace Omnius.Core.Serialization.RocketPack
{
    /// <summary>
    /// RocketPackフォーマットのシリアライズ機能を提供します。
    /// </summary>
    public ref struct RocketPackWriter
    {
        private static readonly Lazy<Encoding> _encoding = new Lazy<Encoding>(() => new UTF8Encoding(false));

        private IBufferWriter<byte> _bufferWriter;
        private IBytesPool _bytesPool;

        public RocketPackWriter(in IBufferWriter<byte> bufferWriter, in IBytesPool bytesPool)
        {
            _bufferWriter = bufferWriter;
            _bytesPool = bytesPool;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in string value)
        {
            using (var memoryOwner = _bytesPool.Memory.Rent(_encoding.Value.GetMaxByteCount(value.Length)))
            {
                int length = _encoding.Value.GetBytes(value.AsSpan(), memoryOwner.Memory.Span);
                Varint.SetUInt32((uint)length, _bufferWriter);

                _bufferWriter.Write(memoryOwner.Memory.Span.Slice(0, length));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in ReadOnlySpan<byte> value)
        {
            Varint.SetUInt32((uint)value.Length, _bufferWriter);
            _bufferWriter.Write(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in Timestamp value)
        {
            this.Write(value.Seconds);
            this.Write((uint)value.Nanos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in bool value)
        {
            _bufferWriter.GetSpan(1)[0] = !value ? (byte)0x00 : (byte)0x01;
            _bufferWriter.Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in byte value)
        {
            Varint.SetUInt64(value, _bufferWriter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in ushort value)
        {
            Varint.SetUInt64(value, _bufferWriter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in uint value)
        {
            Varint.SetUInt64(value, _bufferWriter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in ulong value)
        {
            Varint.SetUInt64(value, _bufferWriter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in sbyte value)
        {
            Varint.SetInt64(value, _bufferWriter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in short value)
        {
            Varint.SetInt64(value, _bufferWriter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in int value)
        {
            Varint.SetInt64(value, _bufferWriter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in long value)
        {
            Varint.SetInt64(value, _bufferWriter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in float value)
        {
            var f = new Float32Bits(value);
            var tempSpan = _bufferWriter.GetSpan(4);
            f.CopyTo(ref tempSpan);
            _bufferWriter.Advance(4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(in double value)
        {
            var f = new Float64Bits(value);
            var tempSpan = _bufferWriter.GetSpan(8);
            f.CopyTo(ref tempSpan);
            _bufferWriter.Advance(8);
        }
    }
}
