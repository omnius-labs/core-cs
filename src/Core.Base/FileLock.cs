using Microsoft.Extensions.Logging;

namespace Omnius.Core.Base;

public sealed class FileLock
{
    private readonly string _path;
    private readonly ILogger _logger;

    public FileLock(string path, ILogger<FileLock> logger)
    {
        _path = path;
        _logger = logger;
    }

    public async Task<IDisposable> LockAsync(CancellationToken cancellationToken = default)
    {
        for (; ; )
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                return new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 1, FileOptions.DeleteOnClose);
            }
            catch (Exception e)
            {
                _logger.LogTrace(e, "FileLock waiting...");
            }

            await Task.Delay(1000).ConfigureAwait(false);
        }
    }

    public IDisposable Lock(CancellationToken cancellationToken = default)
    {
        for (; ; )
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                return new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 1, FileOptions.DeleteOnClose);
            }
            catch (Exception e)
            {
                _logger.LogTrace(e, "FileLock waiting...");
            }

            Thread.Sleep(1000);
        }
    }
}
