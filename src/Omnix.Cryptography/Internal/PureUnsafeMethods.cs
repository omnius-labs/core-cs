using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Omnix.Cryptography.Internal
{
    static unsafe class PureUnsafeMethods
    {
        public static uint Crc32_Castagnoli_Compute(uint x, byte* src, int len)
        {
            return P_Crc32_Castagnoli.Compute(x, src, len);
        }

        static class P_Crc32_Castagnoli
        {
            private static readonly uint[] _table;

            static P_Crc32_Castagnoli()
            {
                //uint poly = 0xedb88320;
                uint poly = 0x82F63B78;
                _table = new uint[256];

                for (uint i = 0; i < 256; i++)
                {
                    uint x = i;

                    for (int j = 0; j < 8; j++)
                    {
                        if ((x & 1) != 0)
                        {
                            x = (x >> 1) ^ poly;
                        }
                        else
                        {
                            x >>= 1;
                        }
                    }

                    _table[i] = x;
                }
            }

            public static uint Compute(uint x, byte* src, int len)
            {
                fixed (uint* p_table = _table)
                {
                    var t_src = src;

                    for (int i = 0; i < len; i++)
                    {
                        x = (x >> 8) ^ p_table[(x & 0xff) ^ *t_src++];
                    }

                    return x;
                }
            }
        }
    }
}
