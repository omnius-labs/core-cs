namespace Omnius.Core.Base;

// https://qiita.com/s-matsuba/items/1ee6bcc1bc5d721fc978
public sealed class EqualityCustomComparer<T> : IEqualityComparer<T>
{
    private readonly Func<T?, T?, bool> _predicate;
    private readonly Func<T, int> _getHashCode;

    public EqualityCustomComparer(Func<T?, T?, bool> predicate)
        : this(predicate, obj => obj?.GetHashCode() ?? 0)
    {
    }

    public EqualityCustomComparer(Func<T?, T?, bool> predicate, Func<T, int> getHashCode)
    {
        _predicate = predicate;
        _getHashCode = getHashCode;
    }

    public bool Equals(T? x, T? y)
    {
        return _predicate(x, y);
    }

    public int GetHashCode(T obj)
    {
        return _getHashCode(obj);
    }
}
