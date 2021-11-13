namespace Omnius.Core.Net.Connections;

public sealed class BandwidthLimiter : IBandwidthLimiter
{
    private readonly Queue<(DateTime time, int size)> _queue = new();
    private readonly int _maxBytesPerSecond;

    public BandwidthLimiter(int maxBytesPerSecond)
    {
        _maxBytesPerSecond = maxBytesPerSecond;
    }

    public object LockObject { get; } = new object();

    public int ComputeFreeBytes()
    {
        lock (this.LockObject)
        {
            var now = DateTime.UtcNow;
            var lowerLimit = now.AddSeconds(-1);

            while (_queue.Count > 0 && _queue.Peek().time < lowerLimit)
            {
                _queue.Dequeue();
            }

            int result = _maxBytesPerSecond - _queue.Sum(n => n.size);

            return result;
        }
    }

    public void AddConsumedBytes(int size)
    {
        if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));

        lock (this.LockObject)
        {
            var now = DateTime.UtcNow;
            _queue.Enqueue((now, size));
        }
    }
}