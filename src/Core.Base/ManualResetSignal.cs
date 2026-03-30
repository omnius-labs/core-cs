namespace Omnius.Core.Base;

public class ManualResetSignal
{
    private TaskCompletionSource _tcs = NewTcs();
    private bool _state;
    private readonly object _lockObject = new();

    public ManualResetSignal(bool initialState)
    {
        _state = initialState;
    }

    public async ValueTask WaitAsync(CancellationToken cancellationToken = default)
    {
        Task task;

        lock (_lockObject)
        {
            if (_state) return;
            task = _tcs.Task;
        }

        await task.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    public void Set()
    {
        lock (_lockObject)
        {
            if (_state) return;
            _state = true;
            _tcs.TrySetResult();
        }
    }

    public void Reset()
    {
        lock (_lockObject)
        {
            if (!_state) return;
            _state = false;
            _tcs = NewTcs();
        }
    }

    private static TaskCompletionSource NewTcs()
    {
        return new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
