using System.Collections;
using Core.Base;

namespace Core.Collections;

public class VolatileHashSet<T> : AsyncDisposableBase, ISet<T>, ICollection<T>, IEnumerable<T>
    where T : notnull
{
    private readonly Dictionary<T, DateTime> _map;
    private readonly TimeSpan _survivalInterval;
    private readonly ISystemClock _systemClock;

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly PeriodicTimer _reaperTimer;
    private readonly Task _reaperTask;

    private readonly object _lockObject = new();

    public VolatileHashSet(TimeSpan survivalInterval, TimeSpan reapingInterval, ISystemClock systemClock)
        : this(survivalInterval, reapingInterval, EqualityComparer<T>.Default, systemClock)
    {
    }

    public VolatileHashSet(TimeSpan survivalInterval, TimeSpan reapingInterval, IEqualityComparer<T> comparer, ISystemClock systemClock)
    {
        _map = new Dictionary<T, DateTime>(comparer);
        _survivalInterval = survivalInterval;
        _systemClock = systemClock;

        _cancellationTokenSource = new CancellationTokenSource();
        _reaperTimer = new PeriodicTimer(reapingInterval);
        _reaperTask = Task.Factory.StartNew(async () =>
        {
            try
            {
                while (await _reaperTimer.WaitForNextTickAsync(_cancellationTokenSource.Token).ConfigureAwait(false))
                {
                    this.Refresh();
                }
            }
            catch (OperationCanceledException)
            {
            }
        }, TaskCreationOptions.LongRunning);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        _cancellationTokenSource.Cancel();
        await _reaperTask;
        _cancellationTokenSource.Dispose();
    }

    private void Refresh()
    {
        lock (_lockObject)
        {
            var now = _systemClock.GetUtcNow();

            var removingKeys = new List<T>();

            foreach (var (key, value) in _map)
            {
                if ((now - value) > _survivalInterval)
                {
                    removingKeys.Add(key);
                }
            }

            if (removingKeys.Count == 0) return;

            foreach (var key in removingKeys)
            {
                _map.Remove(key);
            }

            _map.TrimExcess();
        }
    }

    public TimeSpan SurvivalTime => _survivalInterval;

    public T[] ToArray()
    {
        lock (_lockObject)
        {
            return _map.Keys.ToArray();
        }
    }

    public TimeSpan GetElapsedTime(T item)
    {
        lock (_lockObject)
        {
            if (!_map.TryGetValue(item, out var updatedTime)) return _survivalInterval;

            var now = _systemClock.GetUtcNow();
            return (now - updatedTime);
        }
    }

    public IEqualityComparer<T> Comparer
    {
        get
        {
            lock (_lockObject)
            {
                return _map.Comparer;
            }
        }
    }

    public int Count
    {
        get
        {
            lock (_lockObject)
            {
                return _map.Count;
            }
        }
    }

    public bool Add(T item)
    {
        lock (_lockObject)
        {
            int count = _map.Count;
            _map[item] = _systemClock.GetUtcNow();

            return (count != _map.Count);
        }
    }

    public void Clear()
    {
        lock (_lockObject)
        {
            _map.Clear();
        }
    }

    public bool Contains(T item)
    {
        lock (_lockObject)
        {
            return _map.ContainsKey(item);
        }
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        lock (_lockObject)
        {
            _map.Keys.CopyTo(array, arrayIndex);
        }
    }

    public bool Remove(T item)
    {
        lock (_lockObject)
        {
            return _map.Remove(item);
        }
    }

    bool ICollection<T>.IsReadOnly => false;

    void ICollection<T>.Add(T item)
    {
        this.Add(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        var items = new List<T>();

        lock (_lockObject)
        {
            items.AddRange(_map.Keys);
        }

        foreach (var item in items)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public void UnionWith(IEnumerable<T> other)
    {
        lock (_lockObject)
        {
            var now = _systemClock.GetUtcNow();

            foreach (var value in other)
            {
                _map[value] = now;
            }
        }
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        lock (_lockObject)
        {
            foreach (var value in other)
            {
                _map.Remove(value);
            }
        }
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        lock (_lockObject)
        {
            var tempList = new List<T>();

            foreach (var value in other)
            {
                if (_map.ContainsKey(value)) continue;

                tempList.Add(value);
            }

            foreach (var key in tempList)
            {
                _map.Remove(key);
            }
        }
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        throw new NotImplementedException();
    }

    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        throw new NotImplementedException();
    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {
        throw new NotImplementedException();
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
        throw new NotImplementedException();
    }

    public bool Overlaps(IEnumerable<T> other)
    {
        throw new NotImplementedException();
    }

    public bool SetEquals(IEnumerable<T> other)
    {
        throw new NotImplementedException();
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        throw new NotImplementedException();
    }
}
