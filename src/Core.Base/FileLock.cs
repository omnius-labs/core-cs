namespace Core.Base;

public sealed class FileLock
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly string _path;

    public FileLock(string path)
    {
        _path = path;
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
                _logger.Info(e, "FileLock waiting...");
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
                _logger.Info(e, "FileLock waiting...");
            }

            Thread.Sleep(1000);
        }
    }
}
