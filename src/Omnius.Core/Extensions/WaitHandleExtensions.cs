using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Extensions
{
    public static class WaitHandleExtensions
    {
        // http://www.thomaslevesque.com/2015/06/04/async-and-cancellation-support-for-wait-handles/
        public static bool WaitOne(this WaitHandle handle, CancellationToken cancellationToken = default)
        {
            return WaitOne(handle, Timeout.InfiniteTimeSpan, cancellationToken);
        }

        public static bool WaitOne(this WaitHandle handle, int millisecondsTimeout, CancellationToken cancellationToken = default)
        {
            return WaitOne(handle, TimeSpan.FromMilliseconds(millisecondsTimeout), cancellationToken);
        }

        public static bool WaitOne(this WaitHandle handle, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            int n = WaitHandle.WaitAny(new[] { handle, cancellationToken.WaitHandle }, timeout);

            switch (n)
            {
                case WaitHandle.WaitTimeout:
                    return false;
                case 0:
                    return true;
                default:
                    cancellationToken.ThrowIfCancellationRequested();
                    return false; // never reached
            }
        }

        // https://stackoverflow.com/questions/18756354/wrapping-manualresetevent-as-awaitable-task
        public static async Task WaitAsync(this WaitHandle handle, CancellationToken cancellationToken = default)
        {
            await WaitAsync(handle, Timeout.InfiniteTimeSpan, cancellationToken);
        }

        public static async Task WaitAsync(this WaitHandle handle, int millisecondsTimeout, CancellationToken cancellationToken = default)
        {
            await WaitAsync(handle, TimeSpan.FromMilliseconds(millisecondsTimeout), cancellationToken);
        }

        public static async Task WaitAsync(this WaitHandle handle, TimeSpan timeout, CancellationToken cancellationToken = default)
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
            }, tcs, timeout, executeOnlyOnce: true);

            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                await tcs.Task;
            }

            registration.Unregister(null);
        }
    }
}
