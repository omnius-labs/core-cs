namespace Omnius.Core.Helpers;

public static class ExceptionHelper
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    public static void TryCatch<TException>(Action callback)
        where TException : Exception
    {
        try
        {
            callback.Invoke();
        }
        catch (TException e)
        {
            _logger.Info(e);
        }
    }

    public static TResult? TryCatch<TException, TResult>(Func<TResult> callback)
        where TException : Exception
    {
        try
        {
            return callback.Invoke();
        }
        catch (TException e)
        {
            _logger.Info(e);
        }

        return default;
    }
}
