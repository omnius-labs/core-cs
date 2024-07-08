using System.Diagnostics;

namespace Omnius.Core.Base;

public static class StopwatchExtensions
{
    public static bool TryRestartIfElapsed(this Stopwatch stopwatch, TimeSpan interval)
    {
        if (stopwatch.Elapsed < interval) return false;
        stopwatch.Restart();
        return true;
    }

    public static bool TryRestartIfElapsedOrStopped(this Stopwatch stopwatch, TimeSpan interval)
    {
        if (stopwatch.IsRunning && stopwatch.Elapsed < interval) return false;
        stopwatch.Restart();
        return true;
    }
}
