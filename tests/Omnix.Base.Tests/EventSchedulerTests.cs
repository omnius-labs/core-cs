using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Omnix.Base
{
    public class EventSchedulerTests
    {
        [Fact]
        public async void ExecuteImmediateTest()
        {
            // 呼び出し回数
            int count = 0;

            using var eventScheduler = new EventScheduler((token) =>
            {
                Interlocked.Increment(ref count);
            });

            // Stopped状態で実行されないことを確認する
            eventScheduler.ExecuteImmediate();

            // 実行されるまで待機 (3秒以内には実行されてほしい)
            await Task.Delay(1000 * 3);

            // 実行されないことを確認
            Assert.True(count == 0);

            // Running状態に変更
            await eventScheduler.Start();

            // 実行されることを確認する
            eventScheduler.ExecuteImmediate();

            // 実行されるまで待機 (3秒以内には実行されてほしい)
            await Task.Delay(1000 * 3);

            // 実行されることを確認
            Assert.True(count == 1);
        }

        [Fact]
        public async void TimerTest()
        {
            // 呼び出し回数
            int count = 0;

            using var eventScheduler = new EventScheduler((token) =>
            {
                Interlocked.Increment(ref count);
            });

            // 呼び出し間隔を1秒へ変更する
            eventScheduler.ChangeInterval(new TimeSpan(0, 0, 1));

            // 3秒待機する
            await Task.Delay(1000 * 3);

            // Stopped状態で3秒以内に一度も実行されないことを確認する
            Assert.True(count == 0);

            // Running状態に変更
            await eventScheduler.Start();

            // 3秒待機する
            await Task.Delay(1000 * 3);

            // 3秒以内に1回以上は実行されていてほしい
            Assert.True(count >= 1);
        }
    }
}
