using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnix.Base.Extensions
{
    public static class WaitHandleExtensions
    {
        // http://www.thomaslevesque.com/2015/06/04/async-and-cancellation-support-for-wait-handles/
        public static bool WaitOne(this WaitHandle handle, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            int n = WaitHandle.WaitAny(new[] { handle, cancellationToken.WaitHandle }, millisecondsTimeout);

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

        public static bool WaitOne(this WaitHandle handle, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return handle.WaitOne((int)timeout.TotalMilliseconds, cancellationToken);
        }

        public static bool WaitOne(this WaitHandle handle, CancellationToken cancellationToken)
        {
            return handle.WaitOne(Timeout.Infinite, cancellationToken);
        }

        // https://stackoverflow.com/questions/18756354/wrapping-manualresetevent-as-awaitable-task
        public static Task AsTask(this WaitHandle handle)
        {
            return AsTask(handle, Timeout.InfiniteTimeSpan);
        }

        public static Task AsTask(this WaitHandle handle, TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<object?>();

            var registration = ThreadPool.RegisterWaitForSingleObject(handle, (state, localTimeout) =>
            {
                var localTcs = (TaskCompletionSource<object?>)state;

                if (localTimeout)
                {
                    localTcs.TrySetCanceled();
                }
                else
                {
                    localTcs.TrySetResult(null);
                }
            }, tcs, timeout, executeOnlyOnce: true);
            tcs.Task.ContinueWith((_, state) => ((RegisteredWaitHandle)state).Unregister(null), registration, TaskScheduler.Default);

            return tcs.Task;
        }
    }
}
