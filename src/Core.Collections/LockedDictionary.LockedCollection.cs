using System.Collections;
using Core.Base;

namespace Core.Collections;

public partial class LockedDictionary<TKey, TValue>
{
    public sealed class LockedCollection<T> : ICollection<T>, IEnumerable<T>, ISynchronized
    {
        private readonly ICollection<T> _collection;

        internal LockedCollection(ICollection<T> collection, object lockObject)
        {
            _collection = collection;
            this.LockObject = lockObject;
        }

        public object LockObject { get; }

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

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this.LockObject)
            {
                _collection.CopyTo(array, arrayIndex);
            }
        }

        public T[] ToArray()
        {
            lock (this.LockObject)
            {
                var array = new T[_collection.Count];
                _collection.CopyTo(array, 0);

                return array;
            }
        }

        bool ICollection<T>.IsReadOnly => true;

        void ICollection<T>.Add(T item) => throw new NotSupportedException();

        void ICollection<T>.Clear() => throw new NotSupportedException();

        bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

        bool ICollection<T>.Contains(T item)
        {
            lock (this.LockObject)
            {
                return _collection.Contains(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
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
    }
}
