using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Omnius.Core.Base;

public sealed class AsyncLockMonitor
{
    private readonly ILogger _logger;
    private readonly NeoSmart.AsyncLock.AsyncLock _lock; // Recursiveなロックが可能
    private readonly Guid _guid = Guid.NewGuid();

    public AsyncLockMonitor(ILogger<AsyncLockMonitor> logger)
    {
        _logger = logger;
        _lock = new NeoSmart.AsyncLock.AsyncLock();
    }

    public async Task<IDisposable> LockAsync(
        CancellationToken cancellationToken = default,
        [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0)
    {
        var disposable = await _lock.LockAsync(cancellationToken);

        var sb = new StringBuilder();
        sb.AppendLine(CultureInfo.InvariantCulture, $"LockAsync UUID: {_guid}");
        sb.AppendLine(CultureInfo.InvariantCulture, $"LockAsync MemberName: {member}");
        sb.AppendLine(CultureInfo.InvariantCulture, $"LockAsync FilePath: {file}");
        sb.AppendLine(CultureInfo.InvariantCulture, $"LockAsync LineNumber: {line}");
        _logger.LogTrace(sb.ToString());

        return disposable;
    }
}
