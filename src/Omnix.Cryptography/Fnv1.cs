using System;
using System.Collections.Generic;
using System.Text;

namespace Omnix.Cryptography
{
    // https://qiita.com/Ushio/items/a19083514d087a57fc72
    public static class Fnv1
    {
        public static int ComputeHash32(ReadOnlySpan<byte> value)
        {
            unchecked
            {
                const int basis = (int)2166136261;
                const int prime = 16777619;

                int hash = basis;

                for (int i = 0; i < value.Length; i++)
                {
                    hash = (prime * hash) ^ value[i];
                }

                return hash;
            }
        }

        public static long ComputeHash64(ReadOnlySpan<byte> value)
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
