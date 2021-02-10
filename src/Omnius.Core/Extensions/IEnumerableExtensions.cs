using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

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
                    if (ReferenceEquals(i, item)) return index;

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

        // http://neue.cc/2014/03/14_448.html
        public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> action, int concurrency, CancellationToken cancellationToken = default, bool configureAwait = false)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");
            if (concurrency <= 0) throw new ArgumentOutOfRangeException("concurrency must be greater than 1.");

            using (var semaphore = new SemaphoreSlim(initialCount: concurrency, maxCount: concurrency))
            {
                var exceptionCount = 0;
                var tasks = new List<Task>();

                foreach (var item in source)
                {
                    if (exceptionCount > 0) break;

                    cancellationToken.ThrowIfCancellationRequested();

                    await semaphore.WaitAsync(cancellationToken).ConfigureAwait(configureAwait);
                    var task = action(item).ContinueWith(t =>
                    {
                        try
                        {
                            semaphore.Release();
                        }
                        catch (ObjectDisposedException)
                        {
                            // taskがキャンセルされていた場合、semaphoreがDisposedの可能性がある。
                        }

                        if (t.IsFaulted)
                        {
                            Interlocked.Increment(ref exceptionCount);
                            ExceptionDispatchInfo.Throw(t.Exception?.InnerException!);
                        }
                    });
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks.ToArray()).ConfigureAwait(configureAwait);
            }
        }

        private static readonly Lazy<Random> _random = new(() => new Random());

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> items)
        {
            var array = items.ToList();
            _random.Value.Shuffle(array);
            return array;
        }
    }
}
