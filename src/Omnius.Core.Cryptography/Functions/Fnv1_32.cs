using System;

namespace Omnius.Core.Cryptography.Functions
{
    // https://qiita.com/Ushio/items/a19083514d087a57fc72
    public static class Fnv1_32
    {
        public static int ComputeHash(ReadOnlySpan<byte> value)
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
    }
}
