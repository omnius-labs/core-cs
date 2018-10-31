using System;
using System.Buffers;
using System.Security.Cryptography;
using Omnix.Base;

namespace Omnix.Cryptography
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

        public static ReadOnlyMemory<byte> ComputeHash(ReadOnlySequence<byte> sequence, ReadOnlySpan<byte> key)
        {
            if (key.Length > _blockLength)
            {
                key = Sha2_256.ComputeHash(key).Span;
            }

            var ixor = new byte[_blockLength];
            BytesOperations.Xor(_ipad, key, ixor);

            var oxor = new byte[_blockLength];
            BytesOperations.Xor(_opad, key, oxor);

            byte[] ihash;

            using (var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
            {
                incrementalHash.AppendData(ixor, 0, ixor.Length);

                foreach (var segment in sequence)
                {
                    incrementalHash.AppendData(segment.Span);
                }

                ihash = incrementalHash.GetHashAndReset();
            }

            byte[] ohash;

            using (var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
            {
                incrementalHash.AppendData(oxor, 0, oxor.Length);
                incrementalHash.AppendData(ihash, 0, ihash.Length);

                ohash = incrementalHash.GetHashAndReset();
            }

            return ohash;
        }

        public static ReadOnlyMemory<byte> ComputeHash(ReadOnlySpan<byte> source, ReadOnlySpan<byte> key)
        {
            if (key.Length > _blockLength)
            {
                key = Sha2_256.ComputeHash(key).Span;
            }

            var ixor = new byte[_blockLength];
            BytesOperations.Xor(_ipad, key, ixor);

            var oxor = new byte[_blockLength];
            BytesOperations.Xor(_opad, key, oxor);

            byte[] ihash;

            using (var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
            {
                incrementalHash.AppendData(ixor, 0, ixor.Length);
                incrementalHash.AppendData(source);

                ihash = incrementalHash.GetHashAndReset();
            }

            byte[] ohash;

            using (var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
            {
                incrementalHash.AppendData(oxor, 0, oxor.Length);
                incrementalHash.AppendData(ihash, 0, ihash.Length);

                ohash = incrementalHash.GetHashAndReset();
            }

            return ohash;
        }
    }
}
