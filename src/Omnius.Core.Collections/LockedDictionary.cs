using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Omnius.Core.Collections
{
    public sealed partial class LockedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, ISynchronized
        where TKey : notnull
    {
        private readonly IDictionary<TKey, TValue> _dic;

        private LockedCollection<TKey>? _keys;
        private LockedCollection<TValue>? _values;

        public LockedDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _dic = dictionary;
        }

        public object LockObject { get; } = new object();

        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            lock (this.LockObject)
            {
                var array = new KeyValuePair<TKey, TValue>[_dic.Count];
                _dic.CopyTo(array, 0);

                return array;
            }
        }

        public LockedCollection<TKey> Keys
        {
            get
            {
                lock (this.LockObject)
                {
                    return _keys ?? (_keys = new LockedCollection<TKey>(_dic.Keys, this.LockObject));
                }
            }
        }

        public LockedCollection<TValue> Values
        {
            get
            {
                lock (this.LockObject)
                {
                    return _values ?? (_values = new LockedCollection<TValue>(_dic.Values, this.LockObject));
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
                    return _dic[key];
                }
            }
            set
            {
                lock (this.LockObject)
                {
                    this.Add(key, value);
                }
            }
        }

        public bool Add(TKey key, TValue value)
        {
            lock (this.LockObject)
            {
                int count = _dic.Count;
                _dic[key] = value;

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

        public bool Remove(TKey key)
        {
            lock (this.LockObject)
            {
                return _dic.Remove(key);
            }
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            lock (this.LockObject)
            {
                return _dic.TryGetValue(key, out value);
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

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

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
                return _dic.Contains(item);
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (this.LockObject)
            {
                _dic.CopyTo(array, arrayIndex);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            lock (this.LockObject)
            {
                return _dic.Remove(keyValuePair);
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (this.LockObject)
            {
                foreach (var item in _dic)
                {
                    yield return item;
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
    }
}
