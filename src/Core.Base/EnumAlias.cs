using System.Reflection;
using System.Runtime.Serialization;

namespace Omnius.Core.Base;

public static class EnumAlias<T> where T : struct, Enum
{
    private static readonly IReadOnlyDictionary<T, string> ToAlias;
    private static readonly IReadOnlyDictionary<string, T> FromAlias;

    static EnumAlias()
    {
        var type = typeof(T);
        var pairs = Enum.GetValues<T>()
            .Select(v =>
            {
                var name = Enum.GetName(type, v)!;
                var fi = type.GetField(name)!;
                var attr = fi.GetCustomAttribute<EnumMemberAttribute>();
                var alias = attr?.Value ?? name;
                return (v, alias);
            })
            .ToArray();

        ToAlias = pairs.ToDictionary(x => x.v, x => x.alias);
        FromAlias = pairs.ToDictionary(x => x.alias, x => x.v, StringComparer.OrdinalIgnoreCase);
    }

    public static string ToStringAlias(T v) => ToAlias[v];

    public static bool TryParseAlias(string s, out T v) =>
        FromAlias.TryGetValue(s, out v);

    public static T ParseAlias(string s) =>
        FromAlias.TryGetValue(s, out var v)
            ? v
            : throw new FormatException($"Unknown {typeof(T).Name} alias: '{s}'");
}
