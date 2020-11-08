using System;

namespace Omnius.Core.Cryptography.Functions
{
    // https://qiita.com/Ushio/items/a19083514d087a57fc72
    public static class Fnv1_64
    {
        public static long ComputeHash(ReadOnlySpan<byte> value)
        {
            unchecked
            {
                const long basis = (long)14695981039346656037;
                const long prime = 1099511628211;

                long hash = basis;

                for (int i = 0; i < value.Length; i++)
                {
                    hash = (prime * hash) ^ value[i];
                }

                return hash;
            }
        }
    }
}
