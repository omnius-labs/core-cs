using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core
{
    public sealed class AsyncLock : DisposableBase
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IDisposable _releaser;

        public AsyncLock()
        {
            _releaser = new Releaser(this);
        }

        public async ValueTask<IDisposable> LockAsync()
        {
            var wait = _semaphore.WaitAsync();

            if (wait.IsCompleted)
            {
                return _releaser;
            }

            return await wait.ContinueWith(
                (_, state) => (IDisposable)state!,
                _releaser,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default
            );
        }

        protected override void OnDispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            _semaphore.Dispose();
        }

        private readonly struct Releaser : IDisposable
        {
            private readonly AsyncLock _asynkLock;

            public Releaser(AsyncLock asynkLock)
            {
                _asynkLock = asynkLock;
            }

            public void Dispose()
            {
                _asynkLock._semaphore.Release();
            }
        }
    }
}
