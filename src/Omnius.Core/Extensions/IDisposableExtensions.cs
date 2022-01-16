namespace Omnius.Core;

public static class IDisposableExtensions
{
    public static T AddTo<T>(this T disposable, ICollection<IDisposable> list)
        where T : IDisposable
    {
        if (disposable is null) throw new ArgumentNullException(nameof(disposable));
        if (list is null) throw new ArgumentNullException(nameof(list));

        list.Add(disposable);
        return disposable;
    }
}
