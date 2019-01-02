using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Omnix.Cryptography.HashcashComputer
{
    unsafe static class OmniHashcash_Sha256
    {
        public static byte[] Compute(byte[] value, int limit, TimeSpan timeout, CancellationToken token)
        {
            var stopwatch = Stopwatch.StartNew();
            var xorshift = new Xorshift4096();

            var finalResult = new byte[32];
            int finalCount = -1;

            Span<byte> buffer = stackalloc byte[64];
            value.CopyTo(buffer.Slice(32));

            Span<byte> result = stackalloc byte[32];

            using (var sha2_256 = SHA256.Create())
            {
                for (long loopCount = 0; ; loopCount++)
                {
                    if (loopCount % 256 == 0)
                    {
                        token.ThrowIfCancellationRequested();
                        if (stopwatch.Elapsed > timeout) break;
                    }

                    fixed (byte* p = buffer)
                    {
                        ((ulong*)p)[0] = xorshift.Next();
                        ((ulong*)p)[1] = xorshift.Next();
                        ((ulong*)p)[2] = xorshift.Next();
                        ((ulong*)p)[3] = xorshift.Next();
                        ((ulong*)p)[4] = xorshift.Next();
                        ((ulong*)p)[5] = xorshift.Next();
                        ((ulong*)p)[6] = xorshift.Next();
                        ((ulong*)p)[7] = xorshift.Next();
                    }

                    if (!sha2_256.TryComputeHash(buffer, result, out _)) throw new FormatException();

                    int count = 0;

                    for (int i = 0; i < result.Length; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (((result[i] << j) & 0x80) == 0) count++;
                            else goto End;
                        }
                    }
                End:;

                    if (finalCount < count)
                    {
                        buffer.Slice(0, 32).CopyTo(finalResult);
                        finalCount = count;

                        if (finalCount >= limit) break;
                    }
                }
            }

            return finalResult;
        }
    }
}
