using System.Collections.Generic;

namespace Omnix.Base.Extensions
{
    public static class DeconstructExtensions
    {
        /// <summary>
        /// <see cref="KeyValuePair{T, U}" />を分解します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="pair"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Deconstruct<T, U>(this KeyValuePair<T, U> pair, out T key, out U value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}
