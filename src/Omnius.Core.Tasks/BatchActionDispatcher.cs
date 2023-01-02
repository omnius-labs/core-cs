using System.Collections.Immutable;

namespace Omnius.Core.Tasks;

public sealed partial class BatchActionDispatcher : AsyncDisposableBase, IBatchActionDispatcher
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly TimeSpan _interval;

    private ImmutableDictionary<IBatchAction, BatchActionState> _batchActions = ImmutableDictionary<IBatchAction, BatchActionState>.Empty;

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
            for (; ; )
            {
                await _resetEvent.WaitAsync(_cancellationTokenSource.Token).ConfigureAwait(false);

                var batchActions = _batchActions;
                if (batchActions.Count == 0) continue;

                var now = DateTime.UtcNow;

                foreach (var (batchAction, state) in batchActions)
                {
                    if ((now - state.LastExecutionTime) < batchAction.Interval) continue;

                    state.LastExecutionTime = now;
                    batchAction.Execute();
                }

                await Task.Delay(_interval, _cancellationTokenSource.Token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException e)
        {
            _logger.Debug(e, "Operation Canceled");
        }
    }

    public void Register(IBatchAction batchAction)
    {
        lock (_lockObject)
        {
            _batchActions = _batchActions.Add(batchAction, new BatchActionState() { LastExecutionTime = DateTime.MinValue });
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

    private record BatchActionState
    {
        public DateTime LastExecutionTime { get; set; }
    }
}
