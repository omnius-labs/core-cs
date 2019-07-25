using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Omnix.Algorithms.Correction;
using Omnix.Algorithms.Cryptography;
using Omnix.Base;
using Xunit;

namespace Omnix.Algorithms
{
    public class ReedSolomonTests
    {
        [Fact]
        public async Task EncodeTest()
        {
            int[] indexes = new int[128];
            Memory<byte>[] sources = new Memory<byte>[128];
            Memory<byte>[] repairs = new Memory<byte>[128];
            int PacketLength = 1024;

            var random = new Random();

            for (int i = 0; i < sources.Length; i++)
            {
                indexes[i] = i + 128;
                sources[i] = new byte[PacketLength];
                random.NextBytes(sources[i].Span);
            }

            for (int i = 0; i < repairs.Length; i++)
            {
                repairs[i] = new byte[PacketLength];
            }

            var r = new ReedSolomon(8, 128, 256, BufferPool.Shared);
            await r.Encode(sources.Select(n => (ReadOnlyMemory<byte>)n).ToArray(), indexes, repairs, PacketLength);
        }
    }
}
