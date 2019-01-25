using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnix.Base
{
    /// <summary>
    /// <see cref="Task"/>の管理機能を提供します。
    /// </summary>
    public sealed class TaskManager : ServiceBase
    {
        private readonly Action<CancellationToken> _callback;

        private Task _task;
        private CancellationTokenSource _tokenSource;

        private ServiceStateType _state = ServiceStateType.Stopped;

        private readonly AsyncLock _asyncLock = new AsyncLock();
        private volatile bool _disposed;

        public TaskManager(Action<CancellationToken> callback)
        {
            _callback = callback;
        }

        public void Wait()
        {
            _task?.Wait();
        }

        public bool Wait(int millisecondsTimeout)
        {
            return _task?.Wait(millisecondsTimeout) ?? true;
        }

        public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return _task?.Wait(millisecondsTimeout, cancellationToken) ?? true;
        }

        public void Wait(CancellationToken cancellationToken)
        {
            _task?.Wait(cancellationToken);
        }

        public bool Wait(TimeSpan timeout)
        {
            return _task?.Wait(timeout) ?? true;
        }

        public override ServiceStateType StateType
        {
            get
            {
                return _state;
            }
        }

        internal void InternalStart()
        {
            if (this.StateType != ServiceStateType.Stopped) return;
            _state = ServiceStateType.Starting;

            _tokenSource = new CancellationTokenSource();
            _task = new Task(() =>
            {
                _callback(_tokenSource.Token);
            }, _tokenSource.Token, TaskCreationOptions.LongRunning);
            _task.Start();

            _state = ServiceStateType.Running;
        }

        internal void InternalStop()
        {
            if (this.StateType != ServiceStateType.Running) return;
            _state = ServiceStateType.Stopping;

            _tokenSource.Cancel();
            _task.Wait();

            _task.Dispose();
            _task = null;

            _tokenSource.Dispose();
            _tokenSource = null;

            _state = ServiceStateType.Stopped;
        }

        public override async ValueTask Start(CancellationToken token = default)
        {
            using (await _asyncLock.LockAsync())
            {
                this.InternalStart();
            }
        }

        public override async ValueTask Stop(CancellationToken token = default)
        {
            using (await _asyncLock.LockAsync())
            {
                this.InternalStop();
            }
        }

        public override async ValueTask Restart(CancellationToken token = default)
        {
            using (await _asyncLock.LockAsync())
            {
                this.InternalStop();
                this.InternalStart();
            }
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
