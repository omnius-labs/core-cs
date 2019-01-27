using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Omnix.Cryptography.Tests
{
    public class OmniMinerTests
    {
        [Fact]
        public async Task ComputeTest()
        {
            var random = new Random();
            var value = new byte[1024];
            var key = new byte[1024];

            random.NextBytes(value);
            random.NextBytes(key);

            var hashcash = await OmniMiner.Create(new ReadOnlySequence<byte>(value), key, OmniHashcashAlgorithmType.Sha2_256, 1, TimeSpan.FromSeconds(10), CancellationToken.None);
            var cost = OmniMiner.Verify(hashcash, new ReadOnlySequence<byte>(value), key);

            Assert.True(cost >= 1);
        }
    }
}
