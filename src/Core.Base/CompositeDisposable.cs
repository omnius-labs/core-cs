using System.Collections;

namespace Omnius.Core.Base;

public sealed class CompositeDisposable : DisposableBase, ICollection<IDisposable>, IDisposable
{
    private List<IDisposable> _disposables = new();

    protected override void OnDispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }

    public int Count => _disposables.Count;

    public bool IsReadOnly => false;

    public void Add(IDisposable item) => _disposables.Add(item);

    public void Clear() => _disposables.Clear();

    public bool Contains(IDisposable item) => _disposables.Contains(item);

    public void CopyTo(IDisposable[] array, int arrayIndex) => _disposables.CopyTo(array, arrayIndex);

    public bool Remove(IDisposable item) => _disposables.Remove(item);

    public IEnumerator<IDisposable> GetEnumerator() => _disposables.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _disposables.GetEnumerator();
}
