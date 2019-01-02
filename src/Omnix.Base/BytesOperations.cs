using System;
using System.IO;
using System.Runtime.InteropServices;
using Omnix.Base.Internal;

namespace Omnix.Base
{
    public unsafe static class BytesOperations
    {
        private static NativeLibraryManager _nativeLibraryManager;

        private delegate void ZeroDelegate(byte* source, int length);
        private delegate void CopyDelegate(byte* source, byte* destination, int length);
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool EqualsDelegate(byte* source1, byte* source2, int length);
        private delegate int CompareDelegate(byte* source1, byte* source2, int length);
        private delegate void BitwiseOperationDelegate(byte* source1, byte* source2, byte* result, int length);

        private static ZeroDelegate _zero;
        private static CopyDelegate _copy;
        private static EqualsDelegate _equals;
        private static CompareDelegate _compare;
        private static BitwiseOperationDelegate _and;
        private static BitwiseOperationDelegate _or;
        private static BitwiseOperationDelegate _xor;

        static BytesOperations()
        {
            try
            {
                LoadNativeMethods();
            }
            catch (Exception)
            {
                LoadPureUnsafeMethods();
            }
        }

        internal static void LoadNativeMethods()
        {
            if (_nativeLibraryManager != null)
            {
                _nativeLibraryManager.Dispose();
                _nativeLibraryManager = null;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                {
                    _nativeLibraryManager = new NativeLibraryManager("Assemblies/Omnix.Base.win-x64.dll");
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                {
                    _nativeLibraryManager = new NativeLibraryManager("Assemblies/Omnix.Base.linux-x64.so");
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            else
            {
                throw new NotSupportedException();
            }

            _zero = _nativeLibraryManager.GetMethod<ZeroDelegate>("zero");
            _copy = _nativeLibraryManager.GetMethod<CopyDelegate>("copy");
            _equals = _nativeLibraryManager.GetMethod<EqualsDelegate>("equals");
            _compare = _nativeLibraryManager.GetMethod<CompareDelegate>("compare");
            _and = _nativeLibraryManager.GetMethod<BitwiseOperationDelegate>("math_and");
            _or = _nativeLibraryManager.GetMethod<BitwiseOperationDelegate>("math_or");
            _xor = _nativeLibraryManager.GetMethod<BitwiseOperationDelegate>("math_xor");
        }

        internal static void LoadPureUnsafeMethods()
        {
            if (_nativeLibraryManager != null)
            {
                _nativeLibraryManager.Dispose();
                _nativeLibraryManager = null;
            }

            _zero = PureUnsafeMethods.Zero;
            _copy = PureUnsafeMethods.Copy;
            _equals = PureUnsafeMethods.Equals;
            _compare = PureUnsafeMethods.Compare;
            _and = PureUnsafeMethods.And;
            _or = PureUnsafeMethods.Or;
            _xor = PureUnsafeMethods.Xor;
        }

        [Obsolete("", true)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public new static bool Equals(object obj1, object obj2)
        {
            throw new NotImplementedException();
        }

        public static void Zero(Span<byte> source)
        {
            if (source.Length == 0) return;

            fixed (byte* p = source)
            {
                _zero(p, source.Length);
            }
        }

        public static void Copy(ReadOnlySpan<byte> source, Span<byte> destination, int length)
        {
            if (length > source.Length) throw new ArgumentOutOfRangeException(nameof(length));
            if (length > destination.Length) throw new ArgumentOutOfRangeException(nameof(length));

            if (length == 0) return;

            fixed (byte* p_x = source)
            fixed (byte* p_y = destination)
            {
                Buffer.MemoryCopy(p_x, p_y, destination.Length, length);
                //_copy(p_x, p_y, length);
            }
        }

        // Copyright (c) 2008-2013 Hafthor Stefansson
        // Distributed under the MIT/X11 software license
        // Ref: http://www.opensource.org/licenses/mit-license.php.
        // http://stackoverflow.com/questions/43289/comparing-two-byte-arrays-in-net
        public static bool SequenceEqual(ReadOnlySpan<byte> source1, ReadOnlySpan<byte> source2)
        {
            if (source1.Length != source2.Length) return false;

            fixed (byte* p_x = source1, p_y = source2)
            {
                return _equals(p_x, p_y, source1.Length);
            }
        }
        public static bool SequenceEqual(ReadOnlySpan<byte> source1, ReadOnlySpan<byte> source2, int length)
        {
            if (length > source1.Length) throw new ArgumentOutOfRangeException(nameof(length));
            if (length > source2.Length) throw new ArgumentOutOfRangeException(nameof(length));

            fixed (byte* p_x = source1, p_y = source2)
            {
                return _equals(p_x, p_y, length);
            }
        }

        public static int Compare(ReadOnlySpan<byte> source1, ReadOnlySpan<byte> source2)
        {
            if (source1.Length != source2.Length) return (source1.Length > source2.Length) ? 1 : -1;
            if (source1.Length == 0) return 0;

            fixed (byte* p_x = source1, p_y = source2)
            {
                return _compare(p_x, p_y, source1.Length);
            }
        }

        public static void And(ReadOnlySpan<byte> source1, ReadOnlySpan<byte> source2, Span<byte> destination)
        {
            BytesOperations.BitwiseOperation(_and, source1, source2, destination);
        }

        public static void Or(ReadOnlySpan<byte> source1, ReadOnlySpan<byte> source2, Span<byte> destination)
        {
            BytesOperations.BitwiseOperation(_or, source1, source2, destination);
        }

        public static void Xor(ReadOnlySpan<byte> source1, ReadOnlySpan<byte> source2, Span<byte> destination)
        {
            BytesOperations.BitwiseOperation(_xor, source1, source2, destination);
        }

        private static void BitwiseOperation(BitwiseOperationDelegate bitwiseOperation, ReadOnlySpan<byte> source1, ReadOnlySpan<byte> source2, Span<byte> destination)
        {
            // Zero
            {
                int targetRange = Math.Max(source1.Length, source2.Length);

                if (destination.Length > targetRange)
                {
                    BytesOperations.Zero(destination.Slice(targetRange, destination.Length - targetRange));
                }
            }

            if (source1.Length > source2.Length && destination.Length > source2.Length)
            {
                BytesOperations.Copy(source1, destination, Math.Min(source1.Length, destination.Length) - source2.Length);
            }
            else if (source2.Length > source1.Length && destination.Length > source1.Length)
            {
                BytesOperations.Copy(source2, destination, Math.Min(source2.Length, destination.Length) - source1.Length);
            }

            int length = Math.Min(Math.Min(source1.Length, source2.Length), destination.Length);

            fixed (byte* p_x = source1, p_y = source2)
            {
                fixed (byte* p_buffer = destination)
                {
                    bitwiseOperation(p_x, p_y, p_buffer, length);
                }
            }
        }
    }
}
