using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Omnix.Base;

namespace Omnix.Collections
{
    public class LockedHashDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, ISynchronized
    {
        private Dictionary<TKey, TValue> _dic;
        private int? _capacity;

        private readonly object _lockObject = new object();

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

        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            lock (this.LockObject)
            {
                var array = new KeyValuePair<TKey, TValue>[_dic.Count];
                ((IDictionary<TKey, TValue>)_dic).CopyTo(array, 0);

                return array;
            }
        }

        public LockedKeyCollection Keys
        {
            get
            {
                lock (this.LockObject)
                {
                    return new LockedKeyCollection(_dic.Keys, this.LockObject);
                }
            }
        }

        public LockedValueCollection Values
        {
            get
            {
                lock (this.LockObject)
                {
                    return new LockedValueCollection(_dic.Values, this.LockObject);
                }
            }
        }

        public int Capacity
        {
            get
            {
                lock (this.LockObject)
                {
                    return _capacity ?? 0;
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
                if (_capacity != null && _dic.Count + 1 > _capacity.Value) throw new OverflowException();

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
                    return (ICollection)this.Keys;
                }
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                lock (this.LockObject)
                {
                    return (ICollection)this.Values;
                }
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                lock (this.LockObject)
                {
                    return this[(TKey)key];
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

        void IDictionary.Add(object key, object value)
        {
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

        #region IThisLock

        public object LockObject
        {
            get
            {
                return _lockObject;
            }
        }

        #endregion

        public sealed class LockedKeyCollection : ICollection<TKey>, IEnumerable<TKey>, ICollection, IEnumerable, ISynchronized
        {
            private ICollection<TKey> _collection;
            private readonly object _lockObject;

            internal LockedKeyCollection(ICollection<TKey> collection, object lockObject)
            {
                _collection = collection;
                _lockObject = lockObject;
            }

            public TKey[] ToArray()
            {
                lock (this.LockObject)
                {
                    var array = new TKey[_collection.Count];
                    _collection.CopyTo(array, 0);

                    return array;
                }
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                lock (this.LockObject)
                {
                    _collection.CopyTo(array, arrayIndex);
                }
            }

            public int Count
            {
                get
                {
                    lock (this.LockObject)
                    {
                        return _collection.Count;
                    }
                }
            }

            bool ICollection<TKey>.IsReadOnly
            {
                get
                {
                    lock (this.LockObject)
                    {
                        return true;
                    }
                }
            }

            void ICollection<TKey>.Add(TKey item)
            {
                lock (this.LockObject)
                {
                    throw new NotSupportedException();
                }
            }

            void ICollection<TKey>.Clear()
            {
                lock (this.LockObject)
                {
                    throw new NotSupportedException();
                }
            }

            bool ICollection<TKey>.Contains(TKey item)
            {
                lock (this.LockObject)
                {
                    return _collection.Contains(item);
                }
            }

            bool ICollection<TKey>.Remove(TKey item)
            {
                lock (this.LockObject)
                {
                    throw new NotSupportedException();
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
                    ((ICollection)_collection).CopyTo(array, index);
                }
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                lock (this.LockObject)
                {
                    foreach (var item in _collection)
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

            #region IThisLock

            public object LockObject
            {
                get
                {
                    return _lockObject;
                }
            }

            #endregion
        }

        public sealed class LockedValueCollection : ICollection<TValue>, IEnumerable<TValue>, ICollection, IEnumerable, ISynchronized
        {
            private ICollection<TValue> _collection;
            private readonly object _lockObject;

            internal LockedValueCollection(ICollection<TValue> collection, object lockObject)
            {
                _collection = collection;
                _lockObject = lockObject;
            }

            public TValue[] ToArray()
            {
                lock (this.LockObject)
                {
                    var array = new TValue[_collection.Count];
                    _collection.CopyTo(array, 0);

                    return array;
                }
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                lock (this.LockObject)
                {
                    _collection.CopyTo(array, arrayIndex);
                }
            }

            public int Count
            {
                get
                {
                    lock (this.LockObject)
                    {
                        return _collection.Count;
                    }
                }
            }

            bool ICollection<TValue>.IsReadOnly
            {
                get
                {
                    lock (this.LockObject)
                    {
                        return true;
                    }
                }
            }

            void ICollection<TValue>.Add(TValue item)
            {
                lock (this.LockObject)
                {
                    throw new NotSupportedException();
                }
            }

            void ICollection<TValue>.Clear()
            {
                lock (this.LockObject)
                {
                    throw new NotSupportedException();
                }
            }

            bool ICollection<TValue>.Contains(TValue item)
            {
                lock (this.LockObject)
                {
                    return _collection.Contains(item);
                }
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                lock (this.LockObject)
                {
                    throw new NotSupportedException();
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
                    ((ICollection)_collection).CopyTo(array, index);
                }
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                lock (this.LockObject)
                {
                    foreach (var item in _collection)
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

            #region IThisLock

            public object LockObject
            {
                get
                {
                    return _lockObject;
                }
            }

            #endregion
        }
    }
}
