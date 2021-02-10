using System;
using System.Collections.Generic;

namespace Omnius.Core.Helpers
{
    public static class CollectionHelper
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static new bool Equals(object obj1, object obj2)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<T> Unite<T>(params IEnumerable<T>[] source)
        {
            foreach (var collection in source)
            {
                foreach (var item in collection)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> Unite<T>(IEnumerable<IEnumerable<T>> source)
        {
            foreach (var collection in source)
            {
                foreach (var item in collection)
                {
                    yield return item;
                }
            }
        }

        public static bool Equals<T>(IList<T> source, IList<T> destination, IEqualityComparer<T> equalityComparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (equalityComparer == null) throw new ArgumentNullException(nameof(equalityComparer));

            if (object.ReferenceEquals(source, destination)) return true;
            if (source.Count != destination.Count) return false;

            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (!equalityComparer.Equals(source[i], destination[i])) return false;
            }

            return true;
        }

        public static bool Equals<T>(IEnumerable<T> source, IEnumerable<T> destination, IEqualityComparer<T> equalityComparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (equalityComparer == null) throw new ArgumentNullException(nameof(equalityComparer));

            if (object.ReferenceEquals(source, destination)) return true;

            using (var s = source.GetEnumerator())
            using (var d = destination.GetEnumerator())
            {
                while (s.MoveNext())
                {
                    if (!d.MoveNext()) return false;
                    if (!equalityComparer.Equals(s.Current, d.Current)) return false;
                }

                if (d.MoveNext()) return false;
            }

            return true;
        }

        public static bool Equals<T>(IList<T> source, int sourceIndex, IList<T> destination, int destinationIndex, int length, IEqualityComparer<T> equalityComparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (equalityComparer == null) throw new ArgumentNullException(nameof(equalityComparer));
            if ((source.Count - sourceIndex) < 0) throw new ArgumentOutOfRangeException(nameof(sourceIndex));
            if ((destination.Count - destinationIndex) < 0) throw new ArgumentOutOfRangeException(nameof(destinationIndex));
            if (length > (source.Count - sourceIndex)) throw new ArgumentOutOfRangeException(nameof(length));
            if (length > (destination.Count - destinationIndex)) throw new ArgumentOutOfRangeException(nameof(length));

            for (int i = sourceIndex, j = destinationIndex, k = 0; k < length; i++, j++, k++)
            {
                if (!equalityComparer.Equals(source[i], destination[j])) return false;
            }

            return true;
        }

        public static bool Equals<T>(IEnumerable<T> source, int sourceIndex, IEnumerable<T> destination, int destinationIndex, int length, IEqualityComparer<T> equalityComparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (equalityComparer == null) throw new ArgumentNullException(nameof(equalityComparer));

            using (var s = source.GetEnumerator())
            using (var d = destination.GetEnumerator())
            {
                for (int i = 0; i < sourceIndex; i++)
                {
                    if (!s.MoveNext()) throw new ArgumentOutOfRangeException(nameof(sourceIndex));
                }

                for (int i = 0; i < destinationIndex; i++)
                {
                    if (!d.MoveNext()) throw new ArgumentOutOfRangeException(nameof(destinationIndex));
                }

                for (int i = 0; i < length; i++)
                {
                    if (!s.MoveNext()) throw new ArgumentOutOfRangeException(nameof(length));
                    if (!d.MoveNext()) throw new ArgumentOutOfRangeException(nameof(length));

                    if (!equalityComparer.Equals(s.Current, d.Current)) return false;
                }
            }

            return true;
        }

        public static bool Equals<T>(IList<T> source, IList<T> destination)
        {
            if (source is byte[] x && destination is byte[] y) return BytesOperations.Equals(x.AsSpan(), y.AsSpan());

            var equalityComparer = EqualityComparer<T>.Default;
            return CollectionHelper.Equals(source, destination, equalityComparer);
        }

        public static bool Equals<T>(IEnumerable<T> source, IEnumerable<T> destination)
        {
            var equalityComparer = EqualityComparer<T>.Default;
            return CollectionHelper.Equals(source, destination, equalityComparer);
        }

        public static bool Equals<T>(IList<T> source, int sourceIndex, IList<T> destination, int destinationIndex, int length)
        {
            if (source is byte[] x && destination is byte[] y)
            {
                if ((x.Length - sourceIndex) < 0) throw new ArgumentOutOfRangeException(nameof(sourceIndex));
                if ((y.Length - destinationIndex) < 0) throw new ArgumentOutOfRangeException(nameof(destinationIndex));
                if (length > (x.Length - sourceIndex)) throw new ArgumentOutOfRangeException(nameof(length));
                if (length > (y.Length - destinationIndex)) throw new ArgumentOutOfRangeException(nameof(length));

                return BytesOperations.Equals(x.AsSpan(sourceIndex, length), y.AsSpan(destinationIndex, length));
            }

            var equalityComparer = EqualityComparer<T>.Default;
            return Equals(source, sourceIndex, destination, destinationIndex, length, equalityComparer);
        }

        public static bool Equals<T>(IEnumerable<T> source, int sourceIndex, IEnumerable<T> destination, int destinationIndex, int length)
        {
            var equalityComparer = EqualityComparer<T>.Default;
            return Equals(source, sourceIndex, destination, destinationIndex, length, equalityComparer);
        }

        public static int Compare<T>(IList<T> source, IList<T> destination, IComparer<T> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            if (object.ReferenceEquals(source, destination)) return 0;
            if (source.Count != destination.Count) return (source.Count > destination.Count) ? 1 : -1;

            int c = 0;

            for (int i = 0; i < source.Count; i++)
            {
                if ((c = comparer.Compare(source[i], destination[i])) != 0) return c;
            }

            return 0;
        }

        public static int Compare<T>(IEnumerable<T> source, IEnumerable<T> destination, IComparer<T> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            if (object.ReferenceEquals(source, destination)) return 0;

            int c = 0;

            using (var s = source.GetEnumerator())
            using (var d = destination.GetEnumerator())
            {
                while (s.MoveNext())
                {
                    if (!d.MoveNext()) return 1;

                    if ((c = comparer.Compare(s.Current, d.Current)) != 0) break;
                }

                while (s.MoveNext())
                {
                    if (!d.MoveNext()) return 1;
                }

                if (d.MoveNext()) return -1;
            }

            return c;
        }

        public static int Compare<T>(IList<T> source, int sourceIndex, IList<T> destination, int destinationIndex, int length, IComparer<T> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            if ((source.Count - sourceIndex) < 0) throw new ArgumentOutOfRangeException(nameof(sourceIndex));
            if ((destination.Count - destinationIndex) < 0) throw new ArgumentOutOfRangeException(nameof(destinationIndex));
            if (length > (source.Count - sourceIndex)) throw new ArgumentOutOfRangeException(nameof(length));
            if (length > (destination.Count - destinationIndex)) throw new ArgumentOutOfRangeException(nameof(length));

            int c;

            for (int i = sourceIndex, j = destinationIndex, k = 0; k < length; i++, j++, k++)
            {
                if ((c = comparer.Compare(source[i], destination[j])) != 0) return c;
            }

            return 0;
        }

        public static int Compare<T>(IEnumerable<T> source, int sourceIndex, IEnumerable<T> destination, int destinationIndex, int length, IComparer<T> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            using (var s = source.GetEnumerator())
            using (var d = destination.GetEnumerator())
            {
                for (int i = 0; i < sourceIndex; i++)
                {
                    if (!s.MoveNext()) throw new ArgumentOutOfRangeException(nameof(sourceIndex));
                }

                for (int i = 0; i < destinationIndex; i++)
                {
                    if (!d.MoveNext()) throw new ArgumentOutOfRangeException(nameof(destinationIndex));
                }

                int c = 0;

                for (int i = 0; i < length; i++)
                {
                    if (!s.MoveNext()) throw new ArgumentOutOfRangeException(nameof(length));
                    if (!d.MoveNext()) throw new ArgumentOutOfRangeException(nameof(length));

                    if ((c = comparer.Compare(s.Current, d.Current)) != 0) return c;
                }
            }

            return 0;
        }

        public static int Compare<T>(IList<T> source, IList<T> destination)
        {
            if (source is byte[] x && destination is byte[] y) return BytesOperations.Compare(x, y);

            var compare = Comparer<T>.Default;
            return CollectionHelper.Compare(source, destination, compare);
        }

        public static int Compare<T>(IEnumerable<T> source, IEnumerable<T> destination)
        {
            var compare = Comparer<T>.Default;
            return CollectionHelper.Compare(source, destination, compare);
        }

        public static int Compare<T>(IList<T> source, int sourceIndex, IList<T> destination, int destinationIndex, int length)
        {
            if (source is byte[] x && destination is byte[] y)
            {
                if ((source.Count - sourceIndex) < 0) throw new ArgumentOutOfRangeException(nameof(sourceIndex));
                if ((destination.Count - destinationIndex) < 0) throw new ArgumentOutOfRangeException(nameof(destinationIndex));
                if (length > (source.Count - sourceIndex)) throw new ArgumentOutOfRangeException(nameof(length));
                if (length > (destination.Count - destinationIndex)) throw new ArgumentOutOfRangeException(nameof(length));

                return BytesOperations.Compare(x.AsSpan(sourceIndex, length), y.AsSpan(destinationIndex, length));
            }

            var compare = Comparer<T>.Default;
            return CollectionHelper.Compare(source, sourceIndex, destination, destinationIndex, length, compare);
        }

        public static int Compare<T>(IEnumerable<T> source, int sourceIndex, IEnumerable<T> destination, int destinationIndex, int length)
        {
            var compare = Comparer<T>.Default;
            return CollectionHelper.Compare(source, sourceIndex, destination, destinationIndex, length, compare);
        }
    }
}
