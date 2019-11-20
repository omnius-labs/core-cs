using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Extensions;

namespace Omnius.Core
{
    public sealed class EventTimer : ServiceBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 実行されるメソッド
        /// </summary>
        private readonly Func<CancellationToken, ValueTask> _callback;

        private Timer? _timer;
        private TimeSpan _dueTime;
        private TimeSpan _period;

        private readonly int _maxConcurrentExecutionsCount;
        private readonly SemaphoreSlim _semaphore;

        private CancellationTokenSource? _tokenSource;

        private ServiceStateType _stateType = ServiceStateType.Stopped;

        private readonly AsyncLock _asyncLock = new AsyncLock();
        private readonly object _lockObject = new object();

        public EventTimer(Func<CancellationToken, ValueTask> callback, int maxConcurrentExecutionsCount = 1)
        {
            if (maxConcurrentExecutionsCount < 1) throw new ArgumentOutOfRangeException(nameof(maxConcurrentExecutionsCount));

            _callback = callback;
            _maxConcurrentExecutionsCount = maxConcurrentExecutionsCount;
            _semaphore = new SemaphoreSlim(0, maxConcurrentExecutionsCount);
        }

        private async void Run()
        {
            if (!_semaphore.Wait(0))
            {
                return;
            }

            try
            {
                await this.OnCallbackAsync(_tokenSource!.Token);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async ValueTask OnCallbackAsync(CancellationToken token)
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

        public async void Change(TimeSpan dueTime, TimeSpan period)
        {
            using (await _asyncLock.LockAsync())
            {
                _dueTime = dueTime;
                _period = period;

                if (this.StateType != ServiceStateType.Running)
                {
                    return;
                }

                _timer!.Change(_dueTime, _period);
            }
        }

        public override ServiceStateType StateType => _stateType;

        protected override async ValueTask OnInitializeAsync()
        {
        }

        protected override async ValueTask OnStartAsync()
        {
            using (await _asyncLock.LockAsync())
            {
                _stateType = ServiceStateType.Starting;

                for (int count = (_maxConcurrentExecutionsCount - _semaphore.CurrentCount) - 1; count >= 0; count--)
                {
                    _semaphore.Release();
                }

                _tokenSource = new CancellationTokenSource();
                _timer = new Timer((_) => this.Run(), null, _dueTime, _period);

                _stateType = ServiceStateType.Running;
            }
        }

        protected override async ValueTask OnStopAsync()
        {
            using (await _asyncLock.LockAsync())
            {
                _stateType = ServiceStateType.Stopping;

                _timer!.Dispose();
                _tokenSource!.Cancel();

                for (int count = 0; count < _maxConcurrentExecutionsCount; count++)
                {
                    await _semaphore.WaitAsync();
                }

                _tokenSource!.Dispose();

                _timer = null;
                _tokenSource = null;

                _stateType = ServiceStateType.Stopped;
            }
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                this.StopAsync().AsTask().Wait();
            }
        }

        protected override async ValueTask OnDisposeAsync()
        {
            await this.StopAsync();
        }
    }
}
