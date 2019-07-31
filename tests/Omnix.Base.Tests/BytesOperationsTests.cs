using System;
using System.Linq;
using Xunit;

namespace Omnix.Base
{
    public class BytesOperationsTests
    {
        [Fact]
        public void CopyTest()
        {
            var random = new Random();

            foreach (var maxLength in new int[] { 0, 32, 1024, 1024 * 32, 1024 * 1024 })
            {
                var buffer = new byte[maxLength];

                var targetBuffer1 = new byte[buffer.Length];
                var targetBuffer2 = new byte[buffer.Length];

                for (int i = 0; i < 32; i++)
                {
                    var offset = Math.Min(buffer.Length, random.Next(0, 1024 * 1024));
                    random.NextBytes(buffer);

                    BytesOperations.Copy(buffer.AsSpan().Slice(offset), targetBuffer1.AsSpan().Slice(offset), buffer.Length - offset);
                    Array.Copy(buffer, offset, targetBuffer2, offset, buffer.Length - offset);

                    Assert.True(Enumerable.SequenceEqual(targetBuffer1, targetBuffer2));
                }
            }
        }

        [Fact]
        public void EqualsTest()
        {
            var random = new Random();

            Assert.True(BytesOperations.SequenceEqual(new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 0, 1, 2, 3, 4 }));
            Assert.False(BytesOperations.SequenceEqual(new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 0, 1, 2, 3, 4, 5 }));
            Assert.True(BytesOperations.SequenceEqual(new byte[] { 0, 1, 2, 3, 4 }.AsSpan().Slice(2), new byte[] { 0, 1, 2, 3, 4 }.AsSpan().Slice(2)));
            Assert.False(BytesOperations.SequenceEqual(new byte[] { 0, 1, 2, 3, 4 }.AsSpan().Slice(1), new byte[] { 0, 1, 2, 3, 4 }.AsSpan().Slice(2)));
        }

        [Fact]
        public void CompareTest()
        {
            Assert.True(BytesOperations.Compare(new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 0, 1, 2, 3, 4 }) == 0);
            Assert.True(BytesOperations.Compare(new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 0, 1, 2, 3, 5 }) < 0);
            Assert.True(BytesOperations.Compare(new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 0, 1, 2, 3, 3 }) > 0);
            Assert.True(BytesOperations.Compare(new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 0, 1, 2, 5 }) > 0);
            Assert.True(BytesOperations.Compare(new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 0, 1, 2, 0, 0, 0 }) < 0);
        }

        [Fact]
        public void BitwiseTest()
        {
            try
            {
                for (int type = 0; type < 2; type++)
                {
                    if (type == 0)
                    {
                        BytesOperations.LoadNativeMethods();
                    }
                    else if (type == 1)
                    {
                        BytesOperations.LoadPureUnsafeMethods();
                    }

                    this.InternalBitwiseTest();
                }
            }
            finally
            {
                BytesOperations.LoadNativeMethods();
            }
        }

        private void InternalBitwiseTest()
        {
            var random = new Random();

            for (int i = 0; i < 256; i++)
            {
                int length = random.Next(1024, 8192);

                var x = new byte[length];
                var y = new byte[length];
                var z = new byte[length];

                random.NextBytes(x);
                random.NextBytes(y);

                BytesOperations.And(x, y, z);

                for (int j = 0; j < length; j++)
                {
                    Assert.True(z[j] == (x[j] & y[j]));
                }
            }

            for (int i = 0; i < 256; i++)
            {
                int length = random.Next(1024, 8192);

                var x = new byte[length];
                var y = new byte[length];
                var z = new byte[length];

                random.NextBytes(x);
                random.NextBytes(y);

                BytesOperations.Or(x, y, z);

                for (int j = 0; j < length; j++)
                {
                    Assert.True(z[j] == (x[j] | y[j]));
                }
            }

            for (int i = 0; i < 256; i++)
            {
                int length = random.Next(1024, 8192);

                var x = new byte[length];
                var y = new byte[length];
                var z = new byte[length];

                random.NextBytes(x);
                random.NextBytes(y);

                BytesOperations.Xor(x, y, z);

                for (int j = 0; j < length; j++)
                {
                    Assert.True(z[j] == (x[j] ^ y[j]));
                }
            }
        }
    }
}
