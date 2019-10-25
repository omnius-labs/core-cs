using System;
using System.Runtime.InteropServices;

namespace Omnix.Internal
{
    partial class NativeMethods
    {
        public unsafe static class Crc32_Castagnoli
        {
            public delegate uint ComputeDelegate(uint x, byte* src, int len);

            public static ComputeDelegate Compute { get; private set; }

            static Crc32_Castagnoli()
            {
                if (!TryLoadNativeMethods())
                {
                    LoadPureUnsafeMethods();
                }
            }

            public static bool TryLoadNativeMethods()
            {
                var nativeLibraryManager = NativeMethods.NativeLibraryManager;

                if (nativeLibraryManager != null)
                {
                    Compute = nativeLibraryManager.GetMethod<ComputeDelegate>("Crc32_Castagnoli_Compute");

                    return true;
                }

                return false;
            }

            public static void LoadPureUnsafeMethods()
            {
                Compute = PureUnsafe.Compute;
            }

            private static unsafe class PureUnsafe
            {
                private static readonly uint[] _table;

                static PureUnsafe()
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
}
