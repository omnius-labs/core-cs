using System.Runtime.CompilerServices;
using System.Text;

namespace Core.Base;

public sealed class AsyncLockMonitor
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly NeoSmart.AsyncLock.AsyncLock _lock; // Recursiveなロックが可能

    private readonly Guid _guid = Guid.NewGuid();

    public AsyncLockMonitor()
    {
        _lock = new NeoSmart.AsyncLock.AsyncLock();
    }

    public async Task<IDisposable> LockAsync(
        CancellationToken cancellationToken = default,
        [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0)
    {
        var disposable = await _lock.LockAsync(cancellationToken);

        var sb = new StringBuilder();
        sb.AppendLine($"LockAsync UUID: {_guid}");
        sb.AppendLine($"LockAsync MemberName: {member}");
        sb.AppendLine($"LockAsync FilePath: {file}");
        sb.AppendLine($"LockAsync LineNumber: {line}");
        _logger.Trace(sb.ToString());

        return disposable;
    }
}
