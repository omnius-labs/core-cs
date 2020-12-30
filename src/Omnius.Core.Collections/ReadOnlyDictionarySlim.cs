using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Omnius.Core.Collections
{
    public sealed class ReadOnlyDictionarySlim<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
        where TKey : notnull
    {
        public static ReadOnlyDictionarySlim<TKey, TValue> Empty { get; }

        static ReadOnlyDictionarySlim()
        {
            Empty = new ReadOnlyDictionarySlim<TKey, TValue>(new Dictionary<TKey, TValue>());
        }

        private readonly Dictionary<TKey, TValue> _map;

        public ReadOnlyDictionarySlim(Dictionary<TKey, TValue> map)
        {
            _map = map;
        }

        public TValue this[TKey key] => _map[key];

        public IEnumerable<TKey> Keys => _map.Keys;

        public IEnumerable<TValue> Values => _map.Values;

        public int Count => _map.Count;

        public bool ContainsKey(TKey key)
        {
            return _map.ContainsKey(key);
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return _map.GetEnumerator();
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            return _map.TryGetValue(key, out value);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _map.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _map.GetEnumerator();
        }
    }
}
