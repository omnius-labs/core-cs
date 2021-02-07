using System;
using System.Collections;
using System.Collections.Generic;

namespace Omnius.Core.Collections
{
    public sealed class ReadOnlyListSlim<T> : IReadOnlyList<T>
    {
        public static ReadOnlyListSlim<T> Empty { get; }

        static ReadOnlyListSlim()
        {
            Empty = new ReadOnlyListSlim<T>(Array.Empty<T>());
        }

        private readonly T[] _array;

        public ReadOnlyListSlim(T[] array)
        {
            _array = array;
        }

        public T this[int index] => _array[index];

        public T[] Slice(int start, int length)
        {
            var slice = new T[length];
            Array.Copy(_array, start, slice, 0, length);
            return slice;
        }

        public int Count => _array.Length;

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_array);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public struct Enumerator : IEnumerator<T>
        {
            private readonly T[] _array;
            private int _i;

            internal Enumerator(T[] array) => (_array, _i) = (array, -1);

            public T Current => _array[_i];

            object? IEnumerator.Current => this.Current;

            public bool MoveNext() => ((uint)++_i) < (uint)_array.Length;

            public void Reset() => throw new NotSupportedException();

            public void Dispose()
            {
            }
        }
    }
}
