using System;
using System.Collections;
using System.Collections.Generic;
using Omnix.Base;

namespace Omnix.DataStructures
{
    public class LockedHashSet<T> : ISet<T>, ICollection<T>, IEnumerable<T>, ICollection, IEnumerable, ISynchronized
    {
        private readonly HashSet<T> _hashSet;
        private int? _capacity;

        private readonly object _lockObject = new object();

        public LockedHashSet()
        {
            _hashSet = new HashSet<T>();
        }

        public LockedHashSet(int capacity)
        {
            _hashSet = new HashSet<T>();
            _capacity = capacity;
        }

        public LockedHashSet(IEnumerable<T> collection)
        {
            _hashSet = new HashSet<T>(collection);
        }

        public LockedHashSet(IEqualityComparer<T> comparer)
        {
            _hashSet = new HashSet<T>(comparer);
        }

        public LockedHashSet(int capacity, IEqualityComparer<T> comparer)
        {
            _hashSet = new HashSet<T>(comparer);
            _capacity = capacity;
        }

        public LockedHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            _hashSet = new HashSet<T>(collection, comparer);
        }

        public object LockObject { get; } = new object();

        public T[] ToArray()
        {
            lock (this.LockObject)
            {
                var array = new T[_hashSet.Count];
                _hashSet.CopyTo(array, 0);

                return array;
            }
        }

        public IEqualityComparer<T> Comparer
        {
            get
            {
                lock (this.LockObject)
                {
                    return _hashSet.Comparer;
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

        public void TrimExcess()
        {
            lock (this.LockObject)
            {
                _hashSet.TrimExcess();
            }
        }

        public bool Add(T item)
        {
            lock (this.LockObject)
            {
                if (_capacity != null && _hashSet.Count + 1 > _capacity.Value)
                {
                    throw new OverflowException();
                }

                return _hashSet.Add(item);
            }
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                _hashSet.ExceptWith(other);
            }
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                _hashSet.IntersectWith(other);
            }
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                return _hashSet.IsProperSubsetOf(other);
            }
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                return _hashSet.IsProperSupersetOf(other);
            }
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                return _hashSet.IsSubsetOf(other);
            }
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                return _hashSet.IsSupersetOf(other);
            }
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                return _hashSet.Overlaps(other);
            }
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                return _hashSet.SetEquals(other);
            }
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                _hashSet.SymmetricExceptWith(other);
            }
        }

        public void UnionWith(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                foreach (var item in other)
                {
                    this.Add(item);
                }
            }
        }

        public void Clear()
        {
            lock (this.LockObject)
            {
                _hashSet.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (this.LockObject)
            {
                return _hashSet.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this.LockObject)
            {
                _hashSet.CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get
            {
                lock (this.LockObject)
                {
                    return _hashSet.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                lock (this.LockObject)
                {
                    return false;
                }
            }
        }

        public bool Remove(T item)
        {
            lock (this.LockObject)
            {
                return _hashSet.Remove(item);
            }
        }

        void ICollection<T>.Add(T item)
        {
            lock (this.LockObject)
            {
                this.Add(item);
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
                ((ICollection)_hashSet).CopyTo(array, index);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (this.LockObject)
            {
                foreach (var item in _hashSet)
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
