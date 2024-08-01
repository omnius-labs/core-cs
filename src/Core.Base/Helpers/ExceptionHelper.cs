using Microsoft.Extensions.Logging;

namespace Omnius.Core.Base.Helpers;

public static class ExceptionHelper
{
    public static void ExecuteAndLogIfFailed<TException>(Action callback, ILogger logger)
        where TException : Exception
    {
        try
        {
            callback.Invoke();
        }
        catch (TException e)
        {
            logger.LogTrace(e, "Caught");
        }
    }

    public static TResult? ExecuteAndLogIfFailed<TException, TResult>(Func<TResult> callback, ILogger logger)
        where TException : Exception
    {
        try
        {
            return callback.Invoke();
        }
        catch (TException e)
        {
            logger.LogTrace(e, "Caught");
        }

        return default;
    }
}
