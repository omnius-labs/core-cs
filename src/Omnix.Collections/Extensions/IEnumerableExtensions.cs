using System.Collections.Generic;
using System.Threading;
using Omnix.Collections;
using Omnix.Internal;

namespace Omnix.Extensions
{
    public static class IEnumerableExtensions
    {
        public static LockedList<T> ToLockedList<T>(this IEnumerable<T> list)
            where T : notnull
        {
            object? lockObject = ExtensionHelper.GetLockObject(list);

            bool lockToken = false;

            try
            {
                if (lockObject != null)
                {
                    Monitor.Enter(lockObject, ref lockToken);
                }

                return new LockedList<T>(list);
            }
            finally
            {
                if (lockToken)
                {
                    Monitor.Exit(lockObject!);
                }
            }
        }
    }
}
