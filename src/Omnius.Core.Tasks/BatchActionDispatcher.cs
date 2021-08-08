using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Tasks
{
    public sealed partial class BatchActionDispatcher : AsyncDisposableBase, IBatchActionDispatcher
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private ImmutableHashSet<IBatchAction> _batchActions = ImmutableHashSet<IBatchAction>.Empty;

        private readonly Task _eventLoopTask;
        private readonly ManualResetEvent _resetEvent = new(false);
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private readonly object _lockObject = new();

        public BatchActionDispatcher()
        {
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
                var tasks = ImmutableDictionary<IBatchAction, Task?>.Empty;

                for (; ; )
                {
                    await _resetEvent.WaitAsync(_cancellationTokenSource.Token);

                    var batchActions = _batchActions;

                    foreach (var batchAction in batchActions)
                    {
                        if (tasks.ContainsKey(batchAction)) continue;
                        tasks = tasks.Add(batchAction, null);
                    }

                    foreach (var (batchAction, task) in tasks)
                    {
                        if (batchActions.Contains(batchAction)) continue;
                        tasks = tasks.Remove(batchAction);
                    }

                    if (tasks.Count == 0) continue;

                    foreach (var (batchAction, task) in tasks)
                    {
                        if (task is not null) continue;
                        tasks = tasks.SetItem(batchAction, batchAction.WaitAsync(_cancellationTokenSource.Token).AsTask());
                    }

                    await Task.WhenAny(tasks.Values!);

                    foreach (var (batchAction, task) in tasks)
                    {
                        if (task!.IsCompleted)
                        {
                            tasks = tasks.Remove(batchAction);

                            if (task!.IsCompletedSuccessfully)
                            {
                                batchAction.Run();
                            }
                        }
                    }
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
