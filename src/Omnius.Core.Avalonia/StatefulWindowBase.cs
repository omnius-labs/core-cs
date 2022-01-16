using Avalonia;
using Avalonia.Controls;

namespace Omnius.Core.Avalonia;

public abstract class StatefulWindowBase<TViewModel> : Window
    where TViewModel : class
{
    public StatefulWindowBase()
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

    public void SetWindowStatus(Models.WindowStatus? status)
    {
        if (status is null) return;

        if (status.Position is not null) this.Position = new PixelPoint(status.Position.X, status.Position.Y);
        if (status.Size is not null) this.ClientSize = new Size(status.Size.Width, status.Size.Height);
        this.WindowState = status.State switch
        {
            Models.WindowState.Normal => WindowState.Normal,
            Models.WindowState.Minimized => WindowState.Minimized,
            Models.WindowState.Maximized => WindowState.Maximized,
            Models.WindowState.FullScreen => WindowState.FullScreen,
            _ => WindowState.Normal,
        };
    }

    public Models.WindowStatus GetWindowStatus()
    {
        var position = new Models.WindowPosition() { X = this.Position.X, Y = this.Position.Y };
        var size = new Models.WindowSize() { Width = this.ClientSize.Width, Height = this.ClientSize.Height };
        var state = this.WindowState switch
        {
            WindowState.Normal => Models.WindowState.Normal,
            WindowState.Minimized => Models.WindowState.Minimized,
            WindowState.Maximized => Models.WindowState.Maximized,
            WindowState.FullScreen => Models.WindowState.FullScreen,
            _ => Models.WindowState.Normal,
        };

        return new Models.WindowStatus()
        {
            Position = position,
            Size = size,
            State = state,
        };
    }
}
