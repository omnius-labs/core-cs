using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Internal;

namespace Omnius.Core.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool Contains(this IEnumerable items, object item)
        {
            return items.IndexOf(item) != -1;
        }

        public static int Count(this IEnumerable items)
        {
            if (items != null)
            {
                if (items is ICollection collection)
                {
                    return collection.Count;
                }
                else
                {
                    return Enumerable.Count(items.Cast<object>());
                }
            }
            else
            {
                return 0;
            }
        }

        public static int IndexOf(this IEnumerable items, object item)
        {
            if (items is IList list)
            {
                return list.IndexOf(item);
            }
            else
            {
                int index = 0;

                foreach (var i in items)
                {
                    if (ReferenceEquals(i, item))
                    {
                        return index;
                    }

                    ++index;
                }

                return -1;
            }
        }

        public static object? ElementAt(this IEnumerable items, int index)
        {
            if (items is IList list)
            {
                return list[index];
            }
            else
            {
                return Enumerable.ElementAt(items.Cast<object>(), index);
            }
        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> collection)
        {
            var random = RandomProvider.GetThreadRandom();
            return Randomize(collection, random);
        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> collection, Random random)
        {
            object? lockObject = ExtensionHelper.GetLockObject(collection);
            bool lockToken = false;

            try
            {
                if (lockObject != null)
                {
                    Monitor.Enter(lockObject, ref lockToken);
                }

                var list = new List<T>(collection);
                int n = list.Count;

                while (n > 1)
                {
                    int k = random.Next(n--);
                    var temp = list[n];
                    list[n] = list[k];
                    list[k] = temp;
                }

                return list;
            }
            finally
            {
                if (lockToken)
                {
                    Monitor.Exit(lockObject!);
                }
            }
        }

        // http://neue.cc/2014/03/14_448.html
        public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> action, int concurrency, CancellationToken cancellationToken = default, bool configureAwait = false)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (concurrency <= 0)
            {
                throw new ArgumentOutOfRangeException("concurrency must be greater than 1.");
            }

            using (var semaphore = new SemaphoreSlim(initialCount: concurrency, maxCount: concurrency))
            {
                var exceptionCount = 0;
                var tasks = new List<Task>();

                foreach (var item in source)
                {
                    if (exceptionCount > 0)
                    {
                        break;
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    await semaphore.WaitAsync(cancellationToken).ConfigureAwait(configureAwait);
                    var task = action(item).ContinueWith(t =>
                    {
                        semaphore.Release();

                        if (t.IsFaulted)
                        {
                            Interlocked.Increment(ref exceptionCount);
                            throw t.Exception!;
                        }
                    });
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks.ToArray()).ConfigureAwait(configureAwait);
            }
        }
    }
}
