using System.Collections.Generic;

namespace Omnius.Core.Extensions
{
    public static class DeconstructExtensions
    {
        /// <summary>
        /// <see cref="KeyValuePair{T, U}" />を分解します。
        /// </summary>
        public static void Deconstruct<T, U>(this KeyValuePair<T, U> pair, out T key, out U value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}
