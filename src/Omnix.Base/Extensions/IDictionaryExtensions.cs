using System;
using System.Collections.Generic;
using System.Threading;
using Omnix.Base.Internal;

namespace Omnix.Base.Extensions
{
    public static class IDictionaryExtensions
    {
        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (addValueFactory == null) throw new ArgumentNullException(nameof(addValueFactory));
            if (updateValueFactory == null) throw new ArgumentNullException(nameof(updateValueFactory));

            object syncObject = ExtensionHelper.GetLockObject(dictionary);
            bool lockToken = false;

            try
            {
                if (syncObject != null) Monitor.Enter(syncObject, ref lockToken);

                TValue result;

                if (!dictionary.TryGetValue(key, out result))
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
            finally
            {
                if (lockToken) Monitor.Exit(syncObject);
            }
        }

        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (updateValueFactory == null) throw new ArgumentNullException(nameof(updateValueFactory));

            object syncObject = ExtensionHelper.GetLockObject(dictionary);
            bool lockToken = false;

            try
            {
                if (syncObject != null) Monitor.Enter(syncObject, ref lockToken);

                TValue result;

                if (!dictionary.TryGetValue(key, out result))
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
            finally
            {
                if (lockToken) Monitor.Exit(syncObject);
            }
        }

        public static bool TryUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue newValue, Predicate<TValue> match)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (match == null) throw new ArgumentNullException(nameof(match));

            object syncObject = ExtensionHelper.GetLockObject(dictionary);
            bool lockToken = false;

            try
            {
                if (syncObject != null) Monitor.Enter(syncObject, ref lockToken);

                if (dictionary.TryGetValue(key, out var result) && match(result))
                {
                    dictionary[key] = newValue;

                    return true;
                }

                return false;
            }
            finally
            {
                if (lockToken) Monitor.Exit(syncObject);
            }
        }

        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue value)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));

            object syncObject = ExtensionHelper.GetLockObject(dictionary);
            bool lockToken = false;

            try
            {
                if (syncObject != null) Monitor.Enter(syncObject, ref lockToken);

                dictionary.TryGetValue(key, out value);

                return dictionary.Remove(key);
            }
            finally
            {
                if (lockToken) Monitor.Exit(syncObject);
            }
        }

        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));

            object syncObject = ExtensionHelper.GetLockObject(dictionary);
            bool lockToken = false;

            try
            {
                if (syncObject != null) Monitor.Enter(syncObject, ref lockToken);

                int count = dictionary.Count;
                dictionary[key] = value;

                return (count != dictionary.Count);
            }
            finally
            {
                if (lockToken) Monitor.Exit(syncObject);
            }
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            object syncObject = ExtensionHelper.GetLockObject(dictionary);
            bool lockToken = false;

            try
            {
                if (syncObject != null) Monitor.Enter(syncObject, ref lockToken);

                TValue result;
                if (dictionary.TryGetValue(key, out result)) return result;

                var value = valueFactory(key);
                dictionary.Add(key, value);

                return value;
            }
            finally
            {
                if (lockToken) Monitor.Exit(syncObject);
            }
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));

            object syncObject = ExtensionHelper.GetLockObject(dictionary);
            bool lockToken = false;

            try
            {
                if (syncObject != null) Monitor.Enter(syncObject, ref lockToken);

                TValue result;
                if (dictionary.TryGetValue(key, out result)) return result;

                dictionary.Add(key, value);

                return value;
            }
            finally
            {
                if (lockToken) Monitor.Exit(syncObject);
            }
        }
    }
}
