using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Tasks
{
    public sealed partial class BatchActionDispatcher : AsyncDisposableBase, IBatchActionDispatcher
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly TimeSpan _interval;

        private ImmutableHashSet<IBatchAction> _batchActions = ImmutableHashSet<IBatchAction>.Empty;

        private readonly Task _eventLoopTask;
        private readonly ManualResetEvent _resetEvent = new(false);
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private readonly object _lockObject = new();

        public BatchActionDispatcher(TimeSpan interval)
        {
            _interval = interval;
            _eventLoopTask = this.EventLoopAsync();
        }

        protected override async ValueTask OnDisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            await _eventLoopTask;
            _resetEvent.Dispose();
            _cancellationTokenSource.Dispose();
        }

        private async Task EventLoopAsync()
        {
            try
            {
                var batchActionMap = ImmutableDictionary<IBatchAction, DateTime>.Empty;

                for (; ; )
                {
                    await _resetEvent.WaitAsync(_cancellationTokenSource.Token);

                    var batchActions = _batchActions;

                    foreach (var batchAction in batchActions)
                    {
                        if (batchActionMap.ContainsKey(batchAction)) continue;
                        batchActionMap = batchActionMap.Add(batchAction, DateTime.MinValue);
                    }

                    foreach (var (batchAction, task) in batchActionMap)
                    {
                        if (batchActions.Contains(batchAction)) continue;
                        batchActionMap = batchActionMap.Remove(batchAction);
                    }

                    if (batchActionMap.Count == 0) continue;

                    var now = DateTime.UtcNow;

                    foreach (var (batchAction, lastExecutionTime) in batchActionMap)
                    {
                        if ((now - lastExecutionTime) < batchAction.Interval) continue;

                        batchActionMap = batchActionMap.SetItem(batchAction, now);
                        batchAction.Execute();
                    }

                    await Task.Delay(_interval, _cancellationTokenSource.Token);
                }
            }
            catch (TaskCanceledException e)
            {
                _logger.Debug(e);
            }
        }

        public void Register(IBatchAction batchAction)
        {
            lock (_lockObject)
            {
                _batchActions = _batchActions.Add(batchAction);
                _resetEvent.Set();
            }
        }

        public void Unregister(IBatchAction batchAction)
        {
            lock (_lockObject)
            {
                _batchActions = _batchActions.Remove(batchAction);
                if (_batchActions.Count == 0) _resetEvent.Reset();
            }
        }
    }
}
