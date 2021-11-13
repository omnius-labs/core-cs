using System.Collections;
using Omnius.Core.Tasks;

namespace Omnius.Core.Collections;

public partial class VolatileListDictionary<TKey, TValue> : DisposableBase, IEnumerable<KeyValuePair<TKey, IReadOnlyList<TValue>>>, IEnumerable
    where TKey : notnull
{
    private readonly Dictionary<TKey, List<Entry<TValue>>> _map;
    private readonly TimeSpan _survivalTime;
    private readonly IBatchActionDispatcher _batchActionDispatcher;
    private readonly IBatchAction _batchAction;

    private KeyCollection? _keys;
    private ValueCollection? _values;

    public VolatileListDictionary(TimeSpan survivalTime, IBatchActionDispatcher batchActionDispatcher)
        : this(survivalTime, EqualityComparer<TKey>.Default, batchActionDispatcher)
    {
    }

    public VolatileListDictionary(TimeSpan survivalTime, IEqualityComparer<TKey> comparer, IBatchActionDispatcher batchActionDispatcher)
    {
        _map = new Dictionary<TKey, List<Entry<TValue>>>(comparer);
        _survivalTime = survivalTime;
        _batchActionDispatcher = batchActionDispatcher;
        _batchAction = new BatchAction(this);
        _batchActionDispatcher.Register(_batchAction);
    }

    protected override void OnDispose(bool disposing)
    {
        _batchActionDispatcher.Unregister(_batchAction);
    }

    private sealed class BatchAction : IBatchAction
    {
        private readonly VolatileListDictionary<TKey, TValue> _volatileListDictionary;

        public BatchAction(VolatileListDictionary<TKey, TValue> volatileListDictionary)
        {
            _volatileListDictionary = volatileListDictionary;
        }

        public TimeSpan Interval => TimeSpan.FromSeconds(3);

        public void Execute()
        {
            _volatileListDictionary.Refresh();
        }
    }

    private void Refresh()
    {
        var now = DateTime.UtcNow;

        var removingKeys = new List<TKey>();

        foreach (var (key, entries) in _map)
        {
            entries.RemoveAll(n => (now - n.UpdateTime) > _survivalTime);

            if (entries.Count == 0)
            {
                removingKeys.Add(key);
            }
        }

        foreach (var key in removingKeys)
        {
            _map.Remove(key);
        }

        _map.TrimExcess();
    }

    public TimeSpan SurvivalTime => _survivalTime;

    public KeyValuePair<TKey, IReadOnlyList<TValue>>[] ToArray()
    {
        var list = new List<KeyValuePair<TKey, IReadOnlyList<TValue>>>(_map.Count);

        foreach (var (key, entries) in _map)
        {
            list.Add(new KeyValuePair<TKey, IReadOnlyList<TValue>>(key, new ReadOnlyListSlim<TValue>(entries.Select(n => n.Value).ToArray())));
        }

        return list.ToArray();
    }

    public KeyCollection Keys
    {
        get
        {
            return _keys ?? (_keys = new KeyCollection(_map.Keys));
        }
    }

    public ValueCollection Values
    {
        get
        {
            return _values ?? (_values = new ValueCollection(_map.Values));
        }
    }

    public IEqualityComparer<TKey> Comparer
    {
        get
        {
            return _map.Comparer;
        }
    }

    public int Count
    {
        get
        {
            return _map.Count;
        }
    }

    public void Add(TKey key, TValue value)
    {
        if (!_map.TryGetValue(key, out var list))
        {
            list = new List<Entry<TValue>>();
            _map.Add(key, list);
        }

        list.Add(new Entry<TValue>(value, DateTime.UtcNow));
    }

    public void AddRange(TKey key, IEnumerable<TValue> collection)
    {
        if (!_map.TryGetValue(key, out var list))
        {
            list = new List<Entry<TValue>>();
            _map.Add(key, list);
        }

        foreach (var value in collection)
        {
            list.Add(new Entry<TValue>(value, DateTime.UtcNow));
        }
    }

    public void Clear()
    {
        _map.Clear();
    }

    public bool ContainsKey(TKey key)
    {
        return _map.ContainsKey(key);
    }

    public bool Remove(TKey key)
    {
        return _map.Remove(key);
    }

    public bool TryGetValue(TKey key, out IReadOnlyList<TValue> value)
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

    public IEnumerator<KeyValuePair<TKey, IReadOnlyList<TValue>>> GetEnumerator()
    {
        foreach (var (key, entries) in _map)
        {
            var value = new ReadOnlyListSlim<TValue>(entries.Select(m => m.Value).ToArray());
            yield return new KeyValuePair<TKey, IReadOnlyList<TValue>>(key, value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    internal readonly struct Entry<T>
    {
        public readonly T Value;
        public readonly DateTime UpdateTime;

        public Entry(T value, DateTime updateTime)
        {
            this.Value = value;
            this.UpdateTime = updateTime;
        }
    }
}