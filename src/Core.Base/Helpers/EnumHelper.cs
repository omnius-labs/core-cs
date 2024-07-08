using System.Collections.Concurrent;

namespace Omnius.Core.Base.Helpers;

public static class EnumHelper
{
    public static bool IsValid<T>(T value)
        where T : Enum
    {
        return EnumInfo<T>.IsValid(value);
    }

    private static class EnumInfo<T>
        where T : Enum
    {
        private static readonly ConcurrentDictionary<T, bool> _resultMap = new();

        public static bool IsValid(T value)
        {
            return _resultMap.GetOrAdd(value, (_) => Enum.IsDefined(typeof(T), value));
        }
    }

    public static T? GetOverlappedMaxValue<T>(IEnumerable<T> s1, IEnumerable<T> s2)
        where T : struct, Enum
    {
        var list = s1.ToList();
        list.Sort((x, y) => y.CompareTo(x));

        var hashSet = new HashSet<T>(s2);

        foreach (var item in list)
        {
            if (hashSet.Contains(item)) return item;
        }

        return null;
    }
}
