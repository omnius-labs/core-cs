using System.Collections;
using System.Collections.Generic;

namespace Omnius.Core.Collections
{
    public class LockedList<TItem> : IList<TItem>, ICollection<TItem>, IEnumerable<TItem>, ISynchronized
    {
        private readonly IList<TItem> _list;

        public LockedList(IList<TItem> list)
        {
            _list = list;
        }

        public object LockObject { get; } = new object();

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

        public bool IsReadOnly
        {
            get
            {
                lock (this.LockObject)
                {
                    return _list.IsReadOnly;
                }
            }
        }

        public TItem[] ToArray()
        {
            lock (this.LockObject)
            {
                var array = new TItem[_list.Count];
                _list.CopyTo(array, 0);

                return array;
            }
        }

        public TItem this[int index]
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

        public void Add(TItem item)
        {
            lock (this.LockObject)
            {
                _list.Add(item);
            }
        }

        public void AddRange(IEnumerable<TItem> collection)
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

        public bool Contains(TItem item)
        {
            lock (this.LockObject)
            {
                return _list.Contains(item);
            }
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            lock (this.LockObject)
            {
                _list.CopyTo(array, arrayIndex);
            }
        }

        public int IndexOf(TItem item)
        {
            lock (this.LockObject)
            {
                return _list.IndexOf(item);
            }
        }

        public void Insert(int index, TItem item)
        {
            lock (this.LockObject)
            {
                _list.Insert(index, item);
            }
        }

        public bool Remove(TItem item)
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

        public IEnumerator<TItem> GetEnumerator()
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
