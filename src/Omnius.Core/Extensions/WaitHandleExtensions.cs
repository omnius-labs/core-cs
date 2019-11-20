using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Extensions
{
    public static class WaitHandleExtensions
    {
        // http://www.thomaslevesque.com/2015/06/04/async-and-cancellation-support-for-wait-handles/
        public static bool WaitOne(this WaitHandle handle, int millisecondsTimeout, CancellationToken token = default)
        {
            int n = WaitHandle.WaitAny(new[] { handle, token.WaitHandle }, millisecondsTimeout);

            switch (n)
            {
                case WaitHandle.WaitTimeout:
                    return false;
                case 0:
                    return true;
                default:
                    token.ThrowIfCancellationRequested();
                    return false; // never reached
            }
        }

        public static bool WaitOne(this WaitHandle handle, TimeSpan timeout, CancellationToken token = default)
        {
            return handle.WaitOne((int)timeout.TotalMilliseconds, token);
        }

        public static bool WaitOne(this WaitHandle handle, CancellationToken token = default)
        {
            return handle.WaitOne(Timeout.Infinite, token);
        }

        // https://stackoverflow.com/questions/18756354/wrapping-manualresetevent-as-awaitable-task
        public static async Task WaitAsync(this WaitHandle handle, CancellationToken token = default)
        {
            var tcs = new TaskCompletionSource<object?>();

            var registration = ThreadPool.RegisterWaitForSingleObject(handle, (state, localTimeout) =>
            {
                var localTcs = (TaskCompletionSource<object?>)state!;

                if (localTimeout)
                {
                    localTcs.TrySetCanceled();
                }
                else
                {
                    localTcs.TrySetResult(null);
                }
            }, tcs, Timeout.InfiniteTimeSpan, executeOnlyOnce: true);

            using (token.Register(() => tcs.TrySetCanceled()))
            {
                await tcs.Task;
            }

            registration.Unregister(null);
        }
    }
}
