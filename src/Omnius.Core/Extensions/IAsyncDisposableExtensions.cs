namespace Omnius.Core;

public static class IAsyncDisposableExtensions
{

    public static T AddTo<T>(this T disposable, ICollection<IAsyncDisposable> list)
        where T : IAsyncDisposable
    {
        if (disposable is null) throw new ArgumentNullException(nameof(disposable));
        if (list is null) throw new ArgumentNullException(nameof(list));

        list.Add(disposable);
        return disposable;
    }
}
