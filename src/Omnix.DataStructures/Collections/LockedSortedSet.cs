using System;
using System.Collections;
using System.Collections.Generic;
using Omnix.Base;

namespace Omnix.DataStructures.Collections
{
    public class LockedSortedSet<T> : ISet<T>, ICollection<T>, IEnumerable<T>, ICollection, IEnumerable, ISynchronized
    {
        private readonly SortedSet<T> _sortedSet;
        private int? _capacity;

        public LockedSortedSet()
        {
            _sortedSet = new SortedSet<T>();
        }

        public LockedSortedSet(int capacity)
        {
            _sortedSet = new SortedSet<T>();
            _capacity = capacity;
        }

        public LockedSortedSet(IEnumerable<T> collection)
        {
            _sortedSet = new SortedSet<T>(collection);
        }

        public LockedSortedSet(IComparer<T> comparer)
        {
            _sortedSet = new SortedSet<T>(comparer);
        }

        public LockedSortedSet(int capacity, IComparer<T> comparer)
        {
            _sortedSet = new SortedSet<T>(comparer);
            _capacity = capacity;
        }

        public LockedSortedSet(IEnumerable<T> collection, IComparer<T> comparer)
        {
            _sortedSet = new SortedSet<T>(collection, comparer);
        }

        public object LockObject { get; } = new object();

        public T[] ToArray()
        {
            lock (this.LockObject)
            {
                var array = new T[_sortedSet.Count];
                _sortedSet.CopyTo(array, 0);

                return array;
            }
        }

        public IComparer<T> Comparer
        {
            get
            {
                lock (this.LockObject)
                {
                    return _sortedSet.Comparer;
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

        public bool Add(T item)
        {
            lock (this.LockObject)
            {
                if (_capacity != null && _sortedSet.Count + 1 > _capacity.Value)
                {
                    throw new OverflowException();
                }

                return _sortedSet.Add(item);
            }
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                _sortedSet.ExceptWith(other);
            }
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                _sortedSet.IntersectWith(other);
            }
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                return _sortedSet.IsProperSubsetOf(other);
            }
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                return _sortedSet.IsProperSupersetOf(other);
            }
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                return _sortedSet.IsSubsetOf(other);
            }
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                return _sortedSet.IsSupersetOf(other);
            }
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                return _sortedSet.Overlaps(other);
            }
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                return _sortedSet.SetEquals(other);
            }
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            lock (this.LockObject)
            {
                _sortedSet.SymmetricExceptWith(other);
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
                _sortedSet.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (this.LockObject)
            {
                return _sortedSet.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this.LockObject)
            {
                _sortedSet.CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get
            {
                lock (this.LockObject)
                {
                    return _sortedSet.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public bool Remove(T item)
        {
            lock (this.LockObject)
            {
                return _sortedSet.Remove(item);
            }
        }

        void ICollection<T>.Add(T item)
        {
            lock (this.LockObject)
            {
                this.Add(item);
            }
        }

        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot => this.LockObject;

        void ICollection.CopyTo(Array array, int index)
        {
            lock (this.LockObject)
            {
                ((ICollection)_sortedSet).CopyTo(array, index);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (this.LockObject)
            {
                foreach (var item in _sortedSet)
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
