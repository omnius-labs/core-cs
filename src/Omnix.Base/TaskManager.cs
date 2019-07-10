using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnix.Base
{
    /// <summary>
    /// <see cref="Task"/>の管理機能を提供します。
    /// </summary>
    public sealed class TaskManager : DisposableBase
    {
        private readonly Action<CancellationToken> _callback;

        private Task? _task;
        private CancellationTokenSource? _tokenSource;

        private readonly AsyncLock _asyncLock = new AsyncLock();

        public TaskManager(Action<CancellationToken> callback)
        {
            _callback = callback;
        }

        public Task? Task => _task;

        public void Start()
        {
            _tokenSource = new CancellationTokenSource();
            _task = new Task((state) =>
            {
                var tokenSource = (CancellationTokenSource)state;
                _callback(tokenSource.Token);
            }, _tokenSource, _tokenSource.Token, TaskCreationOptions.LongRunning);
            _task.Start();
        }

        public void Cancel()
        {
            _tokenSource?.Cancel();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _task?.Dispose();
                _tokenSource?.Dispose();
            }
        }
    }
}
