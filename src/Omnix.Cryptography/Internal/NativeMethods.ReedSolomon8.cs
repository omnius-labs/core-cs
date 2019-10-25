using System;
using System.Runtime.InteropServices;

namespace Omnix.Internal
{
    partial class NativeMethods
    {
        public unsafe static class ReedSolomon8
        {
            public delegate void MulDelegate(byte* src, byte* dst, byte* table, int len);

            public static MulDelegate Mul { get; private set; }

            static ReedSolomon8()
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
                    Mul = nativeLibraryManager.GetMethod<MulDelegate>("ReedSolomon8_Mul");

                    return true;
                }

                return false;
            }

            public static void LoadPureUnsafeMethods()
            {
                Mul = PureUnsafe.Mul;
            }

            private static unsafe class PureUnsafe
            {
                public static void Mul(byte* src, byte* dst, byte* table, int len)
                {
                    const int Unroll = 16;
                    int i = 0;

                    for (; i < len && (len - i) > Unroll; i += Unroll)
                    {
                        dst[i] ^= table[src[i]];
                        dst[i + 1] ^= table[src[i + 1]];
                        dst[i + 2] ^= table[src[i + 2]];
                        dst[i + 3] ^= table[src[i + 3]];
                        dst[i + 4] ^= table[src[i + 4]];
                        dst[i + 5] ^= table[src[i + 5]];
                        dst[i + 6] ^= table[src[i + 6]];
                        dst[i + 7] ^= table[src[i + 7]];
                        dst[i + 8] ^= table[src[i + 8]];
                        dst[i + 9] ^= table[src[i + 9]];
                        dst[i + 10] ^= table[src[i + 10]];
                        dst[i + 11] ^= table[src[i + 11]];
                        dst[i + 12] ^= table[src[i + 12]];
                        dst[i + 13] ^= table[src[i + 13]];
                        dst[i + 14] ^= table[src[i + 14]];
                        dst[i + 15] ^= table[src[i + 15]];
                    }

                    for (; i < len; i++)
                    {
                        dst[i] ^= table[src[i]];
                    }
                }
            }
        }
    }
}
