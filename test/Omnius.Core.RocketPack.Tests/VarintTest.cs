using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Omnius.Core.RocketPack
{
    public class VarintTest
    {
        [Fact]
        public void BrokenDataGetTest()
        {
            const byte Int8Code = 0x80;
            const byte Int16Code = 0x81;
            const byte Int32Code = 0x82;
            const byte Int64Code = 0x83;

            {
                using var hub = new BytesHub();
                hub.Writer.Write(new byte[] { Int8Code });
                var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                Assert.False(Varint.TryGetUInt8(ref reader, out var result1));
            }

            {
                {
                    using var hub = new BytesHub();
                    hub.Writer.Write(new byte[] { Int8Code });
                    var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt16(ref reader, out var result1));
                }
                {
                    using var hub = new BytesHub();
                    hub.Writer.Write(new byte[] { Int16Code });
                    var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt16(ref reader, out var result1));
                }
            }

            {
                {
                    using var hub = new BytesHub();
                    hub.Writer.Write(new byte[] { Int8Code });
                    var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt32(ref reader, out var result1));
                }
                {
                    using var hub = new BytesHub();
                    hub.Writer.Write(new byte[] { Int16Code });
                    var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt32(ref reader, out var result1));
                }
                {
                    using var hub = new BytesHub();
                    hub.Writer.Write(new byte[] { Int32Code });
                    var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt32(ref reader, out var result1));
                }
            }

            {
                {
                    using var hub = new BytesHub();
                    hub.Writer.Write(new byte[] { Int8Code });
                    var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt64(ref reader, out var result1));
                }
                {
                    using var hub = new BytesHub();
                    hub.Writer.Write(new byte[] { Int16Code });
                    var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt64(ref reader, out var result1));
                }
                {
                    using var hub = new BytesHub();
                    hub.Writer.Write(new byte[] { Int32Code });
                    var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt64(ref reader, out var result1));
                }
                {
                    using var hub = new BytesHub();
                    hub.Writer.Write(new byte[] { Int64Code });
                    var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                    Assert.False(Varint.TryGetUInt64(ref reader, out var result1));
                }
            }
        }


        [Fact]
        public void EmptyDataGetTest()
        {
            using var hub = new BytesHub();
            var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
            Assert.False(Varint.TryGetUInt8(ref reader, out var result1));
            Assert.False(Varint.TryGetUInt16(ref reader, out var result2));
            Assert.False(Varint.TryGetUInt32(ref reader, out var result3));
            Assert.False(Varint.TryGetUInt64(ref reader, out var result4));
            Assert.False(Varint.TryGetInt8(ref reader, out var result5));
            Assert.False(Varint.TryGetInt16(ref reader, out var result6));
            Assert.False(Varint.TryGetInt32(ref reader, out var result7));
            Assert.False(Varint.TryGetInt64(ref reader, out var result8));
        }

        [Fact]
        public void RandomTest()
        {
            var random = new Random(0);

            using (var hub = new BytesHub())
            {
                for (int i = 0; i < 32; i++)
                {
                    foreach (var result1 in GenRandomValue(random, 8).Select(n => (byte)n))
                    {
                        Varint.SetUInt8(result1, hub.Writer);

                        var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                        Varint.TryGetUInt8(ref reader, out var result2);

                        Assert.Equal(result1, result2);

                        hub.Reset();
                    }
                }
            }

            using (var hub = new BytesHub())
            {
                for (int i = 0; i < 32; i++)
                {
                    foreach (var result1 in GenRandomValue(random, 16).Select(n => (ushort)n))
                    {
                        Varint.SetUInt16(result1, hub.Writer);

                        var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                        Varint.TryGetUInt16(ref reader, out var result2);

                        Assert.Equal(result1, result2);

                        hub.Reset();
                    }
                }
            }

            using (var hub = new BytesHub())
            {
                for (int i = 0; i < 32; i++)
                {
                    foreach (var result1 in GenRandomValue(random, 32).Select(n => (uint)n))
                    {
                        Varint.SetUInt32(result1, hub.Writer);

                        var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                        Varint.TryGetUInt32(ref reader, out var result2);

                        Assert.Equal(result1, result2);

                        hub.Reset();
                    }
                }
            }

            using (var hub = new BytesHub())
            {
                for (int i = 0; i < 32; i++)
                {
                    foreach (var result1 in GenRandomValue(random, 64).Select(n => (ulong)n))
                    {
                        Varint.SetUInt64(result1, hub.Writer);

                        var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                        Varint.TryGetUInt64(ref reader, out var result2);

                        Assert.Equal(result1, result2);

                        hub.Reset();
                    }
                }
            }

            using (var hub = new BytesHub())
            {
                for (int i = 0; i < 32; i++)
                {
                    foreach (var result1 in GenRandomValue(random, 8).Select(n => (sbyte)n))
                    {
                        Varint.SetInt8(result1, hub.Writer);

                        var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                        Varint.TryGetInt8(ref reader, out var result2);

                        Assert.Equal(result1, result2);

                        hub.Reset();
                    }
                }
            }

            using (var hub = new BytesHub())
            {
                for (int i = 0; i < 32; i++)
                {
                    foreach (var result1 in GenRandomValue(random, 16).Select(n => (short)n))
                    {
                        Varint.SetInt16(result1, hub.Writer);

                        var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                        Varint.TryGetInt16(ref reader, out var result2);

                        Assert.Equal(result1, result2);

                        hub.Reset();
                    }
                }
            }

            using (var hub = new BytesHub())
            {
                for (int i = 0; i < 32; i++)
                {
                    foreach (var result1 in GenRandomValue(random, 32).Select(n => (int)n))
                    {
                        Varint.SetInt32(result1, hub.Writer);

                        var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                        Varint.TryGetInt32(ref reader, out var result2);

                        Assert.Equal(result1, result2);

                        hub.Reset();
                    }
                }
            }

            using (var hub = new BytesHub())
            {
                for (int i = 0; i < 32; i++)
                {
                    foreach (var result1 in GenRandomValue(random, 64).Select(n => (long)n))
                    {
                        Varint.SetInt64(result1, hub.Writer);

                        var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                        Varint.TryGetInt64(ref reader, out var result2);

                        Assert.Equal(result1, result2);

                        hub.Reset();
                    }
                }
            }
        }

        private static ulong[] GenRandomValue(Random random, int bits)
        {
            var results = new List<ulong>();

            var v = ((ulong)random.Next() << 32) | (uint)random.Next();

            for (int i = 64 - bits; i < 64; i++)
            {
                results.Add(v >> (i - 1));
            }

            return results.ToArray();
        }
    }
}
