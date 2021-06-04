using System;
using System.Buffers;

namespace Omnius.Core.Serialization
{
    // https://github.com/bitcoin/bitcoin/blob/master/src/base58.cpp

    /// <summary>
    /// Bitcoin形式のBase58コンバーターです。
    /// </summary>
    public unsafe class Base58Btc : IBytesToUtf8StringConverter
    {
        /** All alphanumeric characters except for "0", "I", "O", and "l" */
        private static readonly string _base58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        private static readonly int[] _base58Map = new int[128] {
            -1,-1,-1,-1,-1,-1,-1,-1, -1,-1,-1,-1,-1,-1,-1,-1,
            -1,-1,-1,-1,-1,-1,-1,-1, -1,-1,-1,-1,-1,-1,-1,-1,
            -1,-1,-1,-1,-1,-1,-1,-1, -1,-1,-1,-1,-1,-1,-1,-1,
            -1, 0, 1, 2, 3, 4, 5, 6,  7, 8,-1,-1,-1,-1,-1,-1,
            -1, 9,10,11,12,13,14,15, 16,-1,17,18,19,20,21,-1,
            22,23,24,25,26,27,28,29, 30,31,32,-1,-1,-1,-1,-1,
            -1,33,34,35,36,37,38,39, 40,41,42,43,-1,44,45,46,
            47,48,49,50,51,52,53,54, 55,56,57,-1,-1,-1,-1,-1,
        };

        public bool TryEncode(ReadOnlySequence<byte> sequence, out byte[] text, bool includePrefix = false)
        {
            text = Array.Empty<byte>();

            if (sequence.IsEmpty) return true;

            // Skip & count leading zeroes.
            int zeroCount = 0;

            foreach (var segment in sequence)
            {
                fixed (byte* p_segment_fixed = segment.Span)
                {
                    byte* p_segment_start = p_segment_fixed;
                    byte* p_segment_end = p_segment_fixed + segment.Length;

                    while (p_segment_start != p_segment_end && *p_segment_start == 0)
                    {
                        zeroCount++;
                        p_segment_start++;
                    }
                }
            }

            int length = 0;

            // Allocate enough space in big-endian base58 representation.
            int b58Length = ((int)(sequence.Length - zeroCount) * 138 / 100) + 1; // log(256) / log(58), rounded up.

            using (var b58 = MemoryPool<byte>.Shared.Rent(b58Length))
            {
                BytesOperations.Zero(b58.Memory.Span);

                fixed (byte* p_b58_fixed = b58.Memory.Span)
                {
                    // Process the bytes.
                    foreach (var segment in sequence.Slice(zeroCount))
                    {
                        fixed (byte* p_segment_fixed = segment.Span)
                        {
                            byte* p_segment_start = p_segment_fixed;
                            byte* p_segment_end = p_segment_fixed + segment.Length;

                            while (p_segment_start != p_segment_end)
                            {
                                int carry = *p_segment_start;
                                int i = 0;

                                // Apply "b58 = b58 * 256 + ch".
                                byte* p_b58_start = (p_b58_fixed + b58Length) - 1;
                                byte* p_b58_end = p_b58_fixed - 1;

                                while (p_b58_start != p_b58_end && (carry != 0 || i < length))
                                {
                                    carry += 256 * (*p_b58_start);
                                    *p_b58_start = (byte)(carry % 58);
                                    carry /= 58;

                                    p_b58_start--;
                                    i++;
                                }

                                length = i;
                                p_segment_start++;
                            }
                        }
                    }

                    {
                        byte* p_b58_start = p_b58_fixed + (b58Length - length);
                        byte* p_b58_end = p_b58_start + length;

                        // Skip leading zeroes in base58 result.
                        while (p_b58_start != p_b58_end && *p_b58_start == 0)
                        {
                            p_b58_start++;
                        }

                        var result = new byte[(includePrefix ? 1 : 0) + zeroCount + (p_b58_end - p_b58_start)];

                        fixed (byte* p_result_fixed = result)
                        {
                            byte* p_result_start = p_result_fixed;

                            if (includePrefix)
                            {
                                *p_result_start++ = (byte)'z';
                            }

                            for (int i = zeroCount - 1; i >= 0; i--)
                            {
                                *p_result_start++ = (byte)'1';
                            }

                            while (p_b58_start != p_b58_end)
                            {
                                *p_result_start++ = (byte)_base58Chars[*p_b58_start++];
                            }
                        }

                        text = result;
                    }
                }
            }

            return true;
        }

        public bool TryDecode(ReadOnlySpan<byte> text, IBufferWriter<byte> bufferWriter)
        {
            if (bufferWriter == null) throw new ArgumentNullException(nameof(bufferWriter));

            if (text.IsEmpty) return true;

            // Skip and count leading '1's.
            int zeroCount = 0;

            fixed (byte* p_text_fixed = text)
            {
                byte* p_text_start = p_text_fixed;
                byte* p_text_end = p_text_fixed + text.Length;

                while (p_text_start != p_text_end && *p_text_start == '1')
                {
                    zeroCount++;
                    p_text_start++;
                }
            }

            int length = 0;

            // Allocate enough space in big-endian base256 representation.
            int b256Length = ((text.Length - zeroCount) * 733 / 1000) + 1; // log(58) / log(256), rounded up.

            using (var b256 = MemoryPool<byte>.Shared.Rent(b256Length))
            {
                BytesOperations.Zero(b256.Memory.Span);

                fixed (byte* p_b256_fixed = b256.Memory.Span)
                {
                    // Process the characters.
                    fixed (byte* p_text_fixed = text)
                    {
                        byte* p_text_start = p_text_fixed + zeroCount;
                        byte* p_text_end = p_text_fixed + text.Length;

                        while (p_text_start != p_text_end)
                        {
                            if (*p_text_start >= 128)
                            {
                                return false; // Invalid b58 character
                            }

                            // Decode base58 character
                            int carry = _base58Map[*p_text_start];
                            if (carry == -1)
                            {
                                return false; // Invalid b58 character
                            }

                            int i = 0;

                            byte* p_b256_start = (p_b256_fixed + b256Length) - 1;
                            byte* p_b256_end = p_b256_fixed - 1;

                            while (p_b256_start != p_b256_end && (carry != 0 || i < length))
                            {
                                carry += 58 * (*p_b256_start);
                                *p_b256_start = (byte)(carry % 256);
                                carry /= 256;

                                p_b256_start--;
                                i++;
                            }

                            length = i;
                            p_text_start++;
                        }
                    }

                    {
                        byte* p_b256_start = p_b256_fixed;
                        byte* p_b256_end = p_b256_fixed + b256Length;

                        // Skip leading zeroes in b256.
                        while (p_b256_start != p_b256_end && *p_b256_start == 0)
                        {
                            p_b256_start++;
                        }

                        // Zero fill.
                        if (zeroCount != 0)
                        {
                            var buffer = bufferWriter.GetSpan(zeroCount);
                            BytesOperations.Zero(buffer.Slice(0, zeroCount));
                            bufferWriter.Advance(zeroCount);
                        }

                        // Copy result into output vector.
                        while (p_b256_start != p_b256_end)
                        {
                            var destinationBuffer = bufferWriter.GetSpan();
                            var sourceBuffer = new ReadOnlySpan<byte>(p_b256_start, (int)(p_b256_end - p_b256_start));
                            var minLength = Math.Min(sourceBuffer.Length, destinationBuffer.Length);

                            BytesOperations.Copy(sourceBuffer, destinationBuffer, minLength);
                            bufferWriter.Advance(minLength);

                            p_b256_start += minLength;
                        }
                    }

                    return true;
                }
            }
        }
    }
}
