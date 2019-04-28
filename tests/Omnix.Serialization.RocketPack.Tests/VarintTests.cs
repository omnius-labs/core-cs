using System;
using System.Collections.Generic;
using System.Text;
using Omnix.Base;
using Xunit;

namespace Omnix.Serialization.RocketPack.Tests
{
    public class VarintTests
    {
        [Fact]
        public void RandomTest()
        {
            var random = new Random();
            var hub = new Hub();

            for (int i = 0; i < 1024; i++)
            {
                var result1 = ((ulong)random.Next() << 32) | (uint)random.Next();

                Varint.SetUInt64(result1, hub.Writer);
                hub.Writer.Complete();

                Varint.TryGetUInt64(hub.Reader.GetSequence(), out var result2, out var _);
                hub.Reader.Complete();

                Assert.Equal(result1, result2);
                hub.Reset();
            }
        }
    }
}
