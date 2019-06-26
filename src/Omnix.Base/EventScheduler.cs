using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Omnix.Base;
using Omnix.Base.Extensions;

namespace Omnix.Base
{
    public sealed class EventScheduler : ServiceBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 実行されるメソッド 
        /// </summary>
        private readonly Func<CancellationToken, Task> _callback;

        /// <summary>
        /// 現在実行中のタスク
        /// </summary>
        private volatile Task? _currentTask;

        private TimeSpan? _interval;

        private Timer? _timer;
        private CancellationTokenSource? _tokenSource;

        private ServiceStateType _stateType = ServiceStateType.Stopped;

        private readonly AsyncLock _asyncLock = new AsyncLock();
        private readonly object _lockObject = new object();

        private volatile bool _disposed;

        public EventScheduler(Func<CancellationToken, Task> callback)
        {
            _callback = callback;
        }

        public override ServiceStateType StateType => _stateType;

        public TimeSpan? Interval => _interval;

        private async void Run()
        {
            using (await _asyncLock.LockAsync())
            {
                if (this.StateType != ServiceStateType.Running)
                {
                    return;
                }

                lock (_lockObject)
                {
                    if (_currentTask != null)
                    {
                        return;
                    }

                    var task = this.OnCallbackAsync(_tokenSource!.Token)
                        .ContinueWith((_) =>
                        {
                            lock (_lockObject)
                            {
                                _currentTask = null;
                            }
                        });

                    _currentTask = task;
                }
            }
        }

        private async Task OnCallbackAsync(CancellationToken token)
        {
            try
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                await _callback.Invoke(token);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        public void ExecuteImmediate()
        {
            this.Run();
        }

        public async void ChangeInterval(TimeSpan? interval)
        {
            using (await _asyncLock.LockAsync())
            {
                _interval = interval;

                if (this.StateType != ServiceStateType.Running)
                {
                    return;
                }

                _timer!.Change(interval?.Milliseconds ?? Timeout.Infinite);
            }
        }

        private async ValueTask InternalStart()
        {
            _stateType = ServiceStateType.Starting;

            _tokenSource = new CancellationTokenSource();
            _timer = new Timer((_) => this.Run(), null, Timeout.Infinite, Timeout.Infinite);
            _timer.Change(this.Interval?.Milliseconds ?? Timeout.Infinite);

            _stateType = ServiceStateType.Running;
        }

        private async ValueTask InternalStop()
        {
            _stateType = ServiceStateType.Stopping;

            _timer!.Dispose();
            _tokenSource!.Cancel();

            var currentTask = _currentTask;

            if (currentTask != null)
            {
                await currentTask;
            }

            _tokenSource!.Dispose();

            _timer = null;
            _tokenSource = null;

            _stateType = ServiceStateType.Stopped;
        }

        public override async ValueTask StartAsync()
        {
            using (await _asyncLock.LockAsync())
            {
                if (this.StateType != ServiceStateType.Stopped)
                {
                    return;
                }

                await this.InternalStart();
            }
        }

        public override async ValueTask StopAsync()
        {
            using (await _asyncLock.LockAsync())
            {
                if (this.StateType != ServiceStateType.Running)
                {
                    return;
                }

                await this.InternalStop();
            }
        }

        public override async ValueTask RestartAsync()
        {
            using (await _asyncLock.LockAsync())
            {
                await this.InternalStop();
                await this.InternalStart();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (disposing)
            {
                this.StopAsync().AsTask().Wait();
            }
        }
    }
}
