using System;
using System.Collections;
using System.Collections.Generic;
using Omnix.Base;

namespace Omnix.DataStructures
{
    public class LockedList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, ISynchronized
    {
        private readonly List<T> _list;
        private int? _capacity;

        public LockedList()
        {
            _list = new List<T>();
        }

        public LockedList(int capacity)
        {
            _list = new List<T>();
            _capacity = capacity;
        }

        public LockedList(IEnumerable<T> collection)
        {
            _list = new List<T>();

            foreach (var item in collection)
            {
                this.Add(item);
            }
        }

        public object LockObject { get; } = new object();

        protected virtual bool Filter(T item)
        {
            return false;
        }

        public T[] ToArray()
        {
            lock (this.LockObject)
            {
                var array = new T[_list.Count];
                _list.CopyTo(array, 0);

                return array;
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

        public int Count
        {
            get
            {
                lock (this.LockObject)
                {
                    return _list.Count;
                }
            }
        }

        public T this[int index]
        {
            get
            {
                lock (this.LockObject)
                {
                    return _list[index];
                }
            }
            set
            {
                lock (this.LockObject)
                {
                    _list[index] = value;
                }
            }
        }

        public void Add(T item)
        {
            lock (this.LockObject)
            {
                if (_capacity != null && _list.Count + 1 > _capacity.Value)
                {
                    throw new OverflowException();
                }

                if (this.Filter(item))
                {
                    return;
                }

                // 姑息な、メモリ消費量を減少させる策。
                if (_list.Count < 16)
                {
                    _list.Capacity = _list.Count + 1;
                }

                _list.Add(item);
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            lock (this.LockObject)
            {
                foreach (var item in collection)
                {
                    this.Add(item);
                }
            }
        }

        public void Clear()
        {
            lock (this.LockObject)
            {
                _list.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (this.LockObject)
            {
                return _list.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this.LockObject)
            {
                _list.CopyTo(array, arrayIndex);
            }
        }

        public LockedList<T> GetRange(int index, int count)
        {
            lock (this.LockObject)
            {
                return new LockedList<T>(_list.GetRange(index, count));
            }
        }

        public void Sort(IComparer<T> comparer)
        {
            lock (this.LockObject)
            {
                _list.Sort(comparer);
            }
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            lock (this.LockObject)
            {
                _list.Sort(index, count, comparer);
            }
        }

        public void Sort(Comparison<T> comparerison)
        {
            lock (this.LockObject)
            {
                _list.Sort(comparerison);
            }
        }

        public void Sort()
        {
            lock (this.LockObject)
            {
                _list.Sort();
            }
        }

        public void Reverse()
        {
            lock (this.LockObject)
            {
                _list.Reverse();
            }
        }

        public void Reverse(int index, int count)
        {
            lock (this.LockObject)
            {
                _list.Reverse(index, count);
            }
        }

        public int IndexOf(T item)
        {
            lock (this.LockObject)
            {
                return _list.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (this.LockObject)
            {
                _list.Insert(index, item);
            }
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            lock (this.LockObject)
            {
                _list.InsertRange(index, collection);
            }
        }

        public bool Remove(T item)
        {
            lock (this.LockObject)
            {
                return _list.Remove(item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (this.LockObject)
            {
                _list.RemoveAt(index);
            }
        }

        public void RemoveRange(int index, int count)
        {
            lock (this.LockObject)
            {
                _list.RemoveRange(index, count);
            }
        }

        public int RemoveAll(Predicate<T> match)
        {
            lock (this.LockObject)
            {
                return _list.RemoveAll(match);
            }
        }

        public void TrimExcess()
        {
            lock (this.LockObject)
            {
                _list.TrimExcess();
            }
        }

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        object IList.this[int index]
        {
            get
            {
                lock (this.LockObject)
                {
                    return this[index]!;
                }
            }
            set
            {
                lock (this.LockObject)
                {
                    this[index] = (T)value;
                }
            }
        }

        int IList.Add(object item)
        {
            lock (this.LockObject)
            {
                this.Add((T)item);
                return _list.Count - 1;
            }
        }

        bool IList.Contains(object item)
        {
            lock (this.LockObject)
            {
                return this.Contains((T)item);
            }
        }

        int IList.IndexOf(object item)
        {
            lock (this.LockObject)
            {
                return this.IndexOf((T)item);
            }
        }

        void IList.Insert(int index, object item)
        {
            lock (this.LockObject)
            {
                this.Insert(index, (T)item);
            }
        }

        void IList.Remove(object item)
        {
            lock (this.LockObject)
            {
                this.Remove((T)item);
            }
        }

        bool ICollection<T>.IsReadOnly => false;

        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot => this.LockObject;

        void ICollection.CopyTo(Array array, int index)
        {
            lock (this.LockObject)
            {
                ((ICollection)_list).CopyTo(array, index);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (this.LockObject)
            {
                foreach (var item in _list)
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
