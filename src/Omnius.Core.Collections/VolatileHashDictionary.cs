using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Omnius.Core;

namespace Omnius.Core.Collections
{
    public partial class VolatileHashDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable, IVolatileCollection<TKey>, ISynchronized
        where TKey : notnull
    {
        private readonly Dictionary<TKey, Info<TValue>> _dic;
        private readonly TimeSpan _survivalTime;

        public VolatileHashDictionary(TimeSpan survivalTime)
        {
            _dic = new Dictionary<TKey, Info<TValue>>();
            _survivalTime = survivalTime;
        }

        public VolatileHashDictionary(TimeSpan survivalTime, IEqualityComparer<TKey> comparer)
        {
            _dic = new Dictionary<TKey, Info<TValue>>(comparer);
            _survivalTime = survivalTime;
        }

        public object LockObject { get; } = new object();

        public TimeSpan SurvivalTime => _survivalTime;

        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            lock (this.LockObject)
            {
                var list = new List<KeyValuePair<TKey, TValue>>(_dic.Count);

                foreach (var (key, info) in _dic)
                {
                    list.Add(new KeyValuePair<TKey, TValue>(key, info.Value));
                }

                return list.ToArray();
            }
        }

        public KeyValuePair<TKey, TValue>[] ToArray(TimeSpan span)
        {
            var now = DateTime.UtcNow;

            lock (this.LockObject)
            {
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
        }

        public void Refresh()
        {
            var now = DateTime.UtcNow;

            lock (this.LockObject)
            {
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
                lock (this.LockObject)
                {
                    return new VolatileKeyCollection(_dic.Keys, this.LockObject);
                }
            }
        }

        public VolatileValueCollection Values
        {
            get
            {
                lock (this.LockObject)
                {
                    return new VolatileValueCollection(_dic.Values, this.LockObject);
                }
            }
        }

        public IEqualityComparer<TKey> Comparer
        {
            get
            {
                lock (this.LockObject)
                {
                    return _dic.Comparer;
                }
            }
        }

        public int Count
        {
            get
            {
                lock (this.LockObject)
                {
                    return _dic.Count;
                }
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (this.LockObject)
                {
                    var info = _dic[key];
                    info.UpdateTime = DateTime.UtcNow;

                    return info.Value;
                }
            }
            set
            {
                lock (this.LockObject)
                {
                    _dic[key] = new Info<TValue>() { Value = value, UpdateTime = DateTime.UtcNow };
                }
            }
        }

        public bool Add(TKey key, TValue value)
        {
            lock (this.LockObject)
            {
                int count = _dic.Count;
                _dic[key] = new Info<TValue>() { Value = value, UpdateTime = DateTime.UtcNow };

                return (count != _dic.Count);
            }
        }

        public void Clear()
        {
            lock (this.LockObject)
            {
                _dic.Clear();
            }
        }

        public bool ContainsKey(TKey key)
        {
            lock (this.LockObject)
            {
                return _dic.ContainsKey(key);
            }
        }

        public bool ContainsValue(TValue value)
        {
            lock (this.LockObject)
            {
                return _dic.Values.Select(n => n.Value).Contains(value);
            }
        }

        public bool Remove(TKey key)
        {
            lock (this.LockObject)
            {
                return _dic.Remove(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (this.LockObject)
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
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                lock (this.LockObject)
                {
                    return this.Keys;
                }
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                lock (this.LockObject)
                {
                    return this.Values;
                }
            }
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            lock (this.LockObject)
            {
                this.Add(key, value);
            }
        }

        bool IDictionary.IsFixedSize
        {
            get
            {
                lock (this.LockObject)
                {
                    return false;
                }
            }
        }

        bool IDictionary.IsReadOnly
        {
            get
            {
                lock (this.LockObject)
                {
                    return false;
                }
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                lock (this.LockObject)
                {
                    return this.Keys;
                }
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                lock (this.LockObject)
                {
                    return this.Values;
                }
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                lock (this.LockObject)
                {
                    return this[(TKey)key]!;
                }
            }
            set
            {
                lock (this.LockObject)
                {
                    this[(TKey)key] = (TValue)value;
                }
            }
        }

        void IDictionary.Add(object key, object? value)
        {
            if (value is null)
            {
                throw new NullReferenceException(nameof(value));
            }

            lock (this.LockObject)
            {
                this.Add((TKey)key, (TValue)value);
            }
        }

        bool IDictionary.Contains(object key)
        {
            lock (this.LockObject)
            {
                return this.ContainsKey((TKey)key);
            }
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            lock (this.LockObject)
            {
                return _dic.GetEnumerator();
            }
        }

        void IDictionary.Remove(object key)
        {
            lock (this.LockObject)
            {
                this.Remove((TKey)key);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                lock (this.LockObject)
                {
                    return false;
                }
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            lock (this.LockObject)
            {
                this.Add(item.Key, item.Value);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (this.LockObject)
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
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (this.LockObject)
            {
                foreach (var (key, info) in _dic)
                {
                    array.SetValue(new KeyValuePair<TKey, TValue>(key, info.Value), arrayIndex++);
                }
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            lock (this.LockObject)
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
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                lock (this.LockObject)
                {
                    return true;
                }
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this.LockObject;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            lock (this.LockObject)
            {
                foreach (var (key, info) in _dic)
                {
                    array.SetValue(new KeyValuePair<TKey, TValue>(key, info.Value), index++);
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (this.LockObject)
            {
                foreach (var (key, info) in _dic)
                {
                    yield return new KeyValuePair<TKey, TValue>(key, info.Value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (this.LockObject)
            {
                return this.GetEnumerator();
            }
        }

        internal struct Info<T>
        {
            public T Value { get; set; }
            public DateTime UpdateTime { get; set; }
        }
    }
}
