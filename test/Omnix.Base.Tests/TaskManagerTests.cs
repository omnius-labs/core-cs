using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Omnix.Base
{
    public class TaskManagerTests
    {
        [Fact]
        public async void StartAndWaitTest()
        {
            // Start
            {
                bool flag = false;

                var taskManager = new TaskManager(async (_) =>
                {
                    flag = true;
                });

                Assert.False(flag);

                taskManager.Start();

                if (taskManager.Task is null)
                {
                    throw new NullReferenceException();
                }

                await taskManager.Task;

                Assert.True(flag);

                taskManager.Dispose();
            }

            // Wait
            {
                bool flag = false;

                var taskManager = new TaskManager(async (token) =>
                {
                    token.WaitHandle.WaitOne();
                    flag = true;
                });

                Assert.False(flag);

                taskManager.Start();

                var task = Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    taskManager.Cancel();
                });

                if (taskManager.Task is null)
                {
                    throw new NullReferenceException();
                }

                await taskManager.Task;

                Assert.True(flag);

                taskManager.Dispose();
            }
        }
    }
}
