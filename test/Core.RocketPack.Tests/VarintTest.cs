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
        // 8
        {
            AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.EndOfInput, e.ErrorCode), () =>
            {
                using var bytesPipe = new BytesPipe();
                var sequence = bytesPipe.Reader.GetSequence();
                Varint.GetUInt8(ref sequence);
            });
            AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.EndOfInput, e.ErrorCode), () =>
            {
                using var bytesPipe = new BytesPipe();
                var sequence = bytesPipe.Reader.GetSequence();
                Varint.GetInt8(ref sequence);
            });
        }

        // 16
        {
            AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.EndOfInput, e.ErrorCode), () =>
            {
                using var bytesPipe = new BytesPipe();
                var sequence = bytesPipe.Reader.GetSequence();
                Varint.GetUInt16(ref sequence);
            });
            AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.EndOfInput, e.ErrorCode), () =>
            {
                using var bytesPipe = new BytesPipe();
                var sequence = bytesPipe.Reader.GetSequence();
                Varint.GetInt16(ref sequence);
            });
        }

        // 32
        {
            AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.EndOfInput, e.ErrorCode), () =>
            {
                using var bytesPipe = new BytesPipe();
                var sequence = bytesPipe.Reader.GetSequence();
                Varint.GetUInt32(ref sequence);
            });
            AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.EndOfInput, e.ErrorCode), () =>
            {
                using var bytesPipe = new BytesPipe();
                var sequence = bytesPipe.Reader.GetSequence();
                Varint.GetInt32(ref sequence);
            });
        }

        // 64
        {
            AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.EndOfInput, e.ErrorCode), () =>
            {
                using var bytesPipe = new BytesPipe();
                var sequence = bytesPipe.Reader.GetSequence();
                Varint.GetUInt64(ref sequence);
            });
            AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.EndOfInput, e.ErrorCode), () =>
            {
                using var bytesPipe = new BytesPipe();
                var sequence = bytesPipe.Reader.GetSequence();
                Varint.GetInt64(ref sequence);
            });
        }
    }

    [Fact]
    public void BrokenHeaderDataGetTest()
    {
        // 8
        {
            AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.InvalidHeader, e.ErrorCode), () =>
            {
                using var bytesPipe = new BytesPipe();
                bytesPipe.Writer.Write([Int16Code]);
                var sequence = bytesPipe.Reader.GetSequence();
                Varint.GetUInt8(ref sequence);
            });
            AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.InvalidHeader, e.ErrorCode), () =>
            {
                using var bytesPipe = new BytesPipe();
                bytesPipe.Writer.Write([Int16Code]);
                var sequence = bytesPipe.Reader.GetSequence();
                Varint.GetInt8(ref sequence);
            });
        }

        // 16
        {
            AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.InvalidHeader, e.ErrorCode), () =>
            {
                using var bytesPipe = new BytesPipe();
                bytesPipe.Writer.Write([Int32Code]);
                var sequence = bytesPipe.Reader.GetSequence();
                Varint.GetUInt16(ref sequence);
            });
            AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.InvalidHeader, e.ErrorCode), () =>
            {
                using var bytesPipe = new BytesPipe();
                bytesPipe.Writer.Write([Int32Code]);
                var sequence = bytesPipe.Reader.GetSequence();
                Varint.GetInt16(ref sequence);
            });
        }

        // 32
        {
            AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.InvalidHeader, e.ErrorCode), () =>
            {
                using var bytesPipe = new BytesPipe();
                bytesPipe.Writer.Write([Int64Code]);
                var sequence = bytesPipe.Reader.GetSequence();
                Varint.GetUInt32(ref sequence);
            });
            AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.InvalidHeader, e.ErrorCode), () =>
            {
                using var bytesPipe = new BytesPipe();
                bytesPipe.Writer.Write([Int64Code]);
                var sequence = bytesPipe.Reader.GetSequence();
                Varint.GetInt32(ref sequence);
            });
        }

        // 64
        {
            AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.InvalidHeader, e.ErrorCode), () =>
            {
                using var bytesPipe = new BytesPipe();
                bytesPipe.Writer.Write([(byte)(Int64Code + 1)]);
                var sequence = bytesPipe.Reader.GetSequence();
                Varint.GetUInt64(ref sequence);
            });
            AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.InvalidHeader, e.ErrorCode), () =>
            {
                using var bytesPipe = new BytesPipe();
                bytesPipe.Writer.Write([(byte)(Int64Code + 1)]);
                var sequence = bytesPipe.Reader.GetSequence();
                Varint.GetInt64(ref sequence);
            });
        }
    }

    [Fact]
    public void BrokenBodyDataGetTest()
    {
        // Int8Code
        {
            // 8
            {
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int8Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetUInt8(ref sequence);
                });
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int8Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetInt8(ref sequence);
                });
            }

            // 16
            {
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int8Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetUInt16(ref sequence);
                });
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int8Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetInt16(ref sequence);
                });
            }

            // 32
            {
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int8Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetUInt32(ref sequence);
                });
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int8Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetInt32(ref sequence);
                });
            }

            // 64
            {
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int8Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetUInt64(ref sequence);
                });
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int8Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetInt64(ref sequence);
                });
            }
        }

        // Int16Code
        {
            // 16
            {
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int16Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetUInt16(ref sequence);
                });
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int16Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetInt16(ref sequence);
                });
            }

            // 32
            {
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int16Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetUInt32(ref sequence);
                });
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int16Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetInt32(ref sequence);
                });
            }

            // 64
            {
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int16Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetUInt64(ref sequence);
                });
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int16Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetInt64(ref sequence);
                });
            }
        }

        // Int32Code
        {
            // 32
            {
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int32Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetUInt32(ref sequence);
                });
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int32Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetInt32(ref sequence);
                });
            }

            // 64
            {
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int32Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetUInt64(ref sequence);
                });
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int32Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetInt64(ref sequence);
                });
            }
        }

        // Int64Code
        {
            // 64
            {
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int64Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetUInt64(ref sequence);
                });
                AssertEx.Throws<VarintException>((e) => Assert.Equal(VarintErrorCode.TooSmallBody, e.ErrorCode), () =>
                {
                    using var bytesPipe = new BytesPipe();
                    bytesPipe.Writer.Write([Int64Code]);
                    var sequence = bytesPipe.Reader.GetSequence();
                    Varint.GetInt64(ref sequence);
                });
            }
        }
    }

    [Fact]
    public void RandomTest()
    {
        var fixtureFactory = new FixtureFactory(3);

        using (var bytesPipe = new BytesPipe())
        {
            for (int i = 0; i < 32; i++)
            {
                var result1 = fixtureFactory.GenRandomUInt8();
                Varint.PutUInt8(result1, bytesPipe.Writer);

                var sequence = bytesPipe.Reader.GetSequence();
                var result2 = Varint.GetUInt8(ref sequence);

                Assert.Equal(result1, result2);

                bytesPipe.Reset();
            }
        }

        using (var bytesPipe = new BytesPipe())
        {
            for (int i = 0; i < 32; i++)
            {
                var result1 = fixtureFactory.GenRandomInt8();
                Varint.PutInt8(result1, bytesPipe.Writer);

                var sequence = bytesPipe.Reader.GetSequence();
                var result2 = Varint.GetInt8(ref sequence);

                Assert.Equal(result1, result2);

                bytesPipe.Reset();
            }
        }

        using (var bytesPipe = new BytesPipe())
        {
            for (int i = 0; i < 32; i++)
            {
                var result1 = fixtureFactory.GenRandomUInt16();
                Varint.PutUInt16(result1, bytesPipe.Writer);

                var sequence = bytesPipe.Reader.GetSequence();
                var result2 = Varint.GetUInt16(ref sequence);

                Assert.Equal(result1, result2);

                bytesPipe.Reset();
            }
        }

        using (var bytesPipe = new BytesPipe())
        {
            for (int i = 0; i < 32; i++)
            {
                var result1 = fixtureFactory.GenRandomInt16();
                Varint.PutInt16(result1, bytesPipe.Writer);

                var sequence = bytesPipe.Reader.GetSequence();
                var result2 = Varint.GetInt16(ref sequence);

                Assert.Equal(result1, result2);

                bytesPipe.Reset();
            }
        }

        using (var bytesPipe = new BytesPipe())
        {
            for (int i = 0; i < 32; i++)
            {
                var result1 = fixtureFactory.GenRandomUInt32();
                Varint.PutUInt32(result1, bytesPipe.Writer);

                var sequence = bytesPipe.Reader.GetSequence();
                var result2 = Varint.GetUInt32(ref sequence);

                Assert.Equal(result1, result2);

                bytesPipe.Reset();
            }
        }

        using (var bytesPipe = new BytesPipe())
        {
            for (int i = 0; i < 32; i++)
            {
                var result1 = fixtureFactory.GenRandomInt32();
                Varint.PutInt32(result1, bytesPipe.Writer);

                var sequence = bytesPipe.Reader.GetSequence();
                var result2 = Varint.GetInt32(ref sequence);

                Assert.Equal(result1, result2);

                bytesPipe.Reset();
            }
        }

        using (var bytesPipe = new BytesPipe())
        {
            for (int i = 0; i < 32; i++)
            {
                var result1 = fixtureFactory.GenRandomUInt64();
                Varint.PutUInt64(result1, bytesPipe.Writer);

                var sequence = bytesPipe.Reader.GetSequence();
                var result2 = Varint.GetUInt64(ref sequence);

                Assert.Equal(result1, result2);

                bytesPipe.Reset();
            }
        }

        using (var bytesPipe = new BytesPipe())
        {
            for (int i = 0; i < 32; i++)
            {
                var result1 = fixtureFactory.GenRandomInt64();
                Varint.PutInt64(result1, bytesPipe.Writer);

                var sequence = bytesPipe.Reader.GetSequence();
                var result2 = Varint.GetInt64(ref sequence);

                Assert.Equal(result1, result2);

                bytesPipe.Reset();
            }
        }
    }
}
