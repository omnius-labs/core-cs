namespace Omnius.Core.Base;

public sealed class Arc
{
    public static Arc<T> Create<T>(T value, Action<T> cleanup) where T : class
    {
        return new Arc<T>(value, cleanup);
    }
}

public sealed class Arc<T> : IDisposable where T : class
{
    private sealed class Inner
    {
        public T Value;
        public Action<T> Cleanup;
        public int RefCount;

        public Inner(T value, Action<T> cleanup)
        {
            Value = value;
            Cleanup = cleanup;
            RefCount = 1;
        }
    }

    private Inner? _inner;

    public Arc(T value, Action<T> cleanup)
    {
        _inner = new Inner(value, cleanup);
    }

    private Arc(Inner inner)
    {
        _inner = inner;
        Interlocked.Increment(ref inner.RefCount);
    }

    public Arc<T> Clone()
    {
        var inner = _inner ?? throw new ObjectDisposedException(nameof(Arc<T>));
        return new Arc<T>(inner);
    }

    public T Value => (_inner ?? throw new ObjectDisposedException(nameof(Arc<T>))).Value;

    public void Dispose()
    {
        var inner = Interlocked.Exchange(ref _inner, null);
        if (inner is null) return;

        if (Interlocked.Decrement(ref inner.RefCount) == 0)
        {
            inner.Cleanup(inner.Value);
        }
    }
}
