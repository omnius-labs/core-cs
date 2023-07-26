namespace Omnius.Core;

public class SystemClock : ISystemClock
{
    public DateTime GetUtcNow()
    {
        return DateTime.UtcNow;
    }
}
