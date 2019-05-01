// https://github.com/lontivero/gcs/blob/master/src/SipHash.cs
// MIT License

using System;
using System.Runtime.CompilerServices;
using System.Buffers;

namespace Omnix.Common
{
    internal class SipHasher
    {
#pragma warning disable IDE1006 // Naming Styles
        private ulong v_0;
        private ulong v_1;
        private ulong v_2;
        private ulong v_3;
        private ulong _count;
        private ulong _tmp;
#pragma warning restore IDE1006 // Naming Styles

        public SipHasher(ReadOnlySpan<byte> key) : this(BitConverter.ToUInt64(key), BitConverter.ToUInt64(key.Slice(8)))
        {
        }

        public SipHasher(ulong k0, ulong k1)
        {
            v_0 = 0x736f6d6570736575UL ^ k0;
            v_1 = 0x646f72616e646f6dUL ^ k1;
            v_2 = 0x6c7967656e657261UL ^ k0;
            v_3 = 0x7465646279746573UL ^ k1;
            _count = 0;
            _tmp = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong Rotl64(in ulong x, in byte b)
        {
            return (((x) << (b)) | ((x) >> (64 - (b))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Round(ref ulong v_0, ref ulong v_1, ref ulong v_2, ref ulong v_3)
        {
            v_0 += v_1;
            v_1 = Rotl64(v_1, 13);
            v_1 ^= v_0;
            v_0 = Rotl64(v_0, 32);
            v_2 += v_3;
            v_3 = Rotl64(v_3, 16);
            v_3 ^= v_2;
            v_0 += v_3;
            v_3 = Rotl64(v_3, 21);
            v_3 ^= v_0;
            v_2 += v_1;
            v_1 = Rotl64(v_1, 17);
            v_1 ^= v_2;
            v_2 = Rotl64(v_2, 32);
        }

        public void Write(ReadOnlySequence<byte> sequence)
        {
            foreach (var segment in sequence)
            {
                this.Write(segment.Span);
            }
        }

        public void Write(ReadOnlySpan<byte> data)
        {
            ulong v0 = v_0, v1 = v_1, v2 = v_2, v3 = v_3;
            var size = data.Length;
            var t = _tmp;
            var c = _count;
            int offset = 0;

            while (size-- != 0)
            {
                t |= ((ulong)((data[offset++]))) << (int)(8 * (c % 8));
                c++;
                if ((c & 7) == 0)
                {
                    v3 ^= t;
                    Round(ref v0, ref v1, ref v2, ref v3);
                    Round(ref v0, ref v1, ref v2, ref v3);
                    v0 ^= t;
                    t = 0;
                }
            }

            v_0 = v0;
            v_1 = v1;
            v_2 = v2;
            v_3 = v3;
            _count = c;
            _tmp = t;

            return;
        }

        public ulong Finalize()
        {
            ulong v0 = v_0, v1 = v_1, v2 = v_2, v3 = v_3;

            ulong t = _tmp | (((ulong)_count) << 56);

            v3 ^= t;
            Round(ref v0, ref v1, ref v2, ref v3);
            Round(ref v0, ref v1, ref v2, ref v3);
            v0 ^= t;
            v2 ^= 0xFF;
            Round(ref v0, ref v1, ref v2, ref v3);
            Round(ref v0, ref v1, ref v2, ref v3);
            Round(ref v0, ref v1, ref v2, ref v3);
            Round(ref v0, ref v1, ref v2, ref v3);
            return v0 ^ v1 ^ v2 ^ v3;
        }

        public static ulong Hash(byte[] key, byte[] data)
        {
            var k0 = BitConverter.ToUInt64(key, 0);
            var k1 = BitConverter.ToUInt64(key, 8);

            var hasher = new SipHasher(k0, k1);
            hasher.Write(data);
            return hasher.Finalize();
        }
    }
}
