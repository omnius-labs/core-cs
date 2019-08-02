using System;
using System.Runtime.InteropServices;
using Omnix.Base.Internal;

namespace Omnix.Base
{
    public static unsafe class BytesOperations
    {
        [Obsolete("", true)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static new bool Equals(object obj1, object obj2)
        {
            throw new NotImplementedException();
        }

        public static void Zero(Span<byte> source)
        {
            if (source.Length == 0)
            {
                return;
            }

            fixed (byte* p = source)
            {
                NativeMethods.BytesOperations.Zero(p, source.Length);
            }
        }

        public static void Copy(ReadOnlySpan<byte> source, Span<byte> destination, int length)
        {
            if (length > source.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (length > destination.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (length == 0)
            {
                return;
            }

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
            if (source1.Length != source2.Length)
            {
                return false;
            }

            fixed (byte* p_x = source1, p_y = source2)
            {
                return NativeMethods.BytesOperations.Equals(p_x, p_y, source1.Length);
            }
        }

        public static bool SequenceEqual(ReadOnlySpan<byte> source1, ReadOnlySpan<byte> source2, int length)
        {
            if (length > source1.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (length > source2.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            fixed (byte* p_x = source1, p_y = source2)
            {
                return NativeMethods.BytesOperations.Equals(p_x, p_y, length);
            }
        }

        public static int Compare(ReadOnlySpan<byte> source1, ReadOnlySpan<byte> source2)
        {
            if (source1.Length != source2.Length)
            {
                return (source1.Length > source2.Length) ? 1 : -1;
            }

            if (source1.Length == 0)
            {
                return 0;
            }

            fixed (byte* p_x = source1, p_y = source2)
            {
                return NativeMethods.BytesOperations.Compare(p_x, p_y, source1.Length);
            }
        }

        public static void And(ReadOnlySpan<byte> source1, ReadOnlySpan<byte> source2, Span<byte> destination)
        {
            BitwiseOperation(NativeMethods.BytesOperations.And, source1, source2, destination);
        }

        public static void Or(ReadOnlySpan<byte> source1, ReadOnlySpan<byte> source2, Span<byte> destination)
        {
            BitwiseOperation(NativeMethods.BytesOperations.Or, source1, source2, destination);
        }

        public static void Xor(ReadOnlySpan<byte> source1, ReadOnlySpan<byte> source2, Span<byte> destination)
        {
            BitwiseOperation(NativeMethods.BytesOperations.Xor, source1, source2, destination);
        }

        private static void BitwiseOperation(NativeMethods.BytesOperations.BitwiseOperationDelegate bitwiseOperation, ReadOnlySpan<byte> source1, ReadOnlySpan<byte> source2, Span<byte> destination)
        {
            // Zero
            {
                int targetRange = Math.Max(source1.Length, source2.Length);

                if (destination.Length > targetRange)
                {
                    Zero(destination.Slice(targetRange, destination.Length - targetRange));
                }
            }

            // Copy
            if (source1.Length > source2.Length && destination.Length > source2.Length)
            {
                Copy(source1, destination, Math.Min(source1.Length, destination.Length) - source2.Length);
            }
            else if (source2.Length > source1.Length && destination.Length > source1.Length)
            {
                Copy(source2, destination, Math.Min(source2.Length, destination.Length) - source1.Length);
            }

            // BitwiseOperation
            {
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
}
