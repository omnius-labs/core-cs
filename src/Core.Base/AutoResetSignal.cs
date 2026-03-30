namespace Omnius.Core.Base;

public class AutoResetSignal
{
    private readonly SemaphoreSlim _semaphore;
    private readonly object _lockObject = new();

    public AutoResetSignal(bool initialState)
    {
        _semaphore = new SemaphoreSlim(initialState ? 1 : 0, 1);
    }

    public async ValueTask WaitAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    public void Set()
    {
        lock (_lockObject)
        {
            if (_semaphore.CurrentCount == 0)
            {
                _semaphore.Release();
            }
        }
    }

    public void Reset()
    {
        lock (_lockObject)
        {
            if (_semaphore.CurrentCount > 0)
            {
                _semaphore.Wait(0);
            }
        }
    }
}
