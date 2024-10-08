namespace Omnius.Core.Base;

public static class StringExtensions
{
    public static bool Contains(this string target, string value, StringComparison comparisonType)
    {
        return target.IndexOf(value, comparisonType) != -1;
    }
}
