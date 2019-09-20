using System;
using System.Buffers;
using System.Linq;
using System.Threading.Tasks;
using Omnix.Algorithms.Internal;
using Omnix.Base;
using Omnix.Base.Extensions;
using Xunit;

namespace Omnix.Algorithms.Correction
{
    public class ReedSolomon8Tests
    {
        private static readonly Random _random = new Random();

        private async Task RandomEncodeAndDecode()
        {
            int blockLength = _random.Next(2, 128);
            int maxBlockCount = 128;

            var sources = new Memory<byte>[_random.Next(1, maxBlockCount)];
            var repairs = new Memory<byte>[_random.Next(1, maxBlockCount)];
            var results = new Memory<byte>[sources.Length];

            for (int i = 0; i < sources.Length; i++)
            {
                sources[i] = new byte[blockLength];
                _random.NextBytes(sources[i].Span);
            }

            for (int i = 0; i < repairs.Length; i++)
            {
                repairs[i] = new byte[blockLength];
            }

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = new byte[blockLength];
            }

            var r = new ReedSolomon8(sources.Length, sources.Length + repairs.Length, BufferPool<byte>.Shared);

            // Encode
            {
                int[] indexes = new int[repairs.Length];

                for (int i = 0; i < repairs.Length; i++)
                {
                    indexes[i] = sources.Length + i;
                }

                await r.Encode(sources.Select(n => (ReadOnlyMemory<byte>)n).ToArray(), indexes, repairs, blockLength);
            }

            // Decode
            {
                int[] indexes = new int[sources.Length];

                foreach (var (i, n) in Enumerable.Range(0, sources.Length + repairs.Length).Randomize().Take(sources.Length).Select((n, i) => (i, n)))
                {
                    indexes[i] = n;

                    if (n < sources.Length)
                    {
                        sources[n].CopyTo(results[i]);
                    }
                    else
                    {
                        repairs[n - sources.Length].CopyTo(results[i]);
                    }
                }

                await r.Decode(results, indexes, blockLength);

                for (int i = 0; i < sources.Length; i++)
                {
                    Assert.True(BytesOperations.SequenceEqual(sources[i].Span, results[i].Span));
                }
            }
        }

        [Fact]
        public async Task RandomEncodeAndDecodeTest()
        {
            try
            {
                Assert.True(NativeMethods.ReedSolomon8.TryLoadNativeMethods());

                for (int i = 0; i < 128; i++)
                {
                    await this.RandomEncodeAndDecode();
                }

                NativeMethods.ReedSolomon8.LoadPureUnsafeMethods();

                for (int i = 0; i < 128; i++)
                {
                    await this.RandomEncodeAndDecode();
                }
            }
            finally
            {
                Assert.True(NativeMethods.ReedSolomon8.TryLoadNativeMethods());
            }
        }
    }
}
