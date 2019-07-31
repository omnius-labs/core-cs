using System.Collections;

namespace Omnix.Base.Internal
{
    internal static class ExtensionHelper
    {
        public static object? GetLockObject(object target)
        {
            object? lockObject = null;
            {
                if (target is ICollection collection && collection.IsSynchronized)
                {
                    lockObject = collection.SyncRoot;
                }
            }

            return lockObject;
        }
    }
}
