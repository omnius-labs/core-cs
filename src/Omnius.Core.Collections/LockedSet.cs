using System.Collections;
using System.Collections.Generic;

namespace Omnius.Core.Collections
{
    public class LockedSet<TItem> : ISet<TItem>, ICollection<TItem>, IEnumerable<TItem>, ISynchronized
    {
        private readonly ISet<TItem> _set;

        public LockedSet(ISet<TItem> set)
        {
            _set = set;
        }

        public object LockObject { get; } = new object();

        public int Count
        {
            get
            {
                lock (this.LockObject)
                {
                    return _set.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                lock (this.LockObject)
                {
                    return _set.IsReadOnly;
                }
            }
        }

        public TItem[] ToArray()
        {
            lock (this.LockObject)
            {
                var array = new TItem[_set.Count];
                _set.CopyTo(array, 0);

                return array;
            }
        }

        public bool Add(TItem item)
        {
            lock (this.LockObject)
            {
                return _set.Add(item);
            }
        }

        public void ExceptWith(IEnumerable<TItem> other)
        {
            lock (this.LockObject)
            {
                _set.ExceptWith(other);
            }
        }

        public void IntersectWith(IEnumerable<TItem> other)
        {
            lock (this.LockObject)
            {
                _set.IntersectWith(other);
            }
        }

        public bool IsProperSubsetOf(IEnumerable<TItem> other)
        {
            lock (this.LockObject)
            {
                return _set.IsProperSubsetOf(other);
            }
        }

        public bool IsProperSupersetOf(IEnumerable<TItem> other)
        {
            lock (this.LockObject)
            {
                return _set.IsProperSupersetOf(other);
            }
        }

        public bool IsSubsetOf(IEnumerable<TItem> other)
        {
            lock (this.LockObject)
            {
                return _set.IsSubsetOf(other);
            }
        }

        public bool IsSupersetOf(IEnumerable<TItem> other)
        {
            lock (this.LockObject)
            {
                return _set.IsSupersetOf(other);
            }
        }

        public bool Overlaps(IEnumerable<TItem> other)
        {
            lock (this.LockObject)
            {
                return _set.Overlaps(other);
            }
        }

        public bool SetEquals(IEnumerable<TItem> other)
        {
            lock (this.LockObject)
            {
                return _set.SetEquals(other);
            }
        }

        public void SymmetricExceptWith(IEnumerable<TItem> other)
        {
            lock (this.LockObject)
            {
                _set.SymmetricExceptWith(other);
            }
        }

        public void UnionWith(IEnumerable<TItem> other)
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
                _set.Clear();
            }
        }

        public bool Contains(TItem item)
        {
            lock (this.LockObject)
            {
                return _set.Contains(item);
            }
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            lock (this.LockObject)
            {
                _set.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(TItem item)
        {
            lock (this.LockObject)
            {
                return _set.Remove(item);
            }
        }

        void ICollection<TItem>.Add(TItem item)
        {
            lock (this.LockObject)
            {
                this.Add(item);
            }
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            lock (this.LockObject)
            {
                foreach (var item in _set)
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
