//Copyright (c) 2012 Josip Medved <jmedved@jmedved.com>

//2012-04-12: Initial version.

using System;
using System.Security.Cryptography;
using Omnix.Base;

namespace Omnix.Cryptography
{
    /// <summary>
    /// Generic PBKDF2 implementation.
    /// </summary>
    /// <example>This sample shows how to initialize class with SHA-256 HMAC.
    /// <code>
    /// using (var hmac = new HMACSHA256()) {
    ///     var df = new Pbkdf2(hmac, "password", "salt");
    ///     var bytes = df.GetBytes();
    /// }
    /// </code>
    /// </example>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pbkdf", Justification = "Spelling is correct.")]
    public unsafe class Pbkdf2
    {
        private readonly int _blockSize;
        private uint _blockIndex = 1;

        private byte[] _bufferBytes;
        private int _bufferStartIndex = 0;
        private int _bufferEndIndex = 0;

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="algorithm">HMAC algorithm to use.</param>
        /// <param name="password">The password used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <param name="iterations">The number of iterations for the operation.</param>
        /// <exception cref="System.ArgumentNullException">Algorithm cannot be null - Password cannot be null. -or- Salt cannot be null.</exception>
        public Pbkdf2(HMAC algorithm, ReadOnlyMemory<byte> password, ReadOnlyMemory<byte> salt, int iterations)
        {
            if (algorithm == null) throw new ArgumentNullException(nameof(algorithm), "Algorithm cannot be null.");
            if (password.IsEmpty) throw new ArgumentNullException(nameof(password), "Password cannot be null.");
            if (salt.IsEmpty) throw new ArgumentNullException(nameof(salt), "Salt cannot be null.");

            this.Algorithm = algorithm;
            this.Algorithm.Key = password.ToArray();
            this.Salt = salt;
            this.IterationCount = iterations;

            _blockSize = this.Algorithm.HashSize / 8;
        }

        /// <summary>
        /// Gets algorithm used for generating key.
        /// </summary>
        public HMAC Algorithm { get; private set; }

        /// <summary>
        /// Gets salt bytes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Byte array is proper return value in this case.")]
        public ReadOnlyMemory<byte> Salt { get; private set; }

        /// <summary>
        /// Gets iteration count.
        /// </summary>
        public int IterationCount { get; private set; }

        /// <summary>
        /// Returns a pseudo-random key from a password, salt and iteration count.
        /// </summary>
        /// <param name="count">Number of bytes to return.</param>
        /// <returns>Byte array.</returns>
        public byte[] GetBytes(int count)
        {
            var result = new byte[count];
            int resultOffset = 0;
            int bufferCount = _bufferEndIndex - _bufferStartIndex;

            if (bufferCount > 0)
            { //if there is some data in buffer
                if (count < bufferCount)
                { //if there is enough data in buffer
                    BytesOperations.Copy(_bufferBytes.AsSpan(_bufferStartIndex), result, count);
                    _bufferStartIndex += count;

                    return result;
                }

                BytesOperations.Copy(_bufferBytes.AsSpan(_bufferStartIndex), result, bufferCount);
                _bufferStartIndex = _bufferEndIndex = 0;
                resultOffset += bufferCount;
            }

            while (resultOffset < count)
            {
                int needCount = count - resultOffset;
                _bufferBytes = this.Function();

                if (needCount > _blockSize)
                { //we one (or more) additional passes
                    BytesOperations.Copy(_bufferBytes, result.AsSpan(resultOffset), _blockSize);
                    resultOffset += _blockSize;
                }
                else
                {
                    BytesOperations.Copy(_bufferBytes, result.AsSpan(resultOffset), needCount);
                    _bufferStartIndex = needCount;
                    _bufferEndIndex = _blockSize;

                    break;
                }
            }

            return result;
        }

        private byte[] Function()
        {
            if (_blockIndex == uint.MaxValue) throw new InvalidOperationException("Derived key too long.");

            byte[] currentHash;

            {
                var input = new byte[this.Salt.Length + 4];
                BytesOperations.Copy(this.Salt.Span, input, this.Salt.Length);
                BytesOperations.Copy(NetworkConverter.GetBytes(_blockIndex), input.AsSpan(this.Salt.Length), 4);

                _blockIndex++;

                currentHash = this.Algorithm.ComputeHash(input);
            }

            var finalHash = currentHash;

            for (int i = 2; i <= this.IterationCount; i++)
            {
                currentHash = this.Algorithm.ComputeHash(currentHash, 0, currentHash.Length);

                BytesOperations.Xor(finalHash, currentHash, finalHash);
            }

            return finalHash;
        }
    }
}
