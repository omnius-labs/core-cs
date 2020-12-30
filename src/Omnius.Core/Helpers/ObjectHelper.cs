using System;
using System.Buffers;
using System.Security.Cryptography;
using Omnius.Core.Common;

namespace Omnius.Core.Helpers
{
    public static partial class ObjectHelper
    {
        private static readonly SipHasher _hashFunction;

        static ObjectHelper()
        {
            using var random = RandomNumberGenerator.Create();
            var buffer = new byte[16];
            random.GetBytes(buffer);

            _hashFunction = new SipHasher(buffer);
        }

        public static int GetHashCode(ReadOnlySpan<byte> value)
        {
            _hashFunction.Write(value);
            ulong v = _hashFunction.Finalize();

            return (int)(v & 0xFFFFFFFF) | (int)(v >> 32);
        }

        public static int GetHashCode(ReadOnlySequence<byte> sequence)
        {
            _hashFunction.Write(sequence);
            ulong v = _hashFunction.Finalize();

            return (int)(v & 0xFFFFFFFF) | (int)(v >> 32);
        }
    }
}
