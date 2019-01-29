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

        private Task _task;
        private CancellationTokenSource _tokenSource;

        private readonly AsyncLock _asyncLock = new AsyncLock();
        private volatile bool _disposed;

        public TaskManager(Action<CancellationToken> callback)
        {
            _callback = callback;
        }

        public Task Task => _task;

        public void Start()
        {
            _tokenSource = new CancellationTokenSource();
            _task = new Task(() =>
            {
                _callback(_tokenSource.Token);
            }, _tokenSource.Token, TaskCreationOptions.LongRunning);
            _task.Start();
        }

        public void Cancel()
        {
            _tokenSource.Cancel();
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {
                if (_task != null)
                {
                    _task.Dispose();
                    _task = null;
                }

                if (_tokenSource != null)
                {
                    _tokenSource.Dispose();
                    _tokenSource = null;
                }
            }
        }
    }
}
