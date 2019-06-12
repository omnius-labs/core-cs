using System;
using System.Collections.Generic;
using System.Threading;
using Omnix.Base.Internal;

namespace Omnix.Base.Extensions
{
    public static class RandomExtensions
    {
        public static void Shuffle<T>(this Random random, IList<T> collection)
        {
            object? lockObject = ExtensionHelper.GetLockObject(collection);
            bool lockToken = false;

            try
            {
                if (lockObject != null)
                {
                    Monitor.Enter(lockObject, ref lockToken);
                }

                int n = collection.Count;

                while (n > 1)
                {
                    int k = random.Next(n--);
                    var temp = collection[n];
                    collection[n] = collection[k];
                    collection[k] = temp;
                }
            }
            finally
            {
                if (lockToken)
                {
                    Monitor.Exit(lockObject);
                }
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
