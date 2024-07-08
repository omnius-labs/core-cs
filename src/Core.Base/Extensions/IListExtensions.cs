namespace Omnius.Core.Base;

public static class IListExtensions
{
    public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        if (items == null) throw new ArgumentNullException(nameof(items));

        foreach (var item in items)
        {
            list.Add(item);
        }
    }

    public static T[] Replace<T>(this IList<T> list, IEnumerable<T> items)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        if (items == null) throw new ArgumentNullException(nameof(items));

        var oldItems = list.ToArray();
        list.Clear();
        list.AddRange(items);

        return oldItems;
    }
}
