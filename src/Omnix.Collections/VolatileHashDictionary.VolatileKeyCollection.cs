using System;
using System.Collections;
using System.Collections.Generic;
using Omnix.Base;

namespace Omnix.Collections
{
    public partial class VolatileHashDictionary<TKey, TValue>
    {
        public sealed class VolatileKeyCollection : ICollection<TKey>, IEnumerable<TKey>, ICollection, IEnumerable, ISynchronized
        {
            private readonly ICollection<TKey> _collection;

            internal VolatileKeyCollection(ICollection<TKey> collection, object lockObject)
            {
                _collection = collection;
                this.LockObject = lockObject;
            }

            public object LockObject { get; }

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

            bool ICollection<TKey>.IsReadOnly => true;

            void ICollection<TKey>.Add(TKey item) => throw new NotSupportedException();

            void ICollection<TKey>.Clear() => throw new NotSupportedException();

            bool ICollection<TKey>.Remove(TKey item) => throw new NotSupportedException();

            bool ICollection<TKey>.Contains(TKey item)
            {
                lock (this.LockObject)
                {
                    return _collection.Contains(item);
                }
            }

            bool ICollection.IsSynchronized => true;

            object ICollection.SyncRoot => this.LockObject;

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
        }
    }
}
