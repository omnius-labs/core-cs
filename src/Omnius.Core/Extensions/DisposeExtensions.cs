
namespace Omnius.Core;

public static class IDisposeExtensions
{
    public static void Dispose<T>(this IEnumerable<T> disposables)
    {
        if (disposables == null) throw new ArgumentNullException(nameof(disposables));

        foreach (var disposable in disposables.OfType<IDisposable>())
        {
            disposable?.Dispose();
        }
    }

    public static async ValueTask DisposeAsync<T>(this IEnumerable<T> disposables)
    {
        if (disposables == null) throw new ArgumentNullException(nameof(disposables));

        foreach (var disposable in disposables.OfType<IAsyncDisposable>())
        {
            if (disposable != null) await disposable.DisposeAsync();
        }
    }
}
