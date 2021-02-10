using System;
using System.Buffers;
using System.Security.Cryptography;

namespace Omnius.Core.Cryptography.Functions
{
    public static class Hmac_Sha2_256
    {
        private static readonly int _blockLength = 64;
        private static readonly byte[] _ipad;
        private static readonly byte[] _opad;

        static Hmac_Sha2_256()
        {
            _ipad = new byte[_blockLength];
            _opad = new byte[_blockLength];

            for (int i = 0; i < _blockLength; i++)
            {
                _ipad[i] = 0x36;
                _opad[i] = 0x5C;
            }
        }

        public static byte[] ComputeHash(ReadOnlySequence<byte> sequence, ReadOnlySpan<byte> key)
        {
            byte[] result = new byte[32];
            TryComputeHash(sequence, key, result);

            return result;
        }

        public static byte[] ComputeHash(ReadOnlySpan<byte> sequence, ReadOnlySpan<byte> key)
        {
            byte[] result = new byte[32];
            TryComputeHash(sequence, key, result);

            return result;
        }

        public static bool TryComputeHash(ReadOnlySequence<byte> sequence, ReadOnlySpan<byte> key, Span<byte> destination)
        {
            if (destination.Length < 32) throw new ArgumentOutOfRangeException(nameof(destination));

            Span<byte> extendedKey = stackalloc byte[_blockLength];

            if (key.Length > _blockLength)
            {
                Sha2_256.TryComputeHash(key, extendedKey);
            }
            else
            {
                BytesOperations.Copy(key, extendedKey, Math.Min(key.Length, extendedKey.Length));
            }

            Span<byte> ixor = stackalloc byte[_blockLength];
            BytesOperations.Xor(_ipad, extendedKey, ixor);

            Span<byte> oxor = stackalloc byte[_blockLength];
            BytesOperations.Xor(_opad, extendedKey, oxor);

            Span<byte> ihash = stackalloc byte[32];

            using (var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
            {
                incrementalHash.AppendData(ixor);

                foreach (var segment in sequence)
                {
                    incrementalHash.AppendData(segment.Span);
                }

                incrementalHash.TryGetHashAndReset(ihash, out _);
            }

            using (var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
            {
                incrementalHash.AppendData(oxor);
                incrementalHash.AppendData(ihash);

                return incrementalHash.TryGetHashAndReset(destination, out _);
            }
        }

        public static bool TryComputeHash(ReadOnlySpan<byte> source, ReadOnlySpan<byte> key, Span<byte> destination)
        {
            if (destination.Length < 32) throw new ArgumentOutOfRangeException(nameof(destination));

            Span<byte> extendedKey = stackalloc byte[_blockLength];

            if (key.Length > _blockLength)
            {
                Sha2_256.TryComputeHash(key, extendedKey);
            }
            else
            {
                BytesOperations.Copy(key, extendedKey, Math.Min(key.Length, extendedKey.Length));
            }

            Span<byte> ixor = stackalloc byte[_blockLength];
            BytesOperations.Xor(_ipad, extendedKey, ixor);

            Span<byte> oxor = stackalloc byte[_blockLength];
            BytesOperations.Xor(_opad, extendedKey, oxor);

            Span<byte> ihash = stackalloc byte[32];

            using (var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
            {
                incrementalHash.AppendData(ixor);
                incrementalHash.AppendData(source);

                incrementalHash.TryGetHashAndReset(ihash, out _);
            }

            using (var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
            {
                incrementalHash.AppendData(oxor);
                incrementalHash.AppendData(ihash);

                return incrementalHash.TryGetHashAndReset(destination, out _);
            }
        }
    }
}
