using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Omnix.Base;
using Omnix.Serialization.RocketPack.Internal;

namespace Omnix.Serialization.RocketPack
{
    /// <summary>
    /// RocketPackフォーマットのシリアライズ機能を提供します。
    /// </summary>
    public sealed class RocketPackWriter
    {
        private IBufferWriter<byte> _bufferWriter;
        private BufferPool _bufferPool;

        private static readonly Lazy<Encoding> _encoding = new Lazy<Encoding>(() => new UTF8Encoding(false));

        public RocketPackWriter(IBufferWriter<byte> bufferWriter, BufferPool bufferPool)
        {
            _bufferWriter = bufferWriter;
            _bufferPool = bufferPool;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(string value)
        {
            using (var memoryOwner = _bufferPool.Rent(_encoding.Value.GetMaxByteCount(value.Length)))
            {
                int length = _encoding.Value.GetBytes(value.AsSpan(), memoryOwner.Memory.Span);
                Varint.SetUInt32((uint)length, _bufferWriter);

                _bufferWriter.Write(memoryOwner.Memory.Span.Slice(0, length));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ReadOnlySpan<byte> value)
        {
            Varint.SetUInt32((uint)value.Length, _bufferWriter);
            _bufferWriter.Write(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(Timestamp value)
        {
            this.Write((long)value.Seconds);
            this.Write((uint)value.Nanos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(bool value)
        {
            _bufferWriter.GetSpan(1)[0] = !value ? (byte)0x00 : (byte)0x01;
            _bufferWriter.Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(byte value)
        {
            Varint.SetUInt64(value, _bufferWriter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ushort value)
        {
            Varint.SetUInt64(value, _bufferWriter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(uint value)
        {
            Varint.SetUInt64(value, _bufferWriter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ulong value)
        {
            Varint.SetUInt64(value, _bufferWriter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(sbyte value)
        {
            Varint.SetInt64(value, _bufferWriter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(short value)
        {
            Varint.SetInt64(value, _bufferWriter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int value)
        {
            Varint.SetInt64(value, _bufferWriter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(long value)
        {
            Varint.SetInt64(value, _bufferWriter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(float value)
        {
            var f = new Float32Bits(value);
            f.CopyTo(_bufferWriter.GetSpan(4));
            _bufferWriter.Advance(4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write(double value)
        {
            var f = new Float64Bits(value);
            f.CopyTo(_bufferWriter.GetSpan(8));
            _bufferWriter.Advance(8);
        }
    }
}
