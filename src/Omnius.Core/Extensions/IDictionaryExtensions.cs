using System;
using System.Collections.Generic;

namespace Omnius.Core.Extensions
{
    public static class IDictionaryExtensions
    {
        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
            where TKey : notnull
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (addValueFactory == null) throw new ArgumentNullException(nameof(addValueFactory));
            if (updateValueFactory == null) throw new ArgumentNullException(nameof(updateValueFactory));

            if (!dictionary.TryGetValue(key, out var result))
            {
                result = addValueFactory(key);
                dictionary.Add(key, result);
            }
            else
            {
                result = updateValueFactory(key, result);
                dictionary[key] = result;
            }

            return result;
        }

        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
            where TKey : notnull
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (updateValueFactory == null) throw new ArgumentNullException(nameof(updateValueFactory));

            if (!dictionary.TryGetValue(key, out var result))
            {
                result = addValue;
                dictionary.Add(key, result);
            }
            else
            {
                result = updateValueFactory(key, result);
                dictionary[key] = result;
            }

            return result;
        }

        public static bool TryUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue newValue, Predicate<TValue> match)
            where TKey : notnull
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (match == null) throw new ArgumentNullException(nameof(match));

            if (dictionary.TryGetValue(key, out var result) && match(result))
            {
                dictionary[key] = newValue;

                return true;
            }

            return false;
        }

        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue? value)
            where TKey : notnull
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));

            dictionary.TryGetValue(key, out value);

            return dictionary.Remove(key);
        }

        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
            where TKey : notnull
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));

            int count = dictionary.Count;
            dictionary[key] = value;

            return (count != dictionary.Count);
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
            where TKey : notnull
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            if (dictionary.TryGetValue(key, out var result)) return result;

            var value = valueFactory(key);
            dictionary.Add(key, value);

            return value;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
            where TKey : notnull
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (dictionary.TryGetValue(key, out var result)) return result;

            dictionary.Add(key, value);

            return value;
        }
    }
}
