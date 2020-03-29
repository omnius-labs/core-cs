using System;
using System.Collections;
using System.Collections.Generic;

namespace Omnius.Core.Collections
{
    public partial class VolatileDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IVolatile<TKey>
        where TKey : notnull
    {
        private readonly Dictionary<TKey, Info<TValue>> _dic;
        private readonly TimeSpan _survivalTime;

        private VolatileKeyCollection? _keys;
        private VolatileValueCollection? _values;

        public VolatileDictionary(TimeSpan survivalTime)
        {
            _dic = new Dictionary<TKey, Info<TValue>>();
            _survivalTime = survivalTime;
        }

        public VolatileDictionary(TimeSpan survivalTime, IEqualityComparer<TKey> comparer)
        {
            _dic = new Dictionary<TKey, Info<TValue>>(comparer);
            _survivalTime = survivalTime;
        }

        public TimeSpan SurvivalTime => _survivalTime;

        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            var list = new List<KeyValuePair<TKey, TValue>>(_dic.Count);

            foreach (var (key, info) in _dic)
            {
                list.Add(new KeyValuePair<TKey, TValue>(key, info.Value));
            }

            return list.ToArray();
        }

        public KeyValuePair<TKey, TValue>[] ToArray(TimeSpan span)
        {
            var now = DateTime.UtcNow;

            var list = new List<KeyValuePair<TKey, TValue>>(_dic.Count);

            foreach (var (key, info) in _dic)
            {
                if ((now - info.UpdateTime) < span)
                {
                    list.Add(new KeyValuePair<TKey, TValue>(key, info.Value));
                }
            }

            return list.ToArray();
        }

        public void Refresh()
        {
            var now = DateTime.UtcNow;

            var list = new List<TKey>();

            foreach (var (key, info) in _dic)
            {
                if ((now - info.UpdateTime) > _survivalTime)
                {
                    list.Add(key);
                }
            }

            foreach (var key in list)
            {
                _dic.Remove(key);
            }
        }

        public TimeSpan GetElapsedTime(TKey key)
        {
            if (!_dic.TryGetValue(key, out var info))
            {
                return _survivalTime;
            }

            var now = DateTime.UtcNow;
            return (now - info.UpdateTime);
        }


        public VolatileKeyCollection Keys
        {
            get
            {
                return _keys ?? (_keys = new VolatileKeyCollection(_dic.Keys));
            }
        }

        public VolatileValueCollection Values
        {
            get
            {
                return _values ?? (_values = new VolatileValueCollection(_dic.Values));
            }
        }

        public IEqualityComparer<TKey> Comparer
        {
            get
            {
                return _dic.Comparer;
            }
        }

        public int Count
        {
            get
            {
                return _dic.Count;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                var info = _dic[key];
                info.UpdateTime = DateTime.UtcNow;

                return info.Value;
            }
            set
            {
                _dic[key] = new Info<TValue>() { Value = value, UpdateTime = DateTime.UtcNow };
            }
        }

        public bool Add(TKey key, TValue value)
        {
            int count = _dic.Count;
            _dic[key] = new Info<TValue>() { Value = value, UpdateTime = DateTime.UtcNow };

            return (count != _dic.Count);
        }

        public void Clear()
        {
            _dic.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            return _dic.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return _dic.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_dic.TryGetValue(key, out var info))
            {
                info.UpdateTime = DateTime.UtcNow;
                value = info.Value;

                return true;
            }
            else
            {
                value = default!;

                return false;
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                return this.Keys;
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                return this.Values;
            }
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            this.Add(key, value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            var keyComparer = EqualityComparer<TKey>.Default;
            var valueComparer = EqualityComparer<TValue>.Default;

            foreach (var (key, info) in _dic)
            {
                if (keyComparer.Equals(item.Key, key)
                    && valueComparer.Equals(item.Value, info.Value))
                {
                    return true;
                }
            }

            return false;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var (key, info) in _dic)
            {
                array.SetValue(new KeyValuePair<TKey, TValue>(key, info.Value), arrayIndex++);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            var keyComparer = EqualityComparer<TKey>.Default;
            var valueComparer = EqualityComparer<TValue>.Default;

            bool flag = false;

            foreach (var (key, info) in _dic)
            {
                if (keyComparer.Equals(keyValuePair.Key, key)
                    && valueComparer.Equals(keyValuePair.Value, info.Value))
                {
                    _dic.Remove(key);

                    flag = true;
                }
            }

            return flag;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var (key, info) in _dic)
            {
                yield return new KeyValuePair<TKey, TValue>(key, info.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal struct Info<T>
        {
            public T Value;
            public DateTime UpdateTime;
        }
    }
}
