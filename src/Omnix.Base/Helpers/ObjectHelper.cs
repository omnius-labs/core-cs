using System;
using System.Buffers;
using System.IO;
using System.Security.Cryptography;

namespace Omnix.Base.Helpers
{
    public static partial class ObjectHelper
    {
        private static SipHash _hashFunction;

        static ObjectHelper()
        {
            using (var random = RandomNumberGenerator.Create())
            {
                var buffer = new byte[16];
                random.GetBytes(buffer);

                _hashFunction = new SipHash(buffer);
            }
        }

        public static int GetHashCode(ReadOnlySpan<byte> value)
        {
            long v = _hashFunction.ComputeHash(value);

            return (int)Math.Abs((v & 0xFFFFFFFF) | (v >> 32));
        }

        public static int GetHashCode(ReadOnlySequence<byte> sequence)
        {
            long v = _hashFunction.ComputeHash(sequence);

            return (int)Math.Abs((v & 0xFFFFFFFF) | (v >> 32));
        }
    }
}
