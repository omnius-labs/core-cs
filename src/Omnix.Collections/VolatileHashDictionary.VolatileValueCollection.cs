using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Omnix.Base;

namespace Omnix.Collections
{
    public partial class VolatileHashDictionary<TKey, TValue>
    {
        public sealed class VolatileValueCollection : ICollection<TValue>, IEnumerable<TValue>, ICollection, IEnumerable, ISynchronized
        {
            private readonly ICollection<Info<TValue>> _collection;

            internal VolatileValueCollection(ICollection<Info<TValue>> collection, object lockObject)
            {
                _collection = collection;
                this.LockObject = lockObject;
            }

            public object LockObject { get; }

            public TValue[] ToArray()
            {
                lock (this.LockObject)
                {
                    return _collection.Select(n => n.Value).ToArray();
                }
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                lock (this.LockObject)
                {
                    foreach (var info in _collection)
                    {
                        array[arrayIndex++] = info.Value;
                    }
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

            bool ICollection<TValue>.IsReadOnly => true;

            void ICollection<TValue>.Add(TValue item) => throw new NotSupportedException();

            void ICollection<TValue>.Clear() => throw new NotSupportedException();

            bool ICollection<TValue>.Remove(TValue item) => throw new NotSupportedException();

            bool ICollection<TValue>.Contains(TValue item)
            {
                lock (this.LockObject)
                {
                    return _collection.Select(n => n.Value).Contains(item);
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

            public IEnumerator<TValue> GetEnumerator()
            {
                lock (this.LockObject)
                {
                    foreach (var info in _collection)
                    {
                        yield return info.Value;
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
