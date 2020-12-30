using System.Collections.Generic;

namespace Omnius.Core.Extensions
{
    public static class DeconstructExtensions
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
}
