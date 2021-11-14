namespace Omnius.Core;

public static class TimerExtensions
{
    public static void Change(this Timer timer, TimeSpan period)
    {
        timer.Change(period, period);
    }

    public static void Change(this Timer timer, int period)
    {
        timer.Change(period, period);
    }
}
