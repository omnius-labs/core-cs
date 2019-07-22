using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Omnix.Correction.Internal
{
    internal static unsafe class PureUnsafeMethods
    {
        public static void ReedSolomon8_Mul(byte* src, byte* dst, byte* table, int len)
        {
            int i = 0;

            for (int count = ((len - i) / 8) - 1; count >= 0; count--)
            {
                dst[0] ^= table[src[0]];
                dst[1] ^= table[src[1]];
                dst[2] ^= table[src[2]];
                dst[3] ^= table[src[3]];
                dst[4] ^= table[src[4]];
                dst[5] ^= table[src[5]];
                dst[6] ^= table[src[6]];
                dst[7] ^= table[src[7]];

                src += 8;
                dst += 8;
                i += 8;
            }

            for (; i < len; i++)
            {
                *dst++ ^= table[*src++];
            }
        }
    }
}
