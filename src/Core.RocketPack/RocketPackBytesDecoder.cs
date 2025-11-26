using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;
using Omnius.Core.Base;
using Omnius.Core.RocketPack.Internal;

namespace Omnius.Core.RocketPack;

public sealed class RocketPackDecoderException : Exception
{
    private RocketPackDecoderException(RocketPackDecoderErrorCode errorCode, string message) : base(message)
    {
        this.ErrorCode = errorCode;
    }

    public RocketPackDecoderErrorCode ErrorCode { get; }

    public static RocketPackDecoderException CreateUnexpectedEof(string name)
    {
        return new RocketPackDecoderException(RocketPackDecoderErrorCode.UnexpectedEof, $"unexpected end of {name}");
    }

    public static RocketPackDecoderException CreateMismatchFieldType(long position, FieldType fieldType)
    {
        return new RocketPackDecoderException(RocketPackDecoderErrorCode.MismatchFieldType, $"mismatch field type at position (position: {position}, type: {fieldType})");
    }

    public static RocketPackDecoderException CreateLengthOverflow(long position)
    {
        throw new RocketPackDecoderException(RocketPackDecoderErrorCode.LengthOverflow, $"length overflow (position: {position})");
    }

    public static RocketPackDecoderException CreateInvalidUtf8(long position, DecoderFallbackException ex)
    {
        throw new RocketPackDecoderException(RocketPackDecoderErrorCode.InvalidUtf8, $"string is not valid UTF-8 (position: {position}, error: {ex.Message})");
    }

    public static RocketPackDecoderException CreateOther(string message)
    {
        throw new RocketPackDecoderException(RocketPackDecoderErrorCode.Other, message);
    }
}

public enum RocketPackDecoderErrorCode
{
    UnexpectedEof,
    MismatchFieldType,
    LengthOverflow,
    InvalidUtf8,
    Other,
}

public ref struct RocketPackBytesDecoder
{
    private static readonly Encoding _utf8Encoding = new UTF8Encoding(false, true);

    private SequenceReader<byte> _reader;
    private IBytesPool _bytesPool;

    public RocketPackBytesDecoder(in ReadOnlySequence<byte> sequence, in IBytesPool bytesPool)
    {
        _reader = new SequenceReader<byte>(sequence);
        _bytesPool = bytesPool;
    }

    public long Remaining => (long)_reader.Remaining;

    public long Position => (long)_reader.Consumed;

    public FieldType CurrentType()
    {
        var (major, info) = this.Decompose(this.CurrentRawByte());
        return this.TypeOf(major, info);
    }

    public bool ReadBool()
    {
        var position = this.Position;
        var (major, info) = this.Decompose(this.CurrentRawByte());
        var fieldType = this.TypeOf(major, info);
        this.SkipRawBytes(1);

        return (major, info) switch
        {
            (7, 20) => false,
            (7, 21) => true,
            _ => throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType),
        };
    }

    public byte ReadU8()
    {
        var position = this.Position;
        var (major, info) = this.Decompose(this.CurrentRawByte());
        var fieldType = this.TypeOf(major, info);
        this.SkipRawBytes(1);

        return (major, info) switch
        {
            (0, <= 23) => info,
            (0, 24) => this.ReadRawByte(),
            _ => throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType),
        };
    }

    public ushort ReadU16()
    {
        var position = this.Position;
        var (major, info) = this.Decompose(this.CurrentRawByte());
        var fieldType = this.TypeOf(major, info);
        this.SkipRawBytes(1);

        return (major, info) switch
        {
            (0, <= 23) => info,
            (0, 24) => this.ReadRawByte(),
            (0, 25) => this.ReadUInt16BigEndian(),
            _ => throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType),
        };
    }

    public uint ReadU32()
    {
        var position = this.Position;
        var (major, info) = this.Decompose(this.CurrentRawByte());
        var fieldType = this.TypeOf(major, info);
        this.SkipRawBytes(1);

        return (major, info) switch
        {
            (0, <= 23) => info,
            (0, 24) => this.ReadRawByte(),
            (0, 25) => this.ReadUInt16BigEndian(),
            (0, 26) => this.ReadUInt32BigEndian(),
            _ => throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType),
        };
    }

    public ulong ReadU64()
    {
        var position = this.Position;
        var (major, info) = this.Decompose(this.CurrentRawByte());
        var fieldType = this.TypeOf(major, info);
        this.SkipRawBytes(1);

        return (major, info) switch
        {
            (0, <= 23) => info,
            (0, 24) => this.ReadRawByte(),
            (0, 25) => this.ReadUInt16BigEndian(),
            (0, 26) => this.ReadUInt32BigEndian(),
            (0, 27) => this.ReadUInt64BigEndian(),
            _ => throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType),
        };
    }

    public sbyte ReadI8()
    {
        var position = this.Position;
        var (major, info) = this.Decompose(this.CurrentRawByte());
        var fieldType = this.TypeOf(major, info);
        this.SkipRawBytes(1);

        return (major, info) switch
        {
            (0, <= 23) => (sbyte)info,
            (0, 24) => (sbyte)this.ReadRawByte(),
            (1, <= 23) => (sbyte)(-1 - (sbyte)info),
            (1, >= 24 and <= 28) when (this.CurrentRawByte() & 0x80) != 0x80 => info switch
            {
                24 => (sbyte)(-1 - (sbyte)this.ReadRawByte()),
                _ => throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType),
            },
            _ => throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType),
        };
    }

    public short ReadI16()
    {
        var position = this.Position;
        var (major, info) = this.Decompose(this.CurrentRawByte());
        var fieldType = this.TypeOf(major, info);
        this.SkipRawBytes(1);

        return (major, info) switch
        {
            (0, <= 23) => (short)info,
            (0, 24) => (short)this.ReadRawByte(),
            (0, 25) => (short)this.ReadUInt16BigEndian(),
            (1, <= 23) => (short)(-1 - (short)info),
            (1, >= 24 and <= 28) when (this.CurrentRawByte() & 0x80) != 0x80 => info switch
            {
                24 => (short)(-1 - (short)this.ReadRawByte()),
                25 => (short)(-1 - (short)this.ReadUInt16BigEndian()),
                _ => throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType),
            },
            (1, >= 24 and <= 28) => info switch
            {
                24 => (short)(-1 - (short)this.ReadRawByte()),
                _ => throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType),
            },
            _ => throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType),
        };
    }

    public int ReadI32()
    {
        var position = this.Position;
        var (major, info) = this.Decompose(this.CurrentRawByte());
        var fieldType = this.TypeOf(major, info);
        this.SkipRawBytes(1);

        return (major, info) switch
        {
            (0, <= 23) => (int)info,
            (0, 24) => (int)this.ReadRawByte(),
            (0, 25) => (int)this.ReadUInt16BigEndian(),
            (0, 26) => (int)this.ReadUInt32BigEndian(),
            (1, <= 23) => (int)(-1 - (int)info),
            (1, >= 24 and <= 28) when (this.CurrentRawByte() & 0x80) != 0x80 => info switch
            {
                24 => -1 - (int)this.ReadRawByte(),
                25 => -1 - (int)this.ReadUInt16BigEndian(),
                26 => -1 - (int)this.ReadUInt32BigEndian(),
                _ => throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType),
            },
            (1, >= 24 and <= 28) => info switch
            {
                24 => -1 - (int)this.ReadRawByte(),
                25 => -1 - (int)this.ReadUInt16BigEndian(),
                _ => throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType),
            },
            _ => throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType),
        };
    }

    public long ReadI64()
    {
        var position = this.Position;
        var (major, info) = this.Decompose(this.CurrentRawByte());
        var fieldType = this.TypeOf(major, info);
        this.SkipRawBytes(1);

        return (major, info) switch
        {
            (0, <= 23) => info,
            (0, 24) => (long)this.ReadRawByte(),
            (0, 25) => (long)this.ReadUInt16BigEndian(),
            (0, 26) => (long)this.ReadUInt32BigEndian(),
            (0, 27) => (long)this.ReadUInt64BigEndian(),
            (1, <= 23) => -1 - info,
            (1, >= 24 and <= 28) when (this.CurrentRawByte() & 0x80) != 0x80 => info switch
            {
                24 => -1 - (long)this.ReadRawByte(),
                25 => -1 - (long)this.ReadUInt16BigEndian(),
                26 => -1 - (long)this.ReadUInt32BigEndian(),
                27 => -1 - (long)this.ReadUInt64BigEndian(),
                _ => throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType),
            },
            (1, >= 24 and <= 28) => info switch
            {
                24 => -1 - (long)this.ReadRawByte(),
                25 => -1 - (long)this.ReadUInt16BigEndian(),
                26 => -1 - (long)this.ReadUInt32BigEndian(),
                _ => throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType),
            },
            _ => throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType),
        };
    }

    public float ReadF32()
    {
        var position = this.Position;
        var (major, info) = this.Decompose(this.CurrentRawByte());
        var fieldType = this.TypeOf(major, info);
        this.SkipRawBytes(1);

        if ((major, info) != (7, 26))
        {
            throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType);
        }

        Span<byte> buf = stackalloc byte[4];
        this.ReadRawBytes(buf);
        return new Float32Bits(buf).Value;
    }

    public double ReadF64()
    {
        var position = this.Position;
        var (major, info) = this.Decompose(this.CurrentRawByte());
        var fieldType = this.TypeOf(major, info);
        this.SkipRawBytes(1);

        if ((major, info) != (7, 27))
        {
            throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType);
        }

        Span<byte> buf = stackalloc byte[8];
        this.ReadRawBytes(buf);
        return new Float64Bits(buf).Value;
    }

    public IMemoryOwner<byte> ReadBytes()
    {
        var position = this.Position;
        var (major, info) = this.Decompose(this.CurrentRawByte());
        var fieldType = this.TypeOf(major, info);
        this.SkipRawBytes(1);

        if (major != 2)
        {
            throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType);
        }

        var len = this.ReadRawLen(info);
        if (len is null)
        {
            throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType);
        }

        if (len > int.MaxValue)
        {
            throw RocketPackDecoderException.CreateLengthOverflow(position);
        }

        int length = (int)len;
        var memoryOwner = _bytesPool.Memory.Rent(length);
        var span = memoryOwner.Memory.Span[..length];
        this.ReadRawBytes(span);
        return memoryOwner.Shrink(length);
    }

    public byte[] ReadBytesToArray()
    {
        using var owner = this.ReadBytes();
        return owner.Memory.Span.ToArray();
    }

    public string ReadString()
    {
        var position = this.Position;
        var (major, info) = this.Decompose(this.CurrentRawByte());
        var fieldType = this.TypeOf(major, info);
        this.SkipRawBytes(1);

        if (major != 3)
        {
            throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType);
        }

        var len = this.ReadRawLen(info);
        if (len is null)
        {
            throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType);
        }

        if (len > int.MaxValue)
        {
            throw RocketPackDecoderException.CreateLengthOverflow(position);
        }

        int length = (int)len;
        var memoryOwner = _bytesPool.Memory.Rent(length);
        try
        {
            var span = memoryOwner.Memory.Span[..length];
            this.ReadRawBytes(span);
            return _utf8Encoding.GetString(span);
        }
        catch (DecoderFallbackException ex)
        {
            throw RocketPackDecoderException.CreateInvalidUtf8(position, ex);
        }
        finally
        {
            memoryOwner.Dispose();
        }
    }

    public ulong ReadArray()
    {
        var position = this.Position;
        var (major, info) = this.Decompose(this.CurrentRawByte());
        var fieldType = this.TypeOf(major, info);
        this.SkipRawBytes(1);

        if (major != 4)
        {
            throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType);
        }

        var len = this.ReadRawLen(info);
        if (len is null)
        {
            throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType);
        }

        return len.Value;
    }

    public ulong ReadMap()
    {
        var position = this.Position;
        var (major, info) = this.Decompose(this.CurrentRawByte());
        var fieldType = this.TypeOf(major, info);
        this.SkipRawBytes(1);

        if (major != 5)
        {
            throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType);
        }

        var len = this.ReadRawLen(info);
        if (len is null)
        {
            throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType);
        }

        return len.Value;
    }

    public void ReadNull()
    {
        var position = this.Position;
        var (major, info) = this.Decompose(this.CurrentRawByte());
        var fieldType = this.TypeOf(major, info);
        this.SkipRawBytes(1);

        _ = (major, info) switch
        {
            (7, 22) => 0,
            _ => throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType),
        };
    }

    public T ReadStruct<T>() where T : IRocketPackStruct<T>
    {
        return T.Unpack(ref this);
    }

    public void SkipField()
    {
        ulong remain = 1;

        while (remain > 0)
        {
            var position = this.Position;
            var (major, info) = this.Decompose(this.CurrentRawByte());
            var fieldType = this.TypeOf(major, info);
            this.SkipRawBytes(1);

            ulong? len = major switch
            {
                0 or 1 => info switch
                {
                    <= 23 => 0,
                    24 => 1,
                    25 => 2,
                    26 => 4,
                    27 => 8,
                    28 => 16,
                    _ => null,
                },
                2 or 3 => this.ReadRawLen(info),
                4 => this.ReadRawLen(info),
                5 => this.ReadRawLen(info) is ulong count ? count : null,
                7 => info switch
                {
                    20 or 21 => 0,
                    25 => 2,
                    26 => 4,
                    27 => 8,
                    _ => null,
                },
                _ => null,
            };

            if (major == 4 && len is ulong arrayCount)
            {
                remain = checked(remain + arrayCount);
                len = 0;
            }
            else if (major == 5 && len is ulong mapCount)
            {
                remain = checked(remain + checked(mapCount * 2));
                len = 0;
            }

            if (len is null)
            {
                throw RocketPackDecoderException.CreateMismatchFieldType(position, fieldType);
            }

            if (len > int.MaxValue)
            {
                throw RocketPackDecoderException.CreateLengthOverflow(position);
            }

            this.SkipRawBytes((ulong)len);
            remain--;
        }
    }

    private bool IsEof()
    {
        return _reader.End;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (byte major, byte info) Decompose(byte value)
    {
        var major = (byte)(value >> 5);
        var info = (byte)(value & 0b0001_1111);
        return (major, info);
    }

    private FieldType TypeOf(byte major, byte info)
    {
        switch (major, info)
        {
            case (0, <= 23):
            case (0, 24):
                return FieldType.U8;
            case (0, 25):
                return FieldType.U16;
            case (0, 26):
                return FieldType.U32;
            case (0, 27):
                return FieldType.U64;
            case (1, <= 23):
                return FieldType.U8;
            case (1, >= 24 and <= 28):
                {
                    // Determine minimal signed integer size by peeking first payload byte.
                    var peek = this.PeekRawByte();
                    if ((peek & 0x80) != 0x80)
                    {
                        return info switch
                        {
                            24 => FieldType.I8,
                            25 => FieldType.I16,
                            26 => FieldType.I32,
                            27 => FieldType.I64,
                            _ => FieldType.Unknown(major, info),
                        };
                    }
                    else
                    {
                        return info switch
                        {
                            24 => FieldType.I16,
                            25 => FieldType.I32,
                            26 => FieldType.I64,
                            _ => FieldType.Unknown(major, info),
                        };
                    }
                }
            case (2, _):
                return FieldType.Bytes;
            case (3, _):
                return FieldType.String;
            case (4, _):
                return FieldType.Array;
            case (5, _):
                return FieldType.Map;
            case (7, >= 20 and <= 21):
                return FieldType.Bool;
            case (7, 25):
                return FieldType.F16;
            case (7, 26):
                return FieldType.F32;
            case (7, 27):
                return FieldType.F64;
            default:
                return FieldType.Unknown(major, info);
        }
    }

    private ulong? ReadRawLen(byte info)
    {
        return info switch
        {
            <= 23 => info,
            24 => this.ReadRawByte(),
            25 => this.ReadUInt16BigEndian(),
            26 => this.ReadUInt32BigEndian(),
            27 => this.ReadUInt64BigEndian(),
            _ => null,
        };
    }

    private byte CurrentRawByte()
    {
        if (_reader.Remaining < 1)
        {
            throw RocketPackDecoderException.CreateUnexpectedEof("buffer");
        }

        if (!_reader.TryPeek(out var value))
        {
            throw RocketPackDecoderException.CreateUnexpectedEof("buffer");
        }

        return value;
    }

    private byte PeekRawByte()
    {
        if (_reader.Remaining < 2)
        {
            throw RocketPackDecoderException.CreateUnexpectedEof("buffer");
        }

        Span<byte> buf = stackalloc byte[2];
        if (!_reader.TryCopyTo(buf))
        {
            throw RocketPackDecoderException.CreateUnexpectedEof("buffer");
        }

        return buf[1];
    }

    private byte ReadRawByte()
    {
        if (!_reader.TryRead(out var value))
        {
            throw RocketPackDecoderException.CreateUnexpectedEof("buffer");
        }

        return value;
    }

    private ushort ReadUInt16BigEndian()
    {
        Span<byte> buf = stackalloc byte[2];
        if (!_reader.TryCopyTo(buf))
        {
            throw RocketPackDecoderException.CreateUnexpectedEof("buffer");
        }

        _reader.Advance(buf.Length);
        return BinaryPrimitives.ReadUInt16BigEndian(buf);
    }

    private uint ReadUInt32BigEndian()
    {
        Span<byte> buf = stackalloc byte[4];
        if (!_reader.TryCopyTo(buf))
        {
            throw RocketPackDecoderException.CreateUnexpectedEof("buffer");
        }

        _reader.Advance(buf.Length);
        return BinaryPrimitives.ReadUInt32BigEndian(buf);
    }

    private ulong ReadUInt64BigEndian()
    {
        Span<byte> buf = stackalloc byte[8];
        if (!_reader.TryCopyTo(buf))
        {
            throw RocketPackDecoderException.CreateUnexpectedEof("buffer");
        }

        _reader.Advance(buf.Length);
        return BinaryPrimitives.ReadUInt64BigEndian(buf);
    }

    private void ReadRawBytes(scoped Span<byte> destination)
    {
        if (!_reader.TryCopyTo(destination))
        {
            throw RocketPackDecoderException.CreateUnexpectedEof("buffer");
        }

        _reader.Advance(destination.Length);
    }

    private void SkipRawBytes(ulong length)
    {
        if (_reader.Remaining < (long)length)
        {
            throw RocketPackDecoderException.CreateUnexpectedEof("buffer");
        }

        _reader.Advance((long)length);
    }
}
