using System;
using System.Collections;
using System.Collections.Generic;

namespace Omnix.Collections
{
    public class SmallList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
    {
        private List<T> _list;
        private int? _capacity;

        public SmallList()
        {
            _list = new List<T>();
        }

        public SmallList(int capacity)
        {
            _list = new List<T>();
            _capacity = capacity;
        }

        public SmallList(IEnumerable<T> collection)
        {
            _list = new List<T>();

            foreach (var item in collection)
            {
                this.Add(item);
            }
        }

        public int? Capacity
        {
            get
            {
                return _capacity;
            }
            set
            {
                _capacity = value;
            }
        }

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        protected virtual bool Filter(T item)
        {
            return false;
        }

        public T[] ToArray()
        {
            var array = new T[_list.Count];
            _list.CopyTo(array, 0);

            return array;
        }

        public T this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }

        public void Add(T item)
        {
            if (_capacity != null && _list.Count + 1 > _capacity.Value) throw new OverflowException();
            if (this.Filter(item)) return;

            _list.Capacity = _list.Count + 1;

            _list.Add(item);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                this.Add(item);
            }
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public SmallList<T> GetRange(int index, int count)
        {
            return new SmallList<T>(_list.GetRange(index, count));
        }

        public void Sort(IComparer<T> comparer)
        {
            _list.Sort(comparer);
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            _list.Sort(index, count, comparer);
        }

        public void Sort(Comparison<T> comparerison)
        {
            _list.Sort(comparerison);
        }

        public void Sort()
        {
            _list.Sort();
        }

        public void Reverse()
        {
            _list.Reverse();
        }

        public void Reverse(int index, int count)
        {
            _list.Reverse(index, count);
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            _list.InsertRange(index, collection);
        }

        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            _list.RemoveRange(index, count);
        }

        public int RemoveAll(Predicate<T> match)
        {
            return _list.RemoveAll(match);
        }

        public void TrimExcess()
        {
            _list.TrimExcess();
        }

        bool ICollection<T>.IsReadOnly => false;

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (T)value;
            }
        }

        int IList.Add(object item)
        {
            this.Add((T)item);
            return _list.Count - 1;
        }

        bool IList.Contains(object item)
        {
            return this.Contains((T)item);
        }

        int IList.IndexOf(object item)
        {
            return this.IndexOf((T)item);
        }

        void IList.Insert(int index, object item)
        {
            this.Insert(index, (T)item);
        }

        void IList.Remove(object item)
        {
            this.Remove((T)item);
        }

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => null;

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_list).CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _list)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
