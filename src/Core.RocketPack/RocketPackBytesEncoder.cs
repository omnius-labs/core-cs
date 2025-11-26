using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;
using Omnius.Core.Base;

namespace Omnius.Core.RocketPack;

public sealed class RocketPackEncoderException : Exception
{
    private RocketPackEncoderException(RocketPackEncoderErrorCode errorCode, string message) : base(message)
    {
        this.ErrorCode = errorCode;
    }

    public RocketPackEncoderErrorCode ErrorCode { get; }

    public static RocketPackEncoderException CreateIoError(string message)
    {
        return new RocketPackEncoderException(RocketPackEncoderErrorCode.IoError, message);
    }
}

public enum RocketPackEncoderErrorCode
{
    IoError,
}

public ref struct RocketPackBytesEncoder
{
    private IBufferWriter<byte> _bufferWriter;
    private IBytesPool _bytesPool;

    public RocketPackBytesEncoder(in IBufferWriter<byte> bufferWriter, in IBytesPool bytesPool)
    {
        _bufferWriter = bufferWriter;
        _bytesPool = bytesPool;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte Compose(byte major, byte info)
    {
        return (byte)((major << 5) | (info & 0b0001_1111));
    }

    public void WriteBool(bool value)
    {
        this.WriteRawBytes([Compose(7, value ? (byte)21 : (byte)20)]);
    }

    public void WriteU8(byte value)
    {
        if (value <= 23)
        {
            this.WriteRawBytes([Compose(0, value)]);
        }
        else
        {
            Span<byte> buf = stackalloc byte[2];
            buf[0] = Compose(0, 24);
            buf[1] = (byte)value;
            this.WriteRawBytes(buf);
        }
    }

    public void WriteU16(ushort value)
    {
        if (value <= 23)
        {
            this.WriteRawBytes([Compose(0, (byte)value)]);
        }
        else if (value <= byte.MaxValue)
        {
            Span<byte> buf = stackalloc byte[2];
            buf[0] = Compose(0, 24);
            buf[1] = (byte)value;
            this.WriteRawBytes(buf);
        }
        else
        {
            Span<byte> buf = stackalloc byte[3];
            buf[0] = Compose(0, 25);
            BinaryPrimitives.WriteUInt16BigEndian(buf[1..], value);
            this.WriteRawBytes(buf);
        }
    }

    public void WriteU32(uint value)
    {
        if (value <= 23)
        {
            this.WriteRawBytes([Compose(0, (byte)value)]);
        }
        else if (value <= byte.MaxValue)
        {
            Span<byte> buf = stackalloc byte[2];
            buf[0] = Compose(0, 24);
            buf[1] = (byte)value;
            this.WriteRawBytes(buf);
        }
        else if (value <= ushort.MaxValue)
        {
            Span<byte> buf = stackalloc byte[3];
            buf[0] = Compose(0, 25);
            BinaryPrimitives.WriteUInt16BigEndian(buf[1..], (ushort)value);
            this.WriteRawBytes(buf);
        }
        else
        {
            Span<byte> buf = stackalloc byte[5];
            buf[0] = Compose(0, 26);
            BinaryPrimitives.WriteUInt32BigEndian(buf[1..], value);
            this.WriteRawBytes(buf);
        }
    }

    public void WriteU64(ulong value)
    {
        if (value <= 23)
        {
            this.WriteRawBytes([Compose(0, (byte)value)]);
        }
        else if (value <= byte.MaxValue)
        {
            Span<byte> buf = stackalloc byte[2];
            buf[0] = Compose(0, 24);
            buf[1] = (byte)value;
            this.WriteRawBytes(buf);
        }
        else if (value <= ushort.MaxValue)
        {
            Span<byte> buf = stackalloc byte[3];
            buf[0] = Compose(0, 25);
            BinaryPrimitives.WriteUInt16BigEndian(buf[1..], (ushort)value);
            this.WriteRawBytes(buf);
        }
        else if (value <= uint.MaxValue)
        {
            Span<byte> buf = stackalloc byte[5];
            buf[0] = Compose(0, 26);
            BinaryPrimitives.WriteUInt32BigEndian(buf[1..], (uint)value);
            this.WriteRawBytes(buf);
        }
        else
        {
            Span<byte> buf = stackalloc byte[9];
            buf[0] = Compose(0, 27);
            BinaryPrimitives.WriteUInt64BigEndian(buf[1..], value);
            this.WriteRawBytes(buf);
        }
    }

    public void WriteI8(sbyte value)
    {
        if (value >= 0)
        {
            this.WriteU8((byte)value);
        }
        else
        {
            var v = (byte)(-1 - value);
            if (v <= 23)
            {
                this.WriteRawBytes([Compose(1, v)]);
            }
            else
            {
                Span<byte> buf = stackalloc byte[2];
                buf[0] = Compose(1, 24);
                buf[1] = (byte)v;
                this.WriteRawBytes(buf);
            }
        }
    }

    public void WriteI16(short value)
    {
        if (value >= 0)
        {
            this.WriteU16((ushort)value);
        }
        else
        {
            var v = (ushort)(-1 - value);
            if (v <= 23)
            {
                this.WriteRawBytes([Compose(1, (byte)v)]);
            }
            else if (v <= byte.MaxValue)
            {
                Span<byte> buf = stackalloc byte[2];
                buf[0] = Compose(1, 24);
                buf[1] = (byte)v;
                this.WriteRawBytes(buf);
            }
            else
            {
                Span<byte> buf = stackalloc byte[3];
                buf[0] = Compose(1, 25);
                BinaryPrimitives.WriteUInt16BigEndian(buf[1..], v);
                this.WriteRawBytes(buf);
            }
        }
    }

    public void WriteI32(int value)
    {
        if (value >= 0)
        {
            this.WriteU32((uint)value);
        }
        else
        {
            var v = (uint)(-1 - value);
            if (v <= 23)
            {
                this.WriteRawBytes([Compose(1, (byte)v)]);
            }
            else if (v <= byte.MaxValue)
            {
                Span<byte> buf = stackalloc byte[2];
                buf[0] = Compose(1, 24);
                buf[1] = (byte)v;
                this.WriteRawBytes(buf);
            }
            else if (v <= ushort.MaxValue)
            {
                Span<byte> buf = stackalloc byte[3];
                buf[0] = Compose(1, 25);
                BinaryPrimitives.WriteUInt16BigEndian(buf[1..], (ushort)v);
                this.WriteRawBytes(buf);
            }
            else
            {
                Span<byte> buf = stackalloc byte[5];
                buf[0] = Compose(1, 26);
                BinaryPrimitives.WriteUInt32BigEndian(buf[1..], v);
                this.WriteRawBytes(buf);
            }
        }
    }

    public void WriteI64(long value)
    {
        if (value >= 0)
        {
            this.WriteU64((ulong)value);
        }
        else
        {
            var v = (ulong)(-1 - value);
            if (v <= 23)
            {
                this.WriteRawBytes([Compose(1, (byte)v)]);
            }
            else if (v <= byte.MaxValue)
            {
                Span<byte> buf = stackalloc byte[2];
                buf[0] = Compose(1, 24);
                buf[1] = (byte)v;
                this.WriteRawBytes(buf);
            }
            else if (v <= ushort.MaxValue)
            {
                Span<byte> buf = stackalloc byte[3];
                buf[0] = Compose(1, 25);
                BinaryPrimitives.WriteUInt16BigEndian(buf[1..], (ushort)v);
                this.WriteRawBytes(buf);
            }
            else if (v <= uint.MaxValue)
            {
                Span<byte> buf = stackalloc byte[5];
                buf[0] = Compose(1, 26);
                BinaryPrimitives.WriteUInt32BigEndian(buf[1..], (uint)v);
                this.WriteRawBytes(buf);
            }
            else
            {
                Span<byte> buf = stackalloc byte[9];
                buf[0] = Compose(1, 27);
                BinaryPrimitives.WriteUInt64BigEndian(buf[1..], v);
                this.WriteRawBytes(buf);
            }
        }
    }

    public void WriteF32(float value)
    {
        Span<byte> buf = stackalloc byte[5];
        buf[0] = Compose(7, 26);
        BinaryPrimitives.WriteSingleBigEndian(buf[1..], value);
        this.WriteRawBytes(buf);
    }

    public void WriteF64(double value)
    {
        Span<byte> buf = stackalloc byte[9];
        buf[0] = Compose(7, 27);
        BinaryPrimitives.WriteDoubleBigEndian(buf[1..], value);
        this.WriteRawBytes(buf);
    }

    public void WriteBytes(ReadOnlySpan<byte> value)
    {
        this.WriteRawLen(2, (ulong)value.Length);
        this.WriteRawBytes(value);
    }

    public void WriteString(string value)
    {
        this.WriteUtf8String(value);
    }

    public void WriteUtf8String(Utf8String value)
    {
        this.WriteRawLen(3, (ulong)value.Length);
        _bufferWriter.Write(value.Span);
    }

    public void WriteArray(ulong length)
    {
        this.WriteRawLen(4, length);
    }

    public void WriteMap(ulong length)
    {
        this.WriteRawLen(5, length);
    }

    public void WriteNull()
    {
        this.WriteRawBytes([Compose(7, 22)]);
    }

    public void WriteStruct<T>(in T value) where T : IRocketPackStruct<T>
    {
        T.Pack(ref this, value);
    }

    private void WriteRawLen(byte major, ulong length)
    {
        if (length <= 23)
        {
            this.WriteRawBytes([Compose(major, (byte)length)]);
        }
        else if (length <= byte.MaxValue)
        {
            Span<byte> buf = stackalloc byte[2];
            buf[0] = Compose(major, 24);
            buf[1] = (byte)length;
            this.WriteRawBytes(buf);
        }
        else if (length <= ushort.MaxValue)
        {
            Span<byte> buf = stackalloc byte[3];
            buf[0] = Compose(major, 25);
            BinaryPrimitives.WriteUInt16BigEndian(buf[1..], (ushort)length);
            this.WriteRawBytes(buf);
        }
        else if (length <= uint.MaxValue)
        {
            Span<byte> buf = stackalloc byte[5];
            buf[0] = Compose(major, 26);
            BinaryPrimitives.WriteUInt32BigEndian(buf[1..], (uint)length);
            this.WriteRawBytes(buf);
        }
        else
        {
            Span<byte> buf = stackalloc byte[9];
            buf[0] = Compose(major, 27);
            BinaryPrimitives.WriteUInt64BigEndian(buf[1..], length);
            this.WriteRawBytes(buf);
        }
    }

    private readonly void WriteRawBytes(scoped ReadOnlySpan<byte> value)
    {
        try
        {
            _bufferWriter.Write(value);
        }
        catch (Exception ex)
        {
            throw RocketPackEncoderException.CreateIoError(ex.Message);
        }
    }
}
