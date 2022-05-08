using Avalonia;

namespace Omnius.Core.Avalonia;

public abstract class StatefulWindowBase<TViewModel> : RestorableWindow
    where TViewModel : class
{
    public StatefulWindowBase()
    {
        this.GetObservable(DataContextProperty).Subscribe(this.OnDataContextChanged);
        this.GetObservable(ViewModelProperty).Subscribe(this.OnViewModelChanged);
    }

    public StatefulWindowBase(string configDirectoryPath) : base(configDirectoryPath)
    {
        this.GetObservable(DataContextProperty).Subscribe(this.OnDataContextChanged);
        this.GetObservable(ViewModelProperty).Subscribe(this.OnViewModelChanged);
    }

    public static readonly StyledProperty<TViewModel?> ViewModelProperty =
        AvaloniaProperty.Register<StatefulWindowBase<TViewModel>, TViewModel?>(nameof(ViewModel));

    public TViewModel? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private void OnDataContextChanged(object? v)
    {
        this.ViewModel = v as TViewModel;
    }

    private void OnViewModelChanged(TViewModel? v)
    {
        if (v == null)
        {
            ClearValue(DataContextProperty);
        }
        else if (v != this.DataContext)
        {
            this.DataContext = v;
        }
    }
}
