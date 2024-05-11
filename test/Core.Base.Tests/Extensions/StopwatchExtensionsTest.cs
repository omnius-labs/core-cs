using System.Diagnostics;
using Xunit;

namespace Core.Base;

public class StopwatchExtensionsTest
{
    [Fact]
    public void TryRestartIfElapsedTest()
    {
        var stopwatch = new Stopwatch();

        Assert.False(stopwatch.IsRunning);
        Assert.False(stopwatch.TryRestartIfElapsed(TimeSpan.FromSeconds(10)));
        Assert.False(stopwatch.IsRunning);
        Assert.True(stopwatch.TryRestartIfElapsed(TimeSpan.FromSeconds(0)));
        Assert.True(stopwatch.IsRunning);
    }

    [Fact]
    public void TryRestartIfElapsedOrStopped()
    {
        var stopwatch = new Stopwatch();

        Assert.False(stopwatch.IsRunning);
        Assert.True(stopwatch.TryRestartIfElapsedOrStopped(TimeSpan.FromSeconds(10)));
        Assert.True(stopwatch.IsRunning);
        Assert.True(stopwatch.TryRestartIfElapsedOrStopped(TimeSpan.FromSeconds(0)));
        Assert.True(stopwatch.IsRunning);
    }
}
