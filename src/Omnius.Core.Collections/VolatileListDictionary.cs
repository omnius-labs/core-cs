using System.Collections;
using Omnius.Core.Tasks;

namespace Omnius.Core.Collections;

public partial class VolatileListDictionary<TKey, TValue> : AsyncDisposableBase, IEnumerable<KeyValuePair<TKey, IReadOnlyList<TValue>>>, IEnumerable
    where TKey : notnull
{
    private readonly Dictionary<TKey, List<Entry<TValue>>> _map;
    private readonly TimeSpan _survivalInterval;
    private readonly ISystemClock _systemClock;

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly PeriodicTimer _reaperTimer;
    private readonly Task _reaperTask;

    private readonly object _lockObject = new();

    public VolatileListDictionary(TimeSpan survivalInterval, TimeSpan reapingInterval, ISystemClock systemClock, IBatchActionDispatcher batchActionDispatcher)
        : this(survivalInterval, reapingInterval, EqualityComparer<TKey>.Default, systemClock, batchActionDispatcher)
    {
    }

    public VolatileListDictionary(TimeSpan survivalInterval, TimeSpan reapingInterval, IEqualityComparer<TKey> comparer, ISystemClock systemClock, IBatchActionDispatcher batchActionDispatcher)
    {
        _map = new Dictionary<TKey, List<Entry<TValue>>>(comparer);
        _survivalInterval = survivalInterval;
        _systemClock = systemClock;

        _cancellationTokenSource = new CancellationTokenSource();
        _reaperTimer = new PeriodicTimer(reapingInterval);
        _reaperTask = Task.Factory.StartNew(async () =>
        {
            try
            {
                while (await _reaperTimer.WaitForNextTickAsync(_cancellationTokenSource.Token))
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

            var removingKeys = new List<TKey>();

            foreach (var (key, entries) in _map)
            {
                entries.RemoveAll(n => (now - n.UpdatedTime) > _survivalInterval);

                if (entries.Count == 0)
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

    public KeyValuePair<TKey, IReadOnlyList<TValue>>[] ToArray()
    {
        lock (_lockObject)
        {
            var list = new List<KeyValuePair<TKey, IReadOnlyList<TValue>>>(_map.Count);

            foreach (var (key, entries) in _map)
            {
                list.Add(new KeyValuePair<TKey, IReadOnlyList<TValue>>(key, new ReadOnlyListSlim<TValue>(entries.Select(n => n.Value).ToArray())));
            }

            return list.ToArray();
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            lock (_lockObject)
            {
                return _map.Keys.ToArray();
            }
        }
    }

    public ICollection<IReadOnlyList<TValue>> Values
    {
        get
        {
            lock (_lockObject)
            {
                return _map.Values.Select(entries => new ReadOnlyListSlim<TValue>(entries.Select(n => n.Value).ToArray())).ToArray();
            }
        }
    }

    public IEqualityComparer<TKey> Comparer
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

    public void Add(TKey key, TValue value)
    {
        lock (_lockObject)
        {
            if (!_map.TryGetValue(key, out var list))
            {
                list = new List<Entry<TValue>>();
                _map.Add(key, list);
            }

            list.Add(new Entry<TValue>(value, _systemClock.GetUtcNow()));
        }
    }

    public void AddRange(TKey key, IEnumerable<TValue> collection)
    {
        lock (_lockObject)
        {
            if (!_map.TryGetValue(key, out var list))
            {
                list = new List<Entry<TValue>>();
                _map.Add(key, list);
            }

            foreach (var value in collection)
            {
                list.Add(new Entry<TValue>(value, _systemClock.GetUtcNow()));
            }
        }
    }

    public void Clear()
    {
        lock (_lockObject)
        {
            _map.Clear();
        }
    }

    public bool ContainsKey(TKey key)
    {
        lock (_lockObject)
        {
            return _map.ContainsKey(key);
        }
    }

    public bool Remove(TKey key)
    {
        lock (_lockObject)
        {
            return _map.Remove(key);
        }
    }

    public bool TryGetValue(TKey key, out IReadOnlyList<TValue> value)
    {
        lock (_lockObject)
        {
            if (_map.TryGetValue(key, out var entries))
            {
                value = new ReadOnlyListSlim<TValue>(entries.Select(m => m.Value).ToArray());
                return true;
            }
            else
            {
                value = default!;
                return false;
            }
        }
    }

    public IEnumerator<KeyValuePair<TKey, IReadOnlyList<TValue>>> GetEnumerator()
    {
        var items = new List<KeyValuePair<TKey, IReadOnlyList<TValue>>>();

        lock (_lockObject)
        {
            foreach (var (key, entries) in _map)
            {
                var value = new ReadOnlyListSlim<TValue>(entries.Select(m => m.Value).ToArray());
                items.Add(new KeyValuePair<TKey, IReadOnlyList<TValue>>(key, value));
            }
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

    internal readonly struct Entry<T>
    {
        public readonly T Value;
        public readonly DateTime UpdatedTime;

        public Entry(T value, DateTime updatedTime)
        {
            this.Value = value;
            this.UpdatedTime = updatedTime;
        }
    }
}
