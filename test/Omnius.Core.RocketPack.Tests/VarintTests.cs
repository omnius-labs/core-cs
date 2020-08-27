using System;
using System.Buffers;
using Omnius.Core;
using Xunit;

namespace Omnius.Core.RocketPack
{
    public class VarintTests
    {
        [Fact]
        public void RandomTest()
        {
            var random = new Random();

            using (var hub = new BytesHub())
            {
                for (int i = 0; i < 1024; i++)
                {
                    var result1 = (byte)random.Next();
                    Varint.SetUInt8(result1, hub.Writer);

                    var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                    Varint.TryGetUInt8(ref reader, out var result2);

                    Assert.Equal(result1, result2);

                    hub.Reset();
                }
            }

            using (var hub = new BytesHub())
            {
                for (int i = 0; i < 1024; i++)
                {
                    var result1 = (ushort)random.Next();
                    Varint.SetUInt16(result1, hub.Writer);

                    var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                    Varint.TryGetUInt16(ref reader, out var result2);

                    Assert.Equal(result1, result2);

                    hub.Reset();
                }
            }

            using (var hub = new BytesHub())
            {
                for (int i = 0; i < 1024; i++)
                {
                    var result1 = (uint)random.Next();
                    Varint.SetUInt32(result1, hub.Writer);

                    var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                    Varint.TryGetUInt32(ref reader, out var result2);

                    Assert.Equal(result1, result2);

                    hub.Reset();
                }
            }

            using (var hub = new BytesHub())
            {
                for (int i = 0; i < 1024; i++)
                {
                    var result1 = ((ulong)random.Next() << 32) | (uint)random.Next();
                    Varint.SetUInt64(result1, hub.Writer);

                    var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                    Varint.TryGetUInt64(ref reader, out var result2);

                    Assert.Equal(result1, result2);

                    hub.Reset();
                }
            }

            using (var hub = new BytesHub())
            {
                for (int i = 0; i < 1024; i++)
                {
                    var result1 = (sbyte)random.Next();
                    Varint.SetInt8(result1, hub.Writer);

                    var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                    Varint.TryGetInt8(ref reader, out var result2);

                    Assert.Equal(result1, result2);

                    hub.Reset();
                }
            }

            using (var hub = new BytesHub())
            {
                for (int i = 0; i < 1024; i++)
                {
                    var result1 = (short)random.Next();
                    Varint.SetInt16(result1, hub.Writer);

                    var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                    Varint.TryGetInt16(ref reader, out var result2);

                    Assert.Equal(result1, result2);

                    hub.Reset();
                }
            }

            using (var hub = new BytesHub())
            {
                for (int i = 0; i < 1024; i++)
                {
                    var result1 = (int)random.Next();

                    Varint.SetInt32(result1, hub.Writer);

                    var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                    Varint.TryGetInt32(ref reader, out var result2);

                    Assert.Equal(result1, result2);

                    hub.Reset();
                }
            }

            using (var hub = new BytesHub())
            {
                for (int i = 0; i < 1024; i++)
                {
                    var result1 = ((long)random.Next() << 32) | (uint)random.Next();
                    Varint.SetInt64(result1, hub.Writer);

                    var reader = new SequenceReader<byte>(hub.Reader.GetSequence());
                    Varint.TryGetInt64(ref reader, out var result2);

                    Assert.Equal(result1, result2);

                    hub.Reset();
                }
            }
        }
    }
}
