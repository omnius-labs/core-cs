using Omnix.Base;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Omnix.Cryptography
{
    /// <summary>
    /// This class is immutable and thread-safe.
    /// </summary>
    public sealed class SipHash
    {
        #region Fields

        /// <summary>
        /// Part of the initial 256-bit internal state.
        /// </summary>
        private readonly ulong _initialState0;

        /// <summary>
        /// Part of the initial 256-bit internal state.
        /// </summary>
        private readonly ulong _initialState1;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of SipHash pseudo-random function using the specified 128-bit key.</summary>
        /// <param name="key"><para>Key for the SipHash pseudo-random function.</para><para>Must be exactly 16 bytes long and must not be null.</para></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="key"/> is not exactly 16 bytes long (128 bits).</exception>
        public SipHash(ReadOnlySpan<byte> key)
        {
            if (key.Length != 16) throw new ArgumentException("SipHash key must be exactly 128-bit long (16 bytes).", nameof(key));

            _initialState0 = 0x736f6d6570736575UL ^ BitConverter.ToUInt64(key);
            _initialState1 = 0x646f72616e646f6dUL ^ BitConverter.ToUInt64(key.Slice(sizeof(ulong)));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a 128-bit SipHash key.
        /// </summary>
        public ReadOnlyMemory<byte> Key
        {
            get
            {
                var key = new byte[16];

                unsafe
                {
                    fixed (byte* pKey = key)
                    {
                        var pKey64 = (ulong*)pKey;
                        pKey64[0] = _initialState0 ^ 0x736f6d6570736575UL;
                        pKey64[1] = _initialState1 ^ 0x646f72616e646f6dUL;
                    }
                }

                return key;
            }
        }

        #endregion

        #region Methods

        /// <summary>Computes 64-bit SipHash tag for the specified message.</summary>
        /// <param name="data"><para>The byte array for which to computer SipHash tag.</para><para>Must not be null.</para></param>
        /// <param name="offset"><para>The zero-based index of the first element in the range.</para><para>Must not be negative.</para></param>
        /// <param name="count"><para>The number of elements in the range.</para><para>Must not be negative.</para></param>
        /// <returns>Returns 64-bit (8 bytes) SipHash tag.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="offset"/> or <paramref name="count"/> is negative.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="offset"/> and <paramref name="count"/> do not specify a valid range in <paramref name="data"/>.</exception>
        public long ComputeHash(ReadOnlySpan<byte> data)
        {
            // SipHash internal state
            var v0 = _initialState0;
            var v1 = _initialState1;
            // It is faster to load the initialStateX fields from memory again than to reference v0 and v1:
            var v2 = 0x1F160A001E161714UL ^ _initialState0;
            var v3 = 0x100A160317100A1EUL ^ _initialState1;

            // We process data in 64-bit blocks
            ulong block;

            unsafe
            {
                // Start of the data to process
                fixed (byte* dataStart = data)
                {
                    // The last 64-bit block of data
                    var finalBlock = dataStart + (data.Length & ~7);

                    // Process the input data in blocks of 64 bits
                    for (var blockPointer = (ulong*)dataStart; blockPointer < finalBlock;)
                    {
                        block = *blockPointer++;

                        v3 ^= block;

                        // Round 1
                        v0 += v1;
                        v2 += v3;
                        v1 = v1 << 13 | v1 >> 51;
                        v3 = v3 << 16 | v3 >> 48;
                        v1 ^= v0;
                        v3 ^= v2;
                        v0 = v0 << 32 | v0 >> 32;
                        v2 += v1;
                        v0 += v3;
                        v1 = v1 << 17 | v1 >> 47;
                        v3 = v3 << 21 | v3 >> 43;
                        v1 ^= v2;
                        v3 ^= v0;
                        v2 = v2 << 32 | v2 >> 32;

                        // Round 2
                        v0 += v1;
                        v2 += v3;
                        v1 = v1 << 13 | v1 >> 51;
                        v3 = v3 << 16 | v3 >> 48;
                        v1 ^= v0;
                        v3 ^= v2;
                        v0 = v0 << 32 | v0 >> 32;
                        v2 += v1;
                        v0 += v3;
                        v1 = v1 << 17 | v1 >> 47;
                        v3 = v3 << 21 | v3 >> 43;
                        v1 ^= v2;
                        v3 ^= v0;
                        v2 = v2 << 32 | v2 >> 32;

                        v0 ^= block;
                    }

                    // Load the remaining bytes
                    block = (ulong)data.Length << 56;
                    switch (data.Length & 7)
                    {
                        case 7:
                            block |= *(uint*)finalBlock | (ulong)*(ushort*)(finalBlock + 4) << 32 | (ulong)*(finalBlock + 6) << 48;
                            break;
                        case 6:
                            block |= *(uint*)finalBlock | (ulong)*(ushort*)(finalBlock + 4) << 32;
                            break;
                        case 5:
                            block |= *(uint*)finalBlock | (ulong)*(finalBlock + 4) << 32;
                            break;
                        case 4:
                            block |= *(uint*)finalBlock;
                            break;
                        case 3:
                            block |= *(ushort*)finalBlock | (ulong)*(finalBlock + 2) << 16;
                            break;
                        case 2:
                            block |= *(ushort*)finalBlock;
                            break;
                        case 1:
                            block |= *finalBlock;
                            break;
                    }
                }
            }

            // Process the final block
            {
                v3 ^= block;

                // Round 1
                v0 += v1;
                v2 += v3;
                v1 = v1 << 13 | v1 >> 51;
                v3 = v3 << 16 | v3 >> 48;
                v1 ^= v0;
                v3 ^= v2;
                v0 = v0 << 32 | v0 >> 32;
                v2 += v1;
                v0 += v3;
                v1 = v1 << 17 | v1 >> 47;
                v3 = v3 << 21 | v3 >> 43;
                v1 ^= v2;
                v3 ^= v0;
                v2 = v2 << 32 | v2 >> 32;

                // Round 2
                v0 += v1;
                v2 += v3;
                v1 = v1 << 13 | v1 >> 51;
                v3 = v3 << 16 | v3 >> 48;
                v1 ^= v0;
                v3 ^= v2;
                v0 = v0 << 32 | v0 >> 32;
                v2 += v1;
                v0 += v3;
                v1 = v1 << 17 | v1 >> 47;
                v3 = v3 << 21 | v3 >> 43;
                v1 ^= v2;
                v3 ^= v0;
                v2 = v2 << 32 | v2 >> 32;

                v0 ^= block;
                v2 ^= 0xff;
            }

            // 4 finalization rounds
            {
                // Round 1
                v0 += v1;
                v2 += v3;
                v1 = v1 << 13 | v1 >> 51;
                v3 = v3 << 16 | v3 >> 48;
                v1 ^= v0;
                v3 ^= v2;
                v0 = v0 << 32 | v0 >> 32;
                v2 += v1;
                v0 += v3;
                v1 = v1 << 17 | v1 >> 47;
                v3 = v3 << 21 | v3 >> 43;
                v1 ^= v2;
                v3 ^= v0;
                v2 = v2 << 32 | v2 >> 32;

                // Round 2
                v0 += v1;
                v2 += v3;
                v1 = v1 << 13 | v1 >> 51;
                v3 = v3 << 16 | v3 >> 48;
                v1 ^= v0;
                v3 ^= v2;
                v0 = v0 << 32 | v0 >> 32;
                v2 += v1;
                v0 += v3;
                v1 = v1 << 17 | v1 >> 47;
                v3 = v3 << 21 | v3 >> 43;
                v1 ^= v2;
                v3 ^= v0;
                v2 = v2 << 32 | v2 >> 32;

                // Round 3
                v0 += v1;
                v2 += v3;
                v1 = v1 << 13 | v1 >> 51;
                v3 = v3 << 16 | v3 >> 48;
                v1 ^= v0;
                v3 ^= v2;
                v0 = v0 << 32 | v0 >> 32;
                v2 += v1;
                v0 += v3;
                v1 = v1 << 17 | v1 >> 47;
                v3 = v3 << 21 | v3 >> 43;
                v1 ^= v2;
                v3 ^= v0;
                v2 = v2 << 32 | v2 >> 32;

                // Round 4
                v0 += v1;
                v2 += v3;
                v1 = v1 << 13 | v1 >> 51;
                v3 = v3 << 16 | v3 >> 48;
                v1 ^= v0;
                v3 ^= v2;
                v0 = v0 << 32 | v0 >> 32;
                v2 += v1;
                v0 += v3;
                v1 = v1 << 17 | v1 >> 47;
                v3 = v3 << 21 | v3 >> 43;
                v1 ^= v2;
                v3 ^= v0;
                v2 = v2 << 32 | v2 >> 32;
            }

            return (long)((v0 ^ v1) ^ (v2 ^ v3));
        }

        public long ComputeHash(ReadOnlySequence<byte> sequence)
        {
            using (var memoryOwner = BufferPool.Shared.Rent((int)sequence.Length))
            {
                sequence.CopyTo(memoryOwner.Memory.Span);

                return this.ComputeHash(memoryOwner.Memory.Span);
            }
        }

        #endregion
    }
}
