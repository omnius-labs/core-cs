namespace Omnius.Core.Base;

public static class KeyValuePairExtensions
{
    /// <summary>
    /// <see cref="KeyValuePair{TKey, TValue}" />を分解します。
    /// </summary>
    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
    {
        key = pair.Key;
        value = pair.Value;
    }
}
