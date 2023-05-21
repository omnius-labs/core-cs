using System.Collections;
using System.Diagnostics;

namespace Omnius.Core;

public static class IEnumerableExtensions
{
    public static bool Contains(this IEnumerable items, object item)
    {
        return items.IndexOf(item) != -1;
    }

    public static int Count(this IEnumerable items)
    {
        if (items != null)
        {
            if (items is ICollection collection)
            {
                return collection.Count;
            }
            else
            {
                return Enumerable.Count(items.Cast<object>());
            }
        }
        else
        {
            return 0;
        }
    }

    public static int IndexOf(this IEnumerable items, object item)
    {
        if (items is IList list)
        {
            return list.IndexOf(item);
        }
        else
        {
            int index = 0;

            foreach (var i in items)
            {
                if (ReferenceEquals(i, item)) return index;

                ++index;
            }

            return -1;
        }
    }

    public static object? ElementAt(this IEnumerable items, int index)
    {
        if (items is IList list)
        {
            return list[index];
        }
        else
        {
            return Enumerable.ElementAt(items.Cast<object>(), index);
        }
    }

    private static readonly Lazy<Random> _random = new(() => new Random());

    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> items)
    {
        var array = items.ToList();
        _random.Value.Shuffle(array);
        return array;
    }

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> items)
    {
        foreach (var collection in items)
        {
            foreach (var result in collection)
            {
                yield return result;
            }
        }
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
        where T : class
    {
        if (source == null) return Enumerable.Empty<T>();
        return source.Where(n => n != null)!;
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<Nullable<T>> source)
        where T : struct
    {
        if (source == null) return Enumerable.Empty<T>();
        return source.Where(n => n.HasValue).Select(n => n!.Value);
    }

    public static IEnumerable<ElementWithContext<T>> WithContext<T>(this IEnumerable<T> source)
    {
        var e = source.GetEnumerator();

        // len = 0
        if (!e.MoveNext()) yield break;
        T current = e.Current;

        // len = 1
        if (!e.MoveNext())
        {
            yield return new ElementWithContext<T>(default(T), current, default(T));
            yield break;
        }

        T? previous = default(T);
        T next = e.Current;

        while (!e.MoveNext())
        {
            yield return new ElementWithContext<T>(previous, current, next);
            previous = current;
            current = next;
            next = e.Current;
        }

        // tail
        yield return new ElementWithContext<T>(previous, current, default(T));
    }
}

[DebuggerDisplay("Previous = {previous}, Current = {current}, Next = {next}")]
public readonly struct ElementWithContext<T>
{
    public readonly T? Previous;
    public readonly T Current;
    public readonly T? Next;

    public ElementWithContext(T? previous, T current, T? next)
    {
        Previous = previous;
        Current = current;
        Next = next;
    }

    public void Deconstruct(out T? previous, out T current, out T? next)
    {
        previous = this.Previous;
        current = this.Current;
        next = this.Next;
    }
}
