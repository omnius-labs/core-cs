using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Omnix.Algorithms.Cryptography
{
    public class OmniMinerTests
    {
        [Fact]
        public async Task SimpleComputeTest()
        {
            var random = new Random();
            var value = new byte[1024];
            var key = new byte[1024];

            random.NextBytes(value);
            random.NextBytes(key);

            var hashcash = await OmniMiner.Create(new ReadOnlySequence<byte>(value), key, OmniHashcashAlgorithmType.Simple_Sha2_256, 1, TimeSpan.FromSeconds(100), CancellationToken.None);
            var cost = OmniMiner.Verify(hashcash, new ReadOnlySequence<byte>(value), key);

            // 計算後、コストは最低でも1以上になっているはず。
            Assert.True(cost >= 1);
        }
    }
}
