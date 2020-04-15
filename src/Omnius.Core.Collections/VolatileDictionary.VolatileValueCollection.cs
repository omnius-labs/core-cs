using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Omnius.Core.Collections
{
    public partial class VolatileDictionary<TKey, TValue>
    {
        public sealed class VolatileValueCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable
        {
            private readonly ICollection<Info<TValue>> _collection;

            internal VolatileValueCollection(ICollection<Info<TValue>> collection)
            {
                _collection = collection;
            }

            public TValue[] ToArray()
            {
                return _collection.Select(n => n.Value).ToArray();
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                foreach (var info in _collection)
                {
                    array[arrayIndex++] = info.Value;
                }
            }

            public int Count => _collection.Count;

            bool ICollection<TValue>.IsReadOnly => true;

            void ICollection<TValue>.Add(TValue item) => throw new NotSupportedException();

            void ICollection<TValue>.Clear() => throw new NotSupportedException();

            bool ICollection<TValue>.Remove(TValue item) => throw new NotSupportedException();

            bool ICollection<TValue>.Contains(TValue item)
            {
                return _collection.Select(n => n.Value).Contains(item);
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                foreach (var info in _collection)
                {
                    yield return info.Value;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
