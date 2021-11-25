namespace Omnius.Core;

public sealed class AsyncLock
{
    private readonly NeoSmart.AsyncLock.AsyncLock _lock; // Recursiveなロックが可能

    public AsyncLock()
    {
        _lock = new NeoSmart.AsyncLock.AsyncLock();
    }

    public Task<IDisposable> LockAsync(CancellationToken cancellationToken = default)
    {
        return _lock.LockAsync(cancellationToken);
    }

    public IDisposable Lock(CancellationToken cancellationToken = default)
    {
        return _lock.Lock();
    }
}
