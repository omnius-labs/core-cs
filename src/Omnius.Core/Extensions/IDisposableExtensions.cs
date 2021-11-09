using System;
using System.Collections.Generic;

namespace Omnius.Core;

public static class IDisposableExtensions
{
    public static T ToAdd<T>(this T disposable, in ICollection<IDisposable> list)
        where T : IDisposable
    {
        if (disposable is null) throw new ArgumentNullException(nameof(disposable));
        if (list is null) throw new ArgumentNullException(nameof(list));

        list.Add(disposable);
        return disposable;
    }
}