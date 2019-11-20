using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Omnius.Core;

namespace Omnius.Core.Collections
{
    public sealed partial class LockedHashDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, ISynchronized
        where TKey : notnull
    {
        private readonly Dictionary<TKey, TValue> _dic;
        private int? _capacity;

        private LockedCollection<TKey>? _keys;
        private LockedCollection<TValue>? _values;

        public LockedHashDictionary()
        {
            _dic = new Dictionary<TKey, TValue>();
        }

        public LockedHashDictionary(int capacity)
        {
            _dic = new Dictionary<TKey, TValue>();
            _capacity = capacity;
        }

        public LockedHashDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _dic = new Dictionary<TKey, TValue>();

            foreach (var item in dictionary)
            {
                this.Add(item.Key, item.Value);
            }
        }

        public LockedHashDictionary(IEqualityComparer<TKey> comparer)
        {
            _dic = new Dictionary<TKey, TValue>(comparer);
        }

        public LockedHashDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            _dic = new Dictionary<TKey, TValue>(comparer);
            _capacity = capacity;
        }

        public LockedHashDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            _dic = new Dictionary<TKey, TValue>(comparer);

            foreach (var item in dictionary)
            {
                this.Add(item.Key, item.Value);
            }
        }

        public object LockObject { get; } = new object();

        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            lock (this.LockObject)
            {
                var array = new KeyValuePair<TKey, TValue>[_dic.Count];
                ((IDictionary<TKey, TValue>)_dic).CopyTo(array, 0);

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

        public int? Capacity
        {
            get
            {
                lock (this.LockObject)
                {
                    return _capacity;
                }
            }
            set
            {
                lock (this.LockObject)
                {
                    _capacity = value;
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
                if (_capacity != null && _dic.Count + 1 > _capacity.Value)
                {
                    throw new OverflowException();
                }

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

        public bool ContainsValue(TValue value)
        {
            lock (this.LockObject)
            {
                return _dic.ContainsValue(value);
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
                ((IDictionary<TKey, TValue>)_dic).CopyTo(array, arrayIndex);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            lock (this.LockObject)
            {
                return ((IDictionary<TKey, TValue>)_dic).Remove(keyValuePair);
            }
        }

        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot => this.LockObject;

        void ICollection.CopyTo(Array array, int index)
        {
            lock (this.LockObject)
            {
                ((ICollection)_dic).CopyTo(array, index);
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
