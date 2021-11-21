using System.Runtime.CompilerServices;
using NLog.Fluent;

namespace Omnius.Core;

public sealed class AsyncLockMonitor
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private readonly Guid _guid = Guid.NewGuid();

    public AsyncLockMonitor()
    {
    }

    public async Task<IDisposable> LockAsync(
        CancellationToken cancellationToken = default,
        [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0)
    {
        await _semaphore.WaitAsync();

        _logger.Info()
            .Message($"---- Lock Start: {_guid} ----")
            .Property("MemberName", member)
            .Property("FilePath", file)
            .Property("LineNumber", line)
            .Write();

        return new Releaser(this, _guid);
    }

    private sealed class Releaser : IDisposable
    {
        private readonly AsyncLockMonitor _toRelease;

        private readonly Guid _guid;

        internal Releaser(AsyncLockMonitor toRelease, Guid guid)
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
