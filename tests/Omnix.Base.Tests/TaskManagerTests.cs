using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Omnix.Base.Tests
{
    public class TaskManagerTests
    {
        [Fact]
        public async void StartAndWaitTest()
        {
            {
                bool flag = false;

                var taskManager = new TaskManager((_) =>
                {
                    flag = true;
                });

                Assert.False(flag);

                await taskManager.Start();

                taskManager.Wait();

                Assert.True(flag);

                taskManager.Dispose();
            }

            {
                bool flag = false;

                var taskManager = new TaskManager((token) =>
                {
                    token.WaitHandle.WaitOne();
                    flag = true;
                });

                Assert.False(flag);

                await taskManager.Start();

                var task = Task.Run(async () =>
                {
                    Thread.Sleep(1000);
                    await taskManager.Stop();
                });

                taskManager.Wait();

                await task;

                Assert.True(flag);

                taskManager.Dispose();
            }
        }
    }
}
