using System.Collections;
using Omnius.Core.Helpers;

namespace Omnius.Core;

public class CompositeAsyncDisposable : AsyncDisposableBase, ICollection<IAsyncDisposable>, IAsyncDisposable
{
    private List<IAsyncDisposable> _disposables = new();

    protected override async ValueTask OnDisposeAsync()
    {
        var tasks = new List<ValueTask>();

        foreach (var disposable in _disposables)
        {
            tasks.Add(disposable.DisposeAsync());
        }

        await ValueTaskHelper.WhenAll(tasks.ToArray());
    }

    public int Count => _disposables.Count;

    public bool IsReadOnly => false;

    public void Add(IAsyncDisposable item) => _disposables.Add(item);

    public void Clear() => _disposables.Clear();

    public bool Contains(IAsyncDisposable item) => _disposables.Contains(item);

    public void CopyTo(IAsyncDisposable[] array, int arrayIndex) => _disposables.CopyTo(array, arrayIndex);

    public bool Remove(IAsyncDisposable item) => _disposables.Remove(item);

    public IEnumerator<IAsyncDisposable> GetEnumerator() => _disposables.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _disposables.GetEnumerator();
}
