using System.Buffers;
using Omnius.Core.Pipelines;
using Omnius.Core.Testkit;
using Xunit;

namespace Omnius.Core.RocketPack;

public class VarintTest
{
    const byte Int8Code = 0x80;
    const byte Int16Code = 0x81;
    const byte Int32Code = 0x82;
    const byte Int64Code = 0x83;

    [Fact]
    public void EmptyDataGetTest()
    {
        using var bytesPipe = new BytesPipe();
        var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
        Assert.False(Varint.TryGetUInt8(ref reader, out var _));
        Assert.False(Varint.TryGetInt8(ref reader, out var _));
        Assert.False(Varint.TryGetUInt16(ref reader, out var _));
        Assert.False(Varint.TryGetInt16(ref reader, out var _));
        Assert.False(Varint.TryGetUInt32(ref reader, out var _));
        Assert.False(Varint.TryGetInt32(ref reader, out var _));
        Assert.False(Varint.TryGetUInt64(ref reader, out var _));
        Assert.False(Varint.TryGetInt64(ref reader, out var _));
    }

    [Fact]
    public void BrokenHeaderDataGetTest()
    {
        // 8
        {
            using var bytesPipe = new BytesPipe();
            bytesPipe.Writer.Write(new byte[] { Int16Code });

            {
                var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                Assert.False(Varint.TryGetUInt8(ref reader, out var _));
            }

            {
                var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                Assert.False(Varint.TryGetInt8(ref reader, out var _));
            }
        }

        // 16
        {
            using var bytesPipe = new BytesPipe();
            bytesPipe.Writer.Write(new byte[] { Int32Code });

            {
                var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                Assert.False(Varint.TryGetUInt16(ref reader, out var _));
            }

            {
                var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                Assert.False(Varint.TryGetInt16(ref reader, out var _));
            }
        }

        // 32
        {
            using var bytesPipe = new BytesPipe();
            bytesPipe.Writer.Write(new byte[] { Int64Code });

            {
                var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                Assert.False(Varint.TryGetUInt32(ref reader, out var _));
            }

            {
                var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                Assert.False(Varint.TryGetInt32(ref reader, out var _));
            }
        }

        // 64
        {
            using var bytesPipe = new BytesPipe();
            bytesPipe.Writer.Write(new byte[] { Int64Code + 1 });

            {
                var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                Assert.False(Varint.TryGetUInt64(ref reader, out var _));
            }

            {
                var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                Assert.False(Varint.TryGetInt64(ref reader, out var _));
            }
        }

    }

    [Fact]
    public void BrokenBodyDataGetTest()
    {
        // Int8Code
        {
            using var bytesPipe = new BytesPipe();
            bytesPipe.Writer.Write(new byte[] { Int8Code });

            // 8
            {
                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt8(ref reader, out var _));
                }

                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetInt8(ref reader, out var _));
                }
            }

            // 16
            {
                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt16(ref reader, out var _));
                }

                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetInt16(ref reader, out var _));
                }
            }

            // 32
            {
                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt32(ref reader, out var _));
                }

                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetInt32(ref reader, out var _));
                }
            }

            // 64
            {
                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt64(ref reader, out var _));
                }

                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetInt64(ref reader, out var _));
                }
            }
        }

        // Int16Code
        {
            using var bytesPipe = new BytesPipe();
            bytesPipe.Writer.Write(new byte[] { Int16Code });

            // 16
            {
                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt16(ref reader, out var _));
                }

                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetInt16(ref reader, out var _));
                }
            }

            // 32
            {
                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt32(ref reader, out var _));
                }

                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetInt32(ref reader, out var _));
                }
            }

            // 64
            {
                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt64(ref reader, out var _));
                }

                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetInt64(ref reader, out var _));
                }
            }
        }

        // Int32Code
        {
            using var bytesPipe = new BytesPipe();
            bytesPipe.Writer.Write(new byte[] { Int32Code });

            // 32
            {
                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt32(ref reader, out var _));
                }

                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetInt32(ref reader, out var _));
                }
            }

            // 64
            {
                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt64(ref reader, out var _));
                }

                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetInt64(ref reader, out var _));
                }
            }
        }

        // Int64Code
        {
            using var bytesPipe = new BytesPipe();
            bytesPipe.Writer.Write(new byte[] { Int64Code });

            // 64
            {
                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt64(ref reader, out var _));
                }

                {
                    var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                    Assert.False(Varint.TryGetInt64(ref reader, out var _));
                }
            }
        }
    }

    [Fact]
    public void RandomTest()
    {
        var random = new Random(0);

        using (var bytesPipe = new BytesPipe())
        {
            for (int i = 0; i < 32; i++)
            {
                var result1 = FixtureFactory.GetRandomUInt8();
                Varint.SetUInt8(result1, bytesPipe.Writer);

                var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                Varint.TryGetUInt8(ref reader, out var result2);

                Assert.Equal(result1, result2);

                bytesPipe.Reset();
            }
        }

        using (var bytesPipe = new BytesPipe())
        {
            for (int i = 0; i < 32; i++)
            {
                var result1 = FixtureFactory.GetRandomInt8();
                Varint.SetInt8(result1, bytesPipe.Writer);

                var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                Varint.TryGetInt8(ref reader, out var result2);

                Assert.Equal(result1, result2);

                bytesPipe.Reset();
            }
        }

        using (var bytesPipe = new BytesPipe())
        {
            for (int i = 0; i < 32; i++)
            {
                var result1 = FixtureFactory.GetRandomUInt16();
                Varint.SetUInt16(result1, bytesPipe.Writer);

                var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                Varint.TryGetUInt16(ref reader, out var result2);

                Assert.Equal(result1, result2);

                bytesPipe.Reset();
            }
        }

        using (var bytesPipe = new BytesPipe())
        {
            for (int i = 0; i < 32; i++)
            {
                var result1 = FixtureFactory.GetRandomInt16();
                Varint.SetInt16(result1, bytesPipe.Writer);

                var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                Varint.TryGetInt16(ref reader, out var result2);

                Assert.Equal(result1, result2);

                bytesPipe.Reset();
            }
        }

        using (var bytesPipe = new BytesPipe())
        {
            for (int i = 0; i < 32; i++)
            {
                var result1 = FixtureFactory.GetRandomUInt32();
                Varint.SetUInt32(result1, bytesPipe.Writer);

                var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                Varint.TryGetUInt32(ref reader, out var result2);

                Assert.Equal(result1, result2);

                bytesPipe.Reset();
            }
        }

        using (var bytesPipe = new BytesPipe())
        {
            for (int i = 0; i < 32; i++)
            {
                var result1 = FixtureFactory.GetRandomInt32();
                Varint.SetInt32(result1, bytesPipe.Writer);

                var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                Varint.TryGetInt32(ref reader, out var result2);

                Assert.Equal(result1, result2);

                bytesPipe.Reset();
            }
        }

        using (var bytesPipe = new BytesPipe())
        {
            for (int i = 0; i < 32; i++)
            {
                var result1 = FixtureFactory.GetRandomUInt64();
                Varint.SetUInt64(result1, bytesPipe.Writer);

                var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                Varint.TryGetUInt64(ref reader, out var result2);

                Assert.Equal(result1, result2);

                bytesPipe.Reset();
            }
        }

        using (var bytesPipe = new BytesPipe())
        {
            for (int i = 0; i < 32; i++)
            {
                var result1 = FixtureFactory.GetRandomInt64();
                Varint.SetInt64(result1, bytesPipe.Writer);

                var reader = new SequenceReader<byte>(bytesPipe.Reader.GetSequence());
                Varint.TryGetInt64(ref reader, out var result2);

                Assert.Equal(result1, result2);

                bytesPipe.Reset();
            }
        }
    }
}
