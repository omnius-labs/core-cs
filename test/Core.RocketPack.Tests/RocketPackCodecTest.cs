using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnius.Core.Base;
using Omnius.Core.Pipelines;
using Xunit;

namespace Omnius.Core.RocketPack;

public class RocketPackCodecTest
{
    private delegate void EncodeAction(ref RocketPackBytesEncoder encoder);
    private delegate T DecodeFunc<T>(ref RocketPackBytesDecoder decoder);

    private static byte Compose(byte major, byte info) => (byte)((major << 5) | (info & 0b0001_1111));

    private static byte[] EncodeBytes(EncodeAction action)
    {
        using var pipe = new BytesPipe(BytesPool.Shared);
        var encoder = new RocketPackBytesEncoder(pipe.Writer, BytesPool.Shared);
        action(ref encoder);
        return pipe.Reader.GetSequence().ToArray();
    }

    private static T DecodeBytes<T>(byte[] bytes, DecodeFunc<T> decodeFunc)
    {
        var decoder = new RocketPackBytesDecoder(new ReadOnlySequence<byte>(bytes), BytesPool.Shared);
        return decodeFunc(ref decoder);
    }

    private static byte[] WithPayload(byte header, params byte[] payload)
    {
        var bytes = new byte[1 + payload.Length];
        bytes[0] = header;
        payload.CopyTo(bytes, 1);
        return bytes;
    }

    private static byte[] WithZeros(byte header, int count)
    {
        var bytes = new byte[1 + count];
        bytes[0] = header;
        return bytes;
    }

    [Fact]
    public void NormalBoolTest()
    {
        var cases = new List<(byte[] Bytes, bool Value)>
        {
            (new byte[] { Compose(7, 20) }, false),
            (new byte[] { Compose(7, 21) }, true),
        };

        foreach (var (expected, value) in cases)
        {
            var encoded = EncodeBytes((ref RocketPackBytesEncoder encoder) => encoder.WriteBool(value));
            Assert.Equal(expected, encoded);

            var decoded = DecodeBytes(expected, (ref RocketPackBytesDecoder decoder) => decoder.ReadBool());
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void NormalU8Test()
    {
        var cases = new List<(byte[] Bytes, byte Value)>
        {
            (new byte[] { Compose(0, 0) }, 0),
            (new byte[] { Compose(0, 23) }, 23),
            (new byte[] { Compose(0, 24), (byte)24 }, 24),
            (new byte[] { Compose(0, 24), byte.MaxValue }, byte.MaxValue),
        };

        foreach (var (expected, value) in cases)
        {
            var encoded = EncodeBytes((ref RocketPackBytesEncoder encoder) => encoder.WriteU8(value));
            Assert.Equal(expected, encoded);

            var decoded = DecodeBytes(expected, (ref RocketPackBytesDecoder decoder) => decoder.ReadU8());
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void NormalU16Test()
    {
        var cases = new List<(byte[] Bytes, ushort Value)>
        {
            (new byte[] { Compose(0, 0) }, (ushort)0),
            (new byte[] { Compose(0, 23) }, (ushort)23),
            (new byte[] { Compose(0, 24), (byte)24 }, (ushort)24),
            (new byte[] { Compose(0, 24), byte.MaxValue }, (ushort)byte.MaxValue),
            (new byte[] { Compose(0, 25), byte.MaxValue, byte.MaxValue }, ushort.MaxValue),
        };

        foreach (var (expected, value) in cases)
        {
            var encoded = EncodeBytes((ref RocketPackBytesEncoder encoder) => encoder.WriteU16(value));
            Assert.Equal(expected, encoded);

            var decoded = DecodeBytes(expected, (ref RocketPackBytesDecoder decoder) => decoder.ReadU16());
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void NormalU32Test()
    {
        var cases = new List<(byte[] Bytes, uint Value)>
        {
            (new byte[] { Compose(0, 0) }, 0u),
            (new byte[] { Compose(0, 23) }, 23u),
            (new byte[] { Compose(0, 24), (byte)24 }, 24u),
            (new byte[] { Compose(0, 24), byte.MaxValue }, (uint)byte.MaxValue),
            (new byte[] { Compose(0, 25), byte.MaxValue, byte.MaxValue }, (uint)ushort.MaxValue),
            (new byte[] { Compose(0, 26), byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue }, uint.MaxValue),
        };

        foreach (var (expected, value) in cases)
        {
            var encoded = EncodeBytes((ref RocketPackBytesEncoder encoder) => encoder.WriteU32(value));
            Assert.Equal(expected, encoded);

            var decoded = DecodeBytes(expected, (ref RocketPackBytesDecoder decoder) => decoder.ReadU32());
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void NormalU64Test()
    {
        var cases = new List<(byte[] Bytes, ulong Value)>
        {
            (new byte[] { Compose(0, 0) }, 0ul),
            (new byte[] { Compose(0, 23) }, 23ul),
            (new byte[] { Compose(0, 24), (byte)24 }, 24ul),
            (new byte[] { Compose(0, 24), byte.MaxValue }, (ulong)byte.MaxValue),
            (new byte[] { Compose(0, 25), byte.MaxValue, byte.MaxValue }, (ulong)ushort.MaxValue),
            (new byte[] { Compose(0, 26), byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue }, (ulong)uint.MaxValue),
            (new byte[] { Compose(0, 27), byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue }, ulong.MaxValue),
        };

        foreach (var (expected, value) in cases)
        {
            var encoded = EncodeBytes((ref RocketPackBytesEncoder encoder) => encoder.WriteU64(value));
            Assert.Equal(expected, encoded);

            var decoded = DecodeBytes(expected, (ref RocketPackBytesDecoder decoder) => decoder.ReadU64());
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void NormalI8Test()
    {
        var cases = new List<(byte[] Bytes, sbyte Value)>
        {
            (new byte[] { Compose(0, 0) }, (sbyte)0),
            (new byte[] { Compose(0, 23) }, (sbyte)23),
            (new byte[] { Compose(0, 24), (byte)24 }, (sbyte)24),
            (new byte[] { Compose(0, 24), (byte)sbyte.MaxValue }, sbyte.MaxValue),
            (new byte[] { Compose(1, 0) }, (sbyte)(-1)),
            (new byte[] { Compose(1, 23) }, (sbyte)(-24)),
            (new byte[] { Compose(1, 24), (byte)24 }, (sbyte)(-25)),
            (new byte[] { Compose(1, 24), (byte)sbyte.MaxValue }, sbyte.MinValue),
        };

        foreach (var (expected, value) in cases)
        {
            var encoded = EncodeBytes((ref RocketPackBytesEncoder encoder) => encoder.WriteI8(value));
            Assert.Equal(expected, encoded);

            var decoded = DecodeBytes(expected, (ref RocketPackBytesDecoder decoder) => decoder.ReadI8());
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void NormalI16Test()
    {
        var cases = new List<(byte[] Bytes, short Value)>
        {
            (new byte[] { Compose(0, 0) }, (short)0),
            (new byte[] { Compose(0, 23) }, (short)23),
            (new byte[] { Compose(0, 24), (byte)24 }, (short)24),
            (new byte[] { Compose(0, 24), byte.MaxValue }, (short)byte.MaxValue),
            (new byte[] { Compose(0, 25), 127, byte.MaxValue }, short.MaxValue),
            (new byte[] { Compose(1, 0) }, (short)(-1)),
            (new byte[] { Compose(1, 23) }, (short)(-24)),
            (new byte[] { Compose(1, 24), (byte)24 }, (short)(-25)),
            (new byte[] { Compose(1, 24), byte.MaxValue }, (short)(-((byte.MaxValue) + 1))),
            (new byte[] { Compose(1, 25), 1, 0 }, (short)(-((byte.MaxValue) + 2))),
            (new byte[] { Compose(1, 25), 127, byte.MaxValue }, short.MinValue),
        };

        foreach (var (expected, value) in cases)
        {
            var encoded = EncodeBytes((ref RocketPackBytesEncoder encoder) => encoder.WriteI16(value));
            Assert.Equal(expected, encoded);

            var decoded = DecodeBytes(expected, (ref RocketPackBytesDecoder decoder) => decoder.ReadI16());
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void NormalI32Test()
    {
        var cases = new List<(byte[] Bytes, int Value)>
        {
            (new byte[] { Compose(0, 0) }, 0),
            (new byte[] { Compose(0, 23) }, 23),
            (new byte[] { Compose(0, 24), (byte)24 }, 24),
            (new byte[] { Compose(0, 24), byte.MaxValue }, byte.MaxValue),
            (new byte[] { Compose(0, 25), byte.MaxValue, byte.MaxValue }, ushort.MaxValue),
            (new byte[] { Compose(0, 26), 127, byte.MaxValue, byte.MaxValue, byte.MaxValue }, int.MaxValue),
            (new byte[] { Compose(1, 0) }, -1),
            (new byte[] { Compose(1, 23) }, -24),
            (new byte[] { Compose(1, 24), (byte)24 }, -25),
            (new byte[] { Compose(1, 24), byte.MaxValue }, -(byte.MaxValue + 1)),
            (new byte[] { Compose(1, 25), 1, 0 }, -(byte.MaxValue + 2)),
            (new byte[] { Compose(1, 25), byte.MaxValue, byte.MaxValue }, -(ushort.MaxValue + 1)),
            (new byte[] { Compose(1, 26), 0, 1, 0, 0 }, -(ushort.MaxValue + 2)),
            (new byte[] { Compose(1, 26), 127, byte.MaxValue, byte.MaxValue, byte.MaxValue }, int.MinValue),
        };

        foreach (var (expected, value) in cases)
        {
            var encoded = EncodeBytes((ref RocketPackBytesEncoder encoder) => encoder.WriteI32(value));
            Assert.Equal(expected, encoded);

            var decoded = DecodeBytes(expected, (ref RocketPackBytesDecoder decoder) => decoder.ReadI32());
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void NormalI64Test()
    {
        var cases = new List<(byte[] Bytes, long Value)>
        {
            (new byte[] { Compose(0, 0) }, 0L),
            (new byte[] { Compose(0, 23) }, 23L),
            (new byte[] { Compose(0, 24), (byte)24 }, 24L),
            (new byte[] { Compose(0, 24), byte.MaxValue }, (long)byte.MaxValue),
            (new byte[] { Compose(0, 25), byte.MaxValue, byte.MaxValue }, (long)ushort.MaxValue),
            (new byte[] { Compose(0, 26), byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue }, (long)uint.MaxValue),
            (new byte[] { Compose(0, 27), 127, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue }, long.MaxValue),
            (new byte[] { Compose(1, 0) }, -1L),
            (new byte[] { Compose(1, 23) }, -24L),
            (new byte[] { Compose(1, 24), (byte)24 }, -25L),
            (new byte[] { Compose(1, 24), byte.MaxValue }, -(byte.MaxValue + 1L)),
            (new byte[] { Compose(1, 25), 1, 0 }, -(byte.MaxValue + 2L)),
            (new byte[] { Compose(1, 25), byte.MaxValue, byte.MaxValue }, -(ushort.MaxValue + 1L)),
            (new byte[] { Compose(1, 26), 0, 1, 0, 0 }, -(ushort.MaxValue + 2L)),
            (new byte[] { Compose(1, 26), byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue }, -(uint.MaxValue + 1L)),
            (new byte[] { Compose(1, 27), 0, 0, 0, 1, 0, 0, 0, 0 }, -(uint.MaxValue + 2L)),
            (new byte[] { Compose(1, 27), 127, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue }, long.MinValue),
        };

        foreach (var (expected, value) in cases)
        {
            var encoded = EncodeBytes((ref RocketPackBytesEncoder encoder) => encoder.WriteI64(value));
            Assert.Equal(expected, encoded);

            var decoded = DecodeBytes(expected, (ref RocketPackBytesDecoder decoder) => decoder.ReadI64());
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void NormalF32Test()
    {
        var cases = new List<(byte[] Bytes, float Value)>
        {
            (WithPayload(Compose(7, 26), 0, 0, 0, 0), 0.0f),
        };

        foreach (var (expected, value) in cases)
        {
            var encoded = EncodeBytes((ref RocketPackBytesEncoder encoder) => encoder.WriteF32(value));
            Assert.Equal(expected, encoded);

            var decoded = DecodeBytes(expected, (ref RocketPackBytesDecoder decoder) => decoder.ReadF32());
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void NormalF64Test()
    {
        var cases = new List<(byte[] Bytes, double Value)>
        {
            (WithPayload(Compose(7, 27), 0, 0, 0, 0, 0, 0, 0, 0), 0.0),
        };

        foreach (var (expected, value) in cases)
        {
            var encoded = EncodeBytes((ref RocketPackBytesEncoder encoder) => encoder.WriteF64(value));
            Assert.Equal(expected, encoded);

            var decoded = DecodeBytes(expected, (ref RocketPackBytesDecoder decoder) => decoder.ReadF64());
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void NormalBytesTest()
    {
        var cases = new List<(byte[] Bytes, byte[] Value)>
        {
            (new byte[] { Compose(2, 0) }, Array.Empty<byte>()),
            (WithPayload(Compose(2, 1), 0), new byte[] { 0 }),
            (WithZeros(Compose(2, 23), 23), new byte[23]),
            (WithPayload(Compose(2, 24), 24).Concat(new byte[24]).ToArray(), new byte[24]),
        };

        foreach (var (expected, value) in cases)
        {
            var encoded = EncodeBytes((ref RocketPackBytesEncoder encoder) => encoder.WriteBytes(value));
            Assert.Equal(expected, encoded);

            var decoded = DecodeBytes(expected, (ref RocketPackBytesDecoder decoder) => decoder.ReadBytesToArray());
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void NormalStringTest()
    {
        var cases = new List<(byte[] Bytes, string Value)>
        {
            (new byte[] { Compose(3, 0) }, string.Empty),
            (WithPayload(Compose(3, 6), Encoding.ASCII.GetBytes("AABBCC")), "AABBCC"),
        };

        foreach (var (expected, value) in cases)
        {
            var encoded = EncodeBytes((ref RocketPackBytesEncoder encoder) => encoder.WriteString(value));
            Assert.Equal(expected, encoded);

            var decoded = DecodeBytes(expected, (ref RocketPackBytesDecoder decoder) => decoder.ReadString());
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void NormalArrayTest()
    {
        var cases = new List<(byte[] Bytes, ulong Value)>
        {
            (new byte[] { Compose(4, 1) }, 1ul),
        };

        foreach (var (expected, value) in cases)
        {
            var encoded = EncodeBytes((ref RocketPackBytesEncoder encoder) => encoder.WriteArray(value));
            Assert.Equal(expected, encoded);

            var decoded = DecodeBytes(expected, (ref RocketPackBytesDecoder decoder) => decoder.ReadArray());
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void NormalMapTest()
    {
        var cases = new List<(byte[] Bytes, ulong Value)>
        {
            (new byte[] { Compose(5, 1) }, 1ul),
        };

        foreach (var (expected, value) in cases)
        {
            var encoded = EncodeBytes((ref RocketPackBytesEncoder encoder) => encoder.WriteMap(value));
            Assert.Equal(expected, encoded);

            var decoded = DecodeBytes(expected, (ref RocketPackBytesDecoder decoder) => decoder.ReadMap());
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void NormalDecoderTypeOfTest()
    {
        var cases = new List<(byte[] Bytes, FieldType Type)>
        {
            (new byte[] { Compose(0, 0) }, FieldType.U8),
            (new byte[] { Compose(0, 24) }, FieldType.U8),
            (new byte[] { Compose(0, 25) }, FieldType.U16),
            (new byte[] { Compose(0, 26) }, FieldType.U32),
            (new byte[] { Compose(0, 27) }, FieldType.U64),
            (new byte[] { Compose(1, 0) }, FieldType.U8),
            (new byte[] { Compose(1, 24), 0 }, FieldType.I8),
            (new byte[] { Compose(1, 25), 0, 0 }, FieldType.I16),
            (new byte[] { Compose(1, 26), 0, 0, 0, 0 }, FieldType.I32),
            (WithPayload(Compose(1, 27), 0, 0, 0, 0, 0, 0, 0, 0), FieldType.I64),
            (new byte[] { Compose(1, 24), 0x80 }, FieldType.I16),
            (new byte[] { Compose(1, 25), 0x80, 0x00 }, FieldType.I32),
            (new byte[] { Compose(1, 26), 0x80, 0x00, 0x00, 0x00 }, FieldType.I64),
            (new byte[] { Compose(2, 0) }, FieldType.Bytes),
            (new byte[] { Compose(3, 0) }, FieldType.String),
            (new byte[] { Compose(4, 0) }, FieldType.Array),
            (new byte[] { Compose(5, 0) }, FieldType.Map),
            (new byte[] { Compose(7, 20) }, FieldType.Bool),
            (new byte[] { Compose(7, 21) }, FieldType.Bool),
            (new byte[] { Compose(7, 25) }, FieldType.F16),
            (new byte[] { Compose(7, 26) }, FieldType.F32),
            (new byte[] { Compose(7, 27) }, FieldType.F64),
            (new byte[] { Compose(7, 31) }, FieldType.Unknown(7, 31)),
        };

        foreach (var (bytes, expected) in cases)
        {
            var decoder = new RocketPackBytesDecoder(new ReadOnlySequence<byte>(bytes), BytesPool.Shared);
            Assert.Equal(expected, decoder.CurrentType());
        }
    }

    [Fact]
    public void NormalDecoderSkipFieldTest()
    {
        var buffer = EncodeBytes((ref RocketPackBytesEncoder encoder) =>
        {
            var p0 = true;
            var p1 = (byte)1;
            var p2 = (ushort)2;
            var p3 = (uint)3;
            var p4 = (ulong)4;
            var p5 = (sbyte)5;
            var p6 = (short)6;
            var p7 = 3;
            var p8 = 8L;
            var p9 = 9.5f;
            var p10 = 10.5d;
            var p11 = new byte[] { 0xAA, 0xBB, 0xCC };
            var p12 = "test";
            var p13 = new[] { "test_0", "test_1" };
            var p14 = new SortedDictionary<uint, string>
            {
                { 0u, "test_value_0" },
                { 1u, "test_value_1" },
                { 2u, "test_value_2" },
            };

            encoder.WriteBool(p0);
            encoder.WriteU8(p1);
            encoder.WriteU16(p2);
            encoder.WriteU32(p3);
            encoder.WriteU64(p4);
            encoder.WriteI8(p5);
            encoder.WriteI16(p6);
            encoder.WriteI32(p7);
            encoder.WriteI64(p8);
            encoder.WriteF32(p9);
            encoder.WriteF64(p10);
            encoder.WriteBytes(p11);
            encoder.WriteString(p12);

            encoder.WriteArray((ulong)p13.Length);
            foreach (var v in p13)
            {
                encoder.WriteString(v);
            }

            encoder.WriteMap((ulong)p14.Count);
            foreach (var (key, value) in p14)
            {
                encoder.WriteU32(key);
                encoder.WriteString(value);
            }
        });

        var decoder = new RocketPackBytesDecoder(new ReadOnlySequence<byte>(buffer), BytesPool.Shared);
        for (int i = 0; i <= 14; i++)
        {
            decoder.SkipField();
        }

        Assert.Equal(0, decoder.Remaining);
    }

    [Fact]
    public void TruncatedNegativeNumberReportsEof()
    {
        var bytes = new byte[] { Compose(1, 24) };
        var decoder = new RocketPackBytesDecoder(new ReadOnlySequence<byte>(bytes), BytesPool.Shared);

        RocketPackDecoderException? ex = null;
        try
        {
            decoder.CurrentType();
        }
        catch (RocketPackDecoderException e)
        {
            ex = e;
        }

        Assert.NotNull(ex);
        Assert.Equal(RocketPackDecoderErrorCode.UnexpectedEof, ex!.ErrorCode);
    }
}
