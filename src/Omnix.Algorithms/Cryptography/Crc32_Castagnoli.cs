using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Omnix.Algorithms.Cryptography.Internal;
using Omnix.Base;

namespace Omnix.Algorithms.Cryptography
{
    public static unsafe class Crc32_Castagnoli
    {
        private static readonly ThreadLocal<Encoding> _utf8Encoding = new ThreadLocal<Encoding>(() => new UTF8Encoding(false));

        private static NativeLibraryManager? _nativeLibraryManager;

        private delegate uint ComputeDelegate(uint x, byte* src, int len);
        private static ComputeDelegate _compute;

        static Crc32_Castagnoli()
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
            _nativeLibraryManager?.Dispose();

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                    {
                        _nativeLibraryManager = new NativeLibraryManager("omnix-cryptography.x64.dll");
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
                        _nativeLibraryManager = new NativeLibraryManager("omnix-cryptography.x64.so");
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

                _compute = _nativeLibraryManager.GetMethod<ComputeDelegate>("compute_Crc32_Castagnoli");
            }
            catch (Exception)
            {
                _nativeLibraryManager?.Dispose();
                _nativeLibraryManager = null;

                throw;
            }
        }

        internal static void LoadPureUnsafeMethods()
        {
            if (_nativeLibraryManager != null)
            {
                _nativeLibraryManager.Dispose();
                _nativeLibraryManager = null;
            }

            _compute = PureUnsafeMethods.Crc32_Castagnoli_Compute;
        }

        public static int ComputeHash(ReadOnlySpan<byte> memory)
        {
            uint x = 0xFFFFFFFF;

            fixed (byte* p_buffer = memory)
            {
                var t_buffer = p_buffer;

                x = _compute(x, t_buffer, memory.Length);
            }

            return (int)(x ^ 0xFFFFFFFF);
        }

        public static int ComputeHash(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (var recyclableMemory = MemoryPool<byte>.Shared.Rent(_utf8Encoding.Value.GetMaxByteCount(value.Length)))
            {
                var length = _utf8Encoding.Value.GetBytes(value, recyclableMemory.Memory.Span);

                return ComputeHash(recyclableMemory.Memory.Span.Slice(0, length));
            }
        }

        public static int ComputeHash(ReadOnlySequence<byte> sequence)
        {
            uint x = 0xFFFFFFFF;

            foreach (var segment in sequence)
            {
                fixed (byte* p_segment = segment.Span)
                {
                    x = _compute(x, p_segment, segment.Length);
                }
            }

            return (int)(x ^ 0xFFFFFFFF);
        }
    }
}
