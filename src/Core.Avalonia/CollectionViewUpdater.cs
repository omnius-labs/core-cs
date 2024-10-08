using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Omnius.Core.Base;

namespace Omnius.Core.Avalonia;

public class CollectionViewUpdater<TViewModel, TModel> : AsyncDisposableBase
    where TViewModel : ICollectionViewModel<TViewModel, TModel>, new()
    where TModel : notnull
{
    private readonly IApplicationDispatcher _applicationDispatcher;
    private readonly Func<CancellationToken, ValueTask<IEnumerable<TModel>>> _callback;
    private readonly TimeSpan _refreshSpan;
    private readonly IEqualityComparer<TModel> _equalityComparer;
    private readonly ILogger _logger;

    private readonly ObservableDictionary<TModel, TViewModel> _observableDictionary;

    private readonly Task _refreshTask;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public CollectionViewUpdater(IApplicationDispatcher applicationDispatcher, Func<CancellationToken, ValueTask<IEnumerable<TModel>>> callback, TimeSpan refreshSpan, ILogger<CollectionViewUpdater<TViewModel, TModel>> logger)
        : this(applicationDispatcher, callback, refreshSpan, EqualityComparer<TModel>.Default, logger)
    {
    }

    public CollectionViewUpdater(IApplicationDispatcher applicationDispatcher, Func<CancellationToken, ValueTask<IEnumerable<TModel>>> callback, TimeSpan refreshSpan, IEqualityComparer<TModel> equalityComparer, ILogger<CollectionViewUpdater<TViewModel, TModel>> logger)
    {
        _applicationDispatcher = applicationDispatcher;
        _callback = callback;
        _refreshSpan = refreshSpan;
        _equalityComparer = equalityComparer;
        _logger = logger;

        _observableDictionary = new(_equalityComparer);

        _refreshTask = this.RefreshAsync(_cancellationTokenSource.Token);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        _cancellationTokenSource.Cancel();
        await _refreshTask;
        _cancellationTokenSource.Dispose();
    }

    public ReadOnlyObservableCollection<TViewModel> Collection => _observableDictionary.Values;

    private async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(1).ConfigureAwait(false);

        try
        {
            for (; ; )
            {
                await Task.Delay(_refreshSpan, cancellationToken).ConfigureAwait(false);

                var models = new HashSet<TModel>(await _callback.Invoke(cancellationToken), _equalityComparer);

                await _applicationDispatcher.InvokeAsync(() =>
                {
                    foreach (var model in _observableDictionary.Keys)
                    {
                        if (models.Contains(model)) continue;
                        _observableDictionary.Remove(model);
                    }

                    foreach (var model in models)
                    {
                        if (_observableDictionary.TryGetValue(model, out var viewModel))
                        {
                            viewModel.Update(model);
                            continue;
                        }

                        var newViewModel = new TViewModel();
                        newViewModel.Update(model);

                        _observableDictionary.Add(model, newViewModel);
                    }
                });
            }
        }
        catch (OperationCanceledException e)
        {
            _logger.LogDebug(e, "Operation Canceled");
        }
    }
}
