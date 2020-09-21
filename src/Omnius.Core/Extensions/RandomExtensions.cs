using System;
using System.Collections.Generic;

namespace Omnius.Core.Extensions
{
    public static class RandomExtensions
    {
        public static void Shuffle<T>(this Random random, IList<T> collection)
        {
            int n = collection.Count;

            while (n > 1)
            {
                int k = random.Next(n--);
                var temp = collection[n];
                collection[n] = collection[k];
                collection[k] = temp;
            }
        }

        public static byte[] GetBytes(this Random random, int length)
        {
            var buffer = new byte[length];
            random.NextBytes(buffer);
            return buffer;
        }
    }
}
