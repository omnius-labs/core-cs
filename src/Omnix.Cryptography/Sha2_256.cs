using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Omnix.Cryptography
{
    public static class Sha2_256
    {
        private static readonly ThreadLocal<Encoding> _utf8Encoding = new ThreadLocal<Encoding>(() => new UTF8Encoding(false));

        public static ReadOnlyMemory<byte> ComputeHash(ReadOnlySpan<byte> value)
        {
            byte[] result = new byte[32];

            using (var sha2_256 = SHA256.Create())
            {
                sha2_256.TryComputeHash(value, result.AsSpan(), out int _);
            }

            return result;
        }

        public static ReadOnlyMemory<byte> ComputeHash(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            using (var recyclableMemory = MemoryPool<byte>.Shared.Rent(_utf8Encoding.Value.GetMaxByteCount(value.Length)))
            {
                var length = _utf8Encoding.Value.GetBytes(value, recyclableMemory.Memory.Span);

                return ComputeHash(recyclableMemory.Memory.Span.Slice(0, length));
            }
        }

        public static ReadOnlyMemory<byte> ComputeHash(ReadOnlySequence<byte> sequence)
        {
            using (var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
            {
                foreach (var segment in sequence)
                {
                    incrementalHash.AppendData(segment.Span);
                }

                return incrementalHash.GetHashAndReset();
            }
        }
    }
}
