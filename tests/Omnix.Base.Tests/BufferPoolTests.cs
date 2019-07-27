using System;
using Xunit;

namespace Omnix.Base
{
    public class BufferPoolTests
    {
        [Fact]
        public void RentAndDisposeTest()
        {
            var random = new Random();
            var bufferPool = BufferPool.Create();

            for (int i = 0; i < 32; i++)
            {
                int size = random.Next(1, 1024 * 1024 * 32);
                var buffer = bufferPool.Rent(size);

                Assert.True(buffer.Memory.Length >= size);

                buffer.Dispose();
            }
        }
    }
}
