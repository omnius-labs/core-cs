using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Omnix.Base;

namespace Omnix.Collections.Extensions
{
    public static class IEnumerableExtensions
    {
        public static LockedList<T> ToLockedList<T>(this IEnumerable<T> list)
        {
            object lockObject = null;
            {
                if (list is ICollection collection && collection.IsSynchronized)
                {
                    lockObject = collection.SyncRoot;
                }

                if (lockObject == null && list is ISynchronized synchronized)
                {
                    lockObject = synchronized.LockObject;
                }
            }

            bool lockToken = false;

            try
            {
                Monitor.Enter(lockObject, ref lockToken);

                return new LockedList<T>(list);
            }
            finally
            {
                if (lockToken) Monitor.Exit(lockObject);
            }
        }
    }
}
