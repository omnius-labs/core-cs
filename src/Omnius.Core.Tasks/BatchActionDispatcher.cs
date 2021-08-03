using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Tasks
{
    public sealed partial class BatchActionDispatcher : AsyncDisposableBase, IBatchActionDispatcher
    {
        private ImmutableList<IBatchAction> _batchActions = ImmutableList<IBatchAction>.Empty;

        private readonly Task _eventLoopTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public BatchActionDispatcher()
        {
            _eventLoopTask = this.EventLoopAsync();
        }

        protected override async ValueTask OnDisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            await _eventLoopTask;
            _cancellationTokenSource.Dispose();
        }

        private async Task EventLoopAsync()
        {
            try
            {
                for (; ; )
                {
                    var batchActions = _batchActions;

                    if (batchActions.Count == 0)
                    {
                        await Task.Delay(500, _cancellationTokenSource.Token);
                        continue;
                    }

                    var map = new Dictionary<IBatchAction, Task>();
                    foreach (var batchAction in batchActions)
                    {
                        map.Add(batchAction, batchAction.WaitAsync(_cancellationTokenSource.Token).AsTask());
                    }

                    await Task.WhenAny(map.Values);

                    foreach (var (batchAction, task) in map)
                    {
                        if (task.IsCompletedSuccessfully)
                        {
                            batchAction.Run();
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // ignore
                return;
            }
        }

        public void Register(IBatchAction batchAction)
        {
            _batchActions = _batchActions.Add(batchAction);
        }

        public void Unregister(IBatchAction batchAction)
        {
            _batchActions = _batchActions.Remove(batchAction);
        }
    }
}
