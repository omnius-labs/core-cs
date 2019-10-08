using System;
using System.Buffers;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Omnix.Algorithms.Cryptography
{
    public class OmniMinerTests
    {
        [Fact]
        public async Task SimpleTest()
        {
            var random = new Random();
            var value = new byte[1024];
            var key = new byte[1024];

            random.NextBytes(value);
            random.NextBytes(key);

            var hashcash = await OmniMiner.Create(new ReadOnlySequence<byte>(value), key, OmniHashcashAlgorithmType.Simple_Sha2_256, 1, TimeSpan.FromSeconds(10), CancellationToken.None);
            var cost = OmniMiner.Verify(hashcash, new ReadOnlySequence<byte>(value), key);

            // コストは最低でも1以上になっているはず。
            Assert.True(cost >= 1);
        }

        [Fact]
        public async Task TimeoutTest()
        {
            var random = new Random();
            var value = new byte[1024];
            var key = new byte[1024];

            random.NextBytes(value);
            random.NextBytes(key);

            var sw = Stopwatch.StartNew();

            var hashcash = await OmniMiner.Create(new ReadOnlySequence<byte>(value), key, OmniHashcashAlgorithmType.Simple_Sha2_256, 256, TimeSpan.FromSeconds(10), CancellationToken.None);
            var cost = OmniMiner.Verify(hashcash, new ReadOnlySequence<byte>(value), key);

            // 計算時間は10秒以上のはず。
            Assert.True(sw.Elapsed.TotalSeconds > 10);
        }
    }
}
