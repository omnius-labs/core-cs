namespace Omnius.Core;

public static class NLogLoggerExtentions
{
    public static void TryCatch<T>(this NLog.Logger logger, Action action)
        where T : Exception
    {
        try
        {
            action();
        }
        catch (T e)
        {
            logger.Warn(e);
        }
    }
}
