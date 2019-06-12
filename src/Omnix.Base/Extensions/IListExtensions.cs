using System;
using System.Collections.Generic;
using System.Threading;
using Omnix.Base.Internal;

namespace Omnix.Base.Extensions
{
    public static class IListExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            object? lockObject = ExtensionHelper.GetLockObject(list);
            bool lockToken = false;

            try
            {
                if (lockObject != null)
                {
                    Monitor.Enter(lockObject, ref lockToken);
                }

                foreach (var item in items)
                {
                    list.Add(item);
                }
            }
            finally
            {
                if (lockToken)
                {
                    Monitor.Exit(lockObject);
                }
            }
        }
    }
}
