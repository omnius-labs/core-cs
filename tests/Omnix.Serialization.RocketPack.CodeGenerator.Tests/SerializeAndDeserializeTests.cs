using System;
using Xunit;
using Omnix.Serialization.RocketPack.CodeGenerator;
using Omnix.Base;
using System.Buffers;
using System.Collections.Generic;

namespace Omnix.Serialization.RocketPack.CodeGenerator.Tests
{
    public class SerializeAndDeserializeTests
    {
        [Fact]
        public void ClassTest()
        {
            bool x0 = true;
            sbyte x1 = 1;
            short x2 = 1;
            int x3 = 1;
            long x4 = 1;
            byte x5 = 1;
            ushort x6 = 1;
            uint x7 = 1;
            ulong x8 = 1;
            Enum1 x9 = Enum1.Yes;
            Enum2 x10 = Enum2.Yes;
            Enum3 x11 = Enum3.Yes;
            Enum4 x12 = Enum4.Yes;
            Enum5 x13 = Enum5.Yes;
            Enum6 x14 = Enum6.Yes;
            Enum7 x15 = Enum7.Yes;
            Enum8 x16 = Enum8.Yes;
            float x17 = 1;
            double x18 = 1;
            string x19 = "1";
            Timestamp x20 = Timestamp.FromDateTime(new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc));
            ReadOnlyMemory<byte> x21 = new byte[] { 1 };
            IMemoryOwner<byte> x22 = BufferPool.Shared.Rent(1);
            x22.Memory.Span[0] = 1;
            IList<string> x23 = new string[] { "1" };
            IDictionary<byte, string> x24 = new Dictionary<byte, string>() { { 1, "1" } };
            var message = new HelloMessage(x0, x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18, x19, x20, x21, x22, x23, x24);

            var hub = new Hub();
            message.Export(hub.Writer, BufferPool.Shared);
            hub.Writer.Complete();
            var message2 = HelloMessage.Import(hub.Reader.GetSequence(), BufferPool.Shared);
            hub.Reader.Complete();
            hub.Reset();

            Assert.True(message == message2);
        }
    }
}

