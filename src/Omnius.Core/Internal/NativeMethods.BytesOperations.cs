using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Omnius.Core.Internal
{
    internal partial class NativeMethods
    {
        public static unsafe class BytesOperations
        {
            private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

            static BytesOperations()
            {
                if (!TryLoadNativeMethods())
                {
                    LoadPureUnsafeMethods();
                }
            }

            public delegate void ZeroDelegate(byte* source, int length);

            public delegate void CopyDelegate(byte* source, byte* destination, int length);

            [return: MarshalAs(UnmanagedType.U1)]
            public delegate bool EqualsDelegate(byte* source1, byte* source2, int length);

            public delegate int CompareDelegate(byte* source1, byte* source2, int length);

            public delegate void BitwiseOperationDelegate(byte* source1, byte* source2, byte* result, int length);

            public static ZeroDelegate Zero { get; private set; }

            public static CopyDelegate Copy { get; private set; }

            public static new EqualsDelegate Equals { get; private set; }

            public static CompareDelegate Compare { get; private set; }

            public static BitwiseOperationDelegate And { get; private set; }

            public static BitwiseOperationDelegate Or { get; private set; }

            public static BitwiseOperationDelegate Xor { get; private set; }

            [MemberNotNullWhen(true, nameof(Zero))]
            [MemberNotNullWhen(true, nameof(Copy))]
            [MemberNotNullWhen(true, nameof(Equals))]
            [MemberNotNullWhen(true, nameof(Compare))]
            [MemberNotNullWhen(true, nameof(And))]
            [MemberNotNullWhen(true, nameof(Or))]
            [MemberNotNullWhen(true, nameof(Xor))]
            public static bool TryLoadNativeMethods()
            {
                var nativeLibraryManager = NativeMethods.NativeLibraryManager;

                if (nativeLibraryManager != null)
                {
                    try
                    {
                        Zero = nativeLibraryManager.GetMethod<ZeroDelegate>("BytesOperations_Zero");
                        Copy = nativeLibraryManager.GetMethod<CopyDelegate>("BytesOperations_Copy");
                        Equals = nativeLibraryManager.GetMethod<EqualsDelegate>("BytesOperations_Equals");
                        Compare = nativeLibraryManager.GetMethod<CompareDelegate>("BytesOperations_Compare");
                        And = nativeLibraryManager.GetMethod<BitwiseOperationDelegate>("BytesOperations_And");
                        Or = nativeLibraryManager.GetMethod<BitwiseOperationDelegate>("BytesOperations_Or");
                        Xor = nativeLibraryManager.GetMethod<BitwiseOperationDelegate>("BytesOperations_Xor");

                        return true;
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e);
                    }
                }

                return false;
            }

            [MemberNotNull(nameof(Zero))]
            [MemberNotNull(nameof(Copy))]
            [MemberNotNull(nameof(Equals))]
            [MemberNotNull(nameof(Compare))]
            [MemberNotNull(nameof(And))]
            [MemberNotNull(nameof(Or))]
            [MemberNotNull(nameof(Xor))]
            public static void LoadPureUnsafeMethods()
            {
                Zero = PureUnsafe.Zero;
                Copy = PureUnsafe.Copy;
                Equals = PureUnsafe.Equals;
                Compare = PureUnsafe.Compare;
                And = PureUnsafe.And;
                Or = PureUnsafe.Or;
                Xor = PureUnsafe.Xor;
            }

            private static unsafe class PureUnsafe
            {
                public static void Zero(byte* source, int length)
                {
                    byte* t_s = source;

                    for (int i = (length / 8) - 1; i >= 0; i--, t_s += 8)
                    {
                        *((long*)t_s) = 0;
                    }

                    if ((length & 4) != 0)
                    {
                        *((int*)t_s) = 0;
                        t_s += 4;
                    }

                    if ((length & 2) != 0)
                    {
                        *((short*)t_s) = 0;
                        t_s += 2;
                    }

                    if ((length & 1) != 0)
                    {
                        *t_s = 0;
                    }
                }

                public static void Copy(byte* source, byte* destination, int length)
                {
                    Buffer.MemoryCopy(source, destination, 1024 * 1024, length);
                }

                public static bool Equals(byte* source1, byte* source2, int length)
                {
                    byte* t_x = source1, t_y = source2;

                    for (int i = (length / 8) - 1; i >= 0; i--, t_x += 8, t_y += 8)
                    {
                        if (*((long*)t_x) != *((long*)t_y)) return false;
                    }

                    if ((length & 4) != 0)
                    {
                        if (*((int*)t_x) != *((int*)t_y)) return false;

                        t_x += 4;
                        t_y += 4;
                    }

                    if ((length & 2) != 0)
                    {
                        if (*((short*)t_x) != *((short*)t_y)) return false;

                        t_x += 2;
                        t_y += 2;
                    }

                    if ((length & 1) != 0)
                    {
                        if (*t_x != *t_y) return false;
                    }

                    return true;
                }

                public static int Compare(byte* source1, byte* source2, int length)
                {
                    byte* t_x = source1, t_y = source2;

                    int len = length;
                    int c = 0;

                    for (; len > 0; len--)
                    {
                        c = *t_x++ - *t_y++;
                        if (c != 0) return c;
                    }

                    return 0;
                }

                public static void And(byte* source1, byte* source2, byte* result, int length)
                {
                    byte* t_x = source1, t_y = source2;
                    var t_buffer = result;

                    for (int i = (length / 8) - 1; i >= 0; i--, t_x += 8, t_y += 8, t_buffer += 8)
                    {
                        *((long*)t_buffer) = *((long*)t_x) & *((long*)t_y);
                    }

                    if ((length & 4) != 0)
                    {
                        *((int*)t_buffer) = *((int*)t_x) & *((int*)t_y);
                        t_x += 4;
                        t_y += 4;
                        t_buffer += 4;
                    }

                    if ((length & 2) != 0)
                    {
                        *((short*)t_buffer) = (short)(*((short*)t_x) & *((short*)t_y));
                        t_x += 2;
                        t_y += 2;
                        t_buffer += 2;
                    }

                    if ((length & 1) != 0)
                    {
                        *t_buffer = (byte)(*t_x & *t_y);
                    }
                }

                public static void Or(byte* source1, byte* source2, byte* result, int length)
                {
                    byte* t_x = source1, t_y = source2;
                    var t_buffer = result;

                    for (int i = (length / 8) - 1; i >= 0; i--, t_x += 8, t_y += 8, t_buffer += 8)
                    {
                        *((long*)t_buffer) = *((long*)t_x) | *((long*)t_y);
                    }

                    if ((length & 4) != 0)
                    {
                        *((int*)t_buffer) = *((int*)t_x) | *((int*)t_y);
                        t_x += 4;
                        t_y += 4;
                        t_buffer += 4;
                    }

                    if ((length & 2) != 0)
                    {
                        *((short*)t_buffer) = (short)(*((short*)t_x) | *((short*)t_y));
                        t_x += 2;
                        t_y += 2;
                        t_buffer += 2;
                    }

                    if ((length & 1) != 0)
                    {
                        *t_buffer = (byte)(*t_x | *t_y);
                    }
                }

                public static void Xor(byte* source1, byte* source2, byte* result, int length)
                {
                    byte* t_x = source1, t_y = source2;
                    var t_buffer = result;

                    for (int i = (length / 8) - 1; i >= 0; i--, t_x += 8, t_y += 8, t_buffer += 8)
                    {
                        *((long*)t_buffer) = *((long*)t_x) ^ *((long*)t_y);
                    }

                    if ((length & 4) != 0)
                    {
                        *((int*)t_buffer) = *((int*)t_x) ^ *((int*)t_y);
                        t_x += 4;
                        t_y += 4;
                        t_buffer += 4;
                    }

                    if ((length & 2) != 0)
                    {
                        *((short*)t_buffer) = (short)(*((short*)t_x) ^ *((short*)t_y));
                        t_x += 2;
                        t_y += 2;
                        t_buffer += 2;
                    }

                    if ((length & 1) != 0)
                    {
                        *t_buffer = (byte)(*t_x ^ *t_y);
                    }
                }
            }
        }
    }
}
