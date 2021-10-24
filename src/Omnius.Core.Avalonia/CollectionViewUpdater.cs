using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Avalonia
{
    public class CollectionViewUpdater<TViewModel, TModel> : AsyncDisposableBase
        where TViewModel : ICollectionViewModel<TViewModel, TModel>, new()
        where TModel : notnull
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IApplicationDispatcher _applicationDispatcher;
        private readonly Func<ValueTask<IEnumerable<TModel>>> _callback;
        private readonly TimeSpan _refreshSpan;

        private readonly ObservableDictionary<TModel, TViewModel> _observableDictionary;

        private readonly Task _refreshTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public CollectionViewUpdater(IApplicationDispatcher applicationDispatcher, Func<ValueTask<IEnumerable<TModel>>> callback, TimeSpan refreshSpan)
            : this(applicationDispatcher, callback, refreshSpan, EqualityComparer<TModel>.Default)
        {
        }

        public CollectionViewUpdater(IApplicationDispatcher applicationDispatcher, Func<ValueTask<IEnumerable<TModel>>> callback, TimeSpan refreshSpan, IEqualityComparer<TModel> equalityComparer)
        {
            _applicationDispatcher = applicationDispatcher;
            _callback = callback;
            _refreshSpan = refreshSpan;

            _observableDictionary = new(equalityComparer);

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
            try
            {
                for (; ; )
                {
                    await Task.Delay(_refreshSpan, cancellationToken);

                    var models = await _callback.Invoke();

                    await _applicationDispatcher.InvokeAsync(() =>
                    {
                        foreach (var model in models)
                        {
                            if (_observableDictionary.ContainsKey(model)) continue;
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
                _logger.Debug(e);
            }
        }
    }
}
