using System;
using Xunit;
using Omnix.Serialization.RocketPack.CodeGenerator;
using Omnix.Base;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Omnix.Serialization.Extensions;
using System.Security.Cryptography;
using Omnix.Cryptography;
using Omnix.Serialization.RocketPack.CodeGenerator.Tests.Internal;

namespace Omnix.Serialization.RocketPack.CodeGenerator.Tests
{
    public class SerializeAndDeserializeTests
    {
        [Fact]
        public void HelloMessageTest()
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
            string[] x23 = new string[] { "1" };
            IDictionary<byte, string> x24 = new Dictionary<byte, string>() { { 1, "1" } };

            using (var hub = new Hub())
            {
                var message = new HelloMessage(x0, x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18, x19, x20, x21, x22, x23, x24);
                message.Export(hub.Writer, BufferPool.Shared);
                hub.Writer.Complete();
                var message2 = HelloMessage.Import(hub.Reader.GetSequence(), BufferPool.Shared);
                hub.Reader.Complete();

                Assert.True(message == message2);
            }
        }

        [Fact]
        public void IntDeserializeTest()
        {
            IntPropertiesListMessage list1, list2;

            var random = new Random(0);

            for (int count = 0; count < 1024; count++)
            {
                using (var hub = new Hub())
                {
                    var items = new List<IntPropertiesMessage>();
                    for (int i = 0; i < 1000; i++)
                    {
                        var message = new IntPropertiesMessage(
                                (uint)random.Next(),
                                (uint)random.Next(),
                                (uint)random.Next(),
                                (uint)random.Next(),
                                (uint)random.Next(),
                                (uint)random.Next(),
                                (uint)random.Next(),
                                (uint)random.Next(),
                                (uint)random.Next());

                        items.Add(message);
                    }

                    list1 = new IntPropertiesListMessage(items.ToArray());

                    list1.Export(hub.Writer, BufferPool.Shared);
                    hub.Writer.Complete();

                    var list1_bytes = new byte[hub.Writer.BytesWritten];
                    hub.Reader.GetSequence().CopyTo(list1_bytes);
                    hub.Reader.Complete();

                    list2 = IntPropertiesListMessage.Import(new ReadOnlySequence<byte>(list1_bytes), BufferPool.Shared);

                    Assert.Equal(list1, list2);
                }
            }
        }

        [Fact]
        public void StringDeserializeTest()
        {
            var charList = new char[] { 'A', 'B', 'C', 'D', 'E', '安', '以', '宇', '衣', '於' };

            var random = new Random(0);

            string GetRandomString()
            {
                var sb = new StringBuilder();

                for (int i = random.Next(32, 256) - 1; i >= 0; i--)
                {
                    sb.Append(charList[random.Next(0, charList.Length)]);
                }

                return sb.ToString();
            }

            for (int count = 0; count < 1024; count++)
            {
                using (var hub = new Hub())
                {
                    StringPropertiesListMessage list1, list2;

                    var message = new StringPropertiesMessage(GetRandomString(), GetRandomString(), GetRandomString());

                    var items = new List<StringPropertiesMessage>();
                    for (int i = 0; i < 1000; i++)
                    {
                        items.Add(message);
                    }

                    list1 = new StringPropertiesListMessage(items.ToArray());

                    list1.Export(hub.Writer, BufferPool.Shared);
                    hub.Writer.Complete();

                    var list1_bytes = new byte[hub.Writer.BytesWritten];
                    hub.Reader.GetSequence().CopyTo(list1_bytes);
                    hub.Reader.Complete();

                    list2 = StringPropertiesListMessage.Import(new ReadOnlySequence<byte>(list1_bytes), BufferPool.Shared);

                    Assert.Equal(list1, list2);
                }
            }
        }
    }
}
