using System.Collections;

namespace Omnius.Core.Base.Collections.Internal;

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
