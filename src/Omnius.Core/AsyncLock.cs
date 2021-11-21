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

        public async ValueTask<IDisposable> LockAsync(CancellationToken cancellationToken = default)
        {
            var wait = _semaphore.WaitAsync();
            if (wait.IsCompleted) return _releaser;

            return await wait.ContinueWith(
                (_, state) => (IDisposable)state!,
                _releaser,
                cancellationToken,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _semaphore.Dispose();
            }
        }

        private readonly struct Releaser : IDisposable
        {
            private readonly AsyncLock _asyncLock;

            public Releaser(AsyncLock asyncLock)
            {
                _asyncLock = asyncLock;
            }

            public void Dispose()
            {
                _asyncLock._semaphore.Release();
            }
        }
    }
}
