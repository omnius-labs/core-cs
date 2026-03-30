namespace Omnius.Core.Base;

public class Clock : IClock
{
    public static readonly Clock Shared = new();

    public DateTime GetUtcNow()
    {
        return DateTime.UtcNow;
    }
}

public class FakeClock : IClock
{
    private DateTime _currentTime;
    private readonly object _lockObject = new();

    public FakeClock(DateTime start)
    {
        _currentTime = start;
    }

    public DateTime GetUtcNow()
    {
        lock (_lockObject)
        {
            return _currentTime;
        }
    }

    public void AdvanceTime(TimeSpan duration)
    {
        lock (_lockObject)
        {
            _currentTime = _currentTime.Add(duration);
        }
    }
}
