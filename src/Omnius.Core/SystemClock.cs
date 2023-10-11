namespace Omnius.Core;

public class SystemClock : ISystemClock
{
    public static readonly SystemClock Shared = new();

    public DateTime GetUtcNow()
    {
        return DateTime.UtcNow;
    }
}
