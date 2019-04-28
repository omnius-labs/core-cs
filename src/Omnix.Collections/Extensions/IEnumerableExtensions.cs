using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Omnix.Base;
using Omnix.Collections.Internal;

namespace Omnix.Collections.Extensions
{
    public static class IEnumerableExtensions
    {
        public static LockedList<T> ToLockedList<T>(this IEnumerable<T> list)
        {
            object? lockObject = ExtensionHelper.GetLockObject(list);

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
