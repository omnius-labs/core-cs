using NLog.Fluent;

namespace Omnius.Core;

public sealed class DeadLockMonitor
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private readonly Guid _guid = Guid.NewGuid();

    public DeadLockMonitor()
    {
    }

    public async Task<IDisposable> LockAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync();

        _logger.Debug()
            .Message($"---- Lock Start: {_guid} ----")
            .Property("StackTrace", Environment.StackTrace)
            .Write();

        return new Releaser(this, _guid);
    }

    private sealed class Releaser : IDisposable
    {
        private readonly DeadLockMonitor _toRelease;

        private readonly Guid _guid;

        internal Releaser(DeadLockMonitor toRelease, Guid guid)
        {
            _toRelease = toRelease;
            _guid = guid;
        }

        public void Dispose()
        {
            _logger.Info($"---- Lock End: {_guid} ----");

            _toRelease._semaphore.Release();
        }
    }
}