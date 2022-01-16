using Avalonia;
using Avalonia.Controls;

namespace Omnius.Core.Avalonia;

public abstract class StatefulUserControl<TViewModel> : UserControl
    where TViewModel : class
{
    public StatefulUserControl()
    {
        this.GetObservable(DataContextProperty).Subscribe(this.OnDataContextChanged);
        this.GetObservable(ViewModelProperty).Subscribe(this.OnViewModelChanged);
    }

    public static readonly StyledProperty<TViewModel?> ViewModelProperty =
        AvaloniaProperty.Register<StatefulUserControl<TViewModel>, TViewModel?>(nameof(ViewModel));

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
