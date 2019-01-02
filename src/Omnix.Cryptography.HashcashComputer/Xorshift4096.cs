using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Omnix.Cryptography.HashcashComputer
{
    unsafe class Xorshift4096
    {
        private ulong[] _s = new ulong[64];
        private int _p;

        public Xorshift4096()
        {
            _p = 0;

            using (var random = RandomNumberGenerator.Create())
            {
                fixed (ulong* fixed_s = _s)
                {
                    random.GetBytes(new Span<byte>(fixed_s, _s.Length * sizeof(ulong)));
                }
            }
        }

        public ulong Next()
        {
            ulong s0 = _s[_p];
            ulong s1 = _s[_p = (_p + 1) & 63];

            s1 ^= s1 << 25; // a
            s1 ^= s1 >> 3;  // b
            s0 ^= s0 >> 49; // c

            return (_s[_p] = s0 ^ s1) * 8372773778140471301L;
        }
    }
}
