using System;
using System.Collections;
using Xunit;

namespace Omnius.Core.Extensions
{
    public class BitArrayExtensionTest
    {
        [Fact]
        public void GetCardinalityTest()
        {
            var random = new Random(0);

            const int maxLength = 1024 * 8;

            var bitArray = new BitArray(maxLength);

            for (int k = 0; k < 32; k++)
            {
                int limit = random.Next(0, 100);
                int count = 0;

                for (int i = 0; i < maxLength; i++)
                {
                    if (random.Next(0, 100) < limit)
                    {
                        count++;
                        bitArray.Set(i, true);
                    }
                }

                Assert.Equal(count, bitArray.GetCardinality());

                bitArray.SetAll(false);
            }
        }
    }
}
