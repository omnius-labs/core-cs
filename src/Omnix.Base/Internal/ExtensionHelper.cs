using System.Collections;

namespace Omnix.Base.Internal
{
    internal static class ExtensionHelper
    {
        public static object GetLockObject(object target)
        {
            object syncObject = null;
            {
                if (target is ICollection collection && collection.IsSynchronized)
                {
                    syncObject = collection.SyncRoot;
                }
            }

            return syncObject;
        }
    }
}
