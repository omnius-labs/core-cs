using System.Buffers;
using System.Collections;

namespace Omnius.Core.Extensions
{
    public static class BitArrayExtensions
    {
        // https://stackoverflow.com/questions/5063178/counting-bits-set-in-a-net-bitarray-class

        /// <summary>
        /// Setされたフラグの数を取得します。
        /// </summary>
        public static int GetCardinality(this BitArray bitArray)
        {
            int intsLength = (bitArray.Count >> 5) + 1;
            var ints = ArrayPool<int>.Shared.Rent(intsLength);

            bitArray.CopyTo(ints, 0);

            int count = 0;

            // fix for not truncated bits in last integer that may have been set to true with SetAll()
            ints[intsLength - 1] &= ~(-1 << (bitArray.Count % 32));

            for (int i = 0; i < intsLength; i++)
            {
                int c = ints[i];

                // magic (http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel)
                unchecked
                {
                    c = c - ((c >> 1) & 0x55555555);
                    c = (c & 0x33333333) + ((c >> 2) & 0x33333333);
                    c = ((c + (c >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
                }

                count += c;
            }

            ArrayPool<int>.Shared.Return(ints);

            return count;
        }
    }
}
