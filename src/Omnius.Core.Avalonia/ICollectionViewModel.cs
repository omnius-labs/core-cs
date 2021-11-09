namespace Omnius.Core.Avalonia;

public interface ICollectionViewModel<TViewModel, TModel>
    where TViewModel : ICollectionViewModel<TViewModel, TModel>
{
    void Update(TModel model);
}