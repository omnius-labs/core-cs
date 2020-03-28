using System;
using System.Collections;
using System.Collections.Generic;

namespace Omnius.Core.Collections
{
    public partial class VolatileDictionary<TKey, TValue>
    {
        public sealed class VolatileKeyCollection : ICollection<TKey>, IEnumerable<TKey>, IEnumerable
        {
            private readonly ICollection<TKey> _collection;

            internal VolatileKeyCollection(ICollection<TKey> collection)
            {
                _collection = collection;
            }

            public TKey[] ToArray()
            {
                var array = new TKey[_collection.Count];
                _collection.CopyTo(array, 0);

                return array;
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                _collection.CopyTo(array, arrayIndex);
            }

            public int Count => _collection.Count;

            bool ICollection<TKey>.IsReadOnly => true;

            void ICollection<TKey>.Add(TKey item) => throw new NotSupportedException();

            void ICollection<TKey>.Clear() => throw new NotSupportedException();

            bool ICollection<TKey>.Remove(TKey item) => throw new NotSupportedException();

            bool ICollection<TKey>.Contains(TKey item)
            {
                return _collection.Contains(item);
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                foreach (var item in _collection)
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
}
