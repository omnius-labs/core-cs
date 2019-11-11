using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Omnix.Base
{
    public class EventTimerTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async void MaxConcurrentExecutionsCountTest(int maxConcurrentExecutionsCount)
        {
            // 呼び出し回数
            int calledCount = 0;

            using var eventTimer = new EventTimer(async (token) =>
            {
                Interlocked.Increment(ref calledCount);

                // 1時間停止
                await Task.Delay(new TimeSpan(1, 0, 0), token);
            }, maxConcurrentExecutionsCount);

            eventTimer.Change(new TimeSpan(0, 0, 0), new TimeSpan(0, 0, 1));

            // Running状態に変更
            await eventTimer.StartAsync();

            await Task.Delay(1000 * 5);

            // 実行回数は「maxConcurrentExecutionsCount」回のはず
            Assert.Equal(calledCount, maxConcurrentExecutionsCount);
        }

        [Fact]
        public async void TimerTest()
        {
            // 呼び出し回数
            int calledCount = 0;

            using var eventTimer = new EventTimer(async (token) =>
            {
                await Task.Run(() =>
                {
                    Interlocked.Increment(ref calledCount);
                });
            });

            // 呼び出し間隔を1秒へ変更する
            eventTimer.Change(new TimeSpan(0, 0, 1), new TimeSpan(0, 0, 1));

            // 3秒待機する
            await Task.Delay(1000 * 3);

            // Stopped状態で3秒以内に一度も実行されないことを確認する
            Assert.Equal(0, calledCount);

            // Running状態に変更
            await eventTimer.StartAsync();

            // 3秒待機する
            await Task.Delay(1000 * 3);

            // 3秒以内に1回以上は実行されていてほしい
            Assert.True(calledCount >= 1);
        }
    }
}
