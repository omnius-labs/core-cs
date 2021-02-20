using System;
using System.Linq;
using Omnius.Core.Internal;
using Xunit;

namespace Omnius.Core
{
    public class BytesOperationsTest
    {
        [Fact]
        public void CopyTest()
        {
            var random = new Random(0);

            foreach (var maxLength in new int[] { 0, 32, 1024, 1024 * 32, 1024 * 1024 })
            {
                var buffer = new byte[maxLength];

                var targetBuffer1 = new byte[buffer.Length];
                var targetBuffer2 = new byte[buffer.Length];

                for (int i = 0; i < 32; i++)
                {
                    var offset = Math.Min(buffer.Length, random.Next(0, 1024 * 1024));
                    random.NextBytes(buffer);

                    BytesOperations.Copy(buffer.AsSpan()[offset..], targetBuffer1.AsSpan()[offset..], buffer.Length - offset);
                    Array.Copy(buffer, offset, targetBuffer2, offset, buffer.Length - offset);

                    Assert.True(Enumerable.SequenceEqual(targetBuffer1, targetBuffer2));
                }
            }
        }

        [Fact]
        public void EqualsTest()
        {
            Assert.True(BytesOperations.Equals(new byte[] { 0, 1, 2, 3, 4 }, (ReadOnlySpan<byte>)(new byte[] { 0, 1, 2, 3, 4 })));
            Assert.False(BytesOperations.Equals(new byte[] { 0, 1, 2, 3, 4 }, (ReadOnlySpan<byte>)(new byte[] { 0, 1, 2, 3, 4, 5 })));
            Assert.True(BytesOperations.Equals(new byte[] { 0, 1, 2, 3, 4 }.AsSpan().Slice(2), new byte[] { 0, 1, 2, 3, 4 }.AsSpan().Slice(2)));
            Assert.False(BytesOperations.Equals(new byte[] { 0, 1, 2, 3, 4 }.AsSpan().Slice(1), new byte[] { 0, 1, 2, 3, 4 }.AsSpan().Slice(2)));
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
                Assert.True(NativeMethods.BytesOperations.TryLoadNativeMethods());
                this.InternalBitwiseTest();

                NativeMethods.BytesOperations.LoadPureUnsafeMethods();
                this.InternalBitwiseTest();
            }
            finally
            {
                Assert.True(NativeMethods.BytesOperations.TryLoadNativeMethods());
            }
        }

        private void InternalBitwiseTest()
        {
            var random = new Random(0);

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
