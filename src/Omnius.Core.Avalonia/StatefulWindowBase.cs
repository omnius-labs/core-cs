using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Omnius.Core.Helpers;

namespace Omnius.Core.Avalonia;

public abstract class StatefulWindowBase<TViewModel> : Window
    where TViewModel : class
{
    private readonly string? _configDirectoryPath;

    private PixelPoint _positionBeforeResizing;
    private Size _clientSizeBeforeResizing;

    private const string WindowStatesFileName = "window_status.json";

    public StatefulWindowBase()
    {
        this.GetObservable(DataContextProperty).Subscribe(this.OnDataContextChanged);
        this.GetObservable(ViewModelProperty).Subscribe(this.OnViewModelChanged);

        this.PositionChanged += (sender, e) => this.OnPositionChanged(e.Point);
        this.GetObservable(ClientSizeProperty).Subscribe(this.OnClientSizeChanged);
    }

    public StatefulWindowBase(string configDirectoryPath) : this()
    {
        _configDirectoryPath = configDirectoryPath;

        this.LoadWindowStatus();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        this.SaveWindowStatus();

        base.OnClosing(e);
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

    private void LoadWindowStatus()
    {
        if (_configDirectoryPath is not null)
        {
            DirectoryHelper.CreateDirectory(_configDirectoryPath!);

            var filePath = Path.Combine(_configDirectoryPath!, WindowStatesFileName);

            if (File.Exists(filePath))
            {
                var windowStatus = Models.WindowStatus.Load(filePath);
                this.SetWindowStatus(windowStatus);
            }
        }
    }

    private void SaveWindowStatus()
    {
        if (_configDirectoryPath is not null)
        {
            var filePath = Path.Combine(_configDirectoryPath, WindowStatesFileName);
            var status = this.GetWindowStatus();
            status.Save(filePath);
        }
    }

    private void OnPositionChanged(PixelPoint position)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (this.WindowState == WindowState.Normal)
            {
                _positionBeforeResizing = position;
            }
        }, DispatcherPriority.Background);
    }

    private void OnClientSizeChanged(Size size)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (this.WindowState == WindowState.Normal)
            {
                _clientSizeBeforeResizing = size;
            }
        }, DispatcherPriority.Background);
    }

    private Models.WindowStatus GetWindowStatus()
    {
        var position = new Models.WindowPosition() { X = _positionBeforeResizing.X, Y = _positionBeforeResizing.Y };
        var size = new Models.WindowSize() { Width = _clientSizeBeforeResizing.Width, Height = _clientSizeBeforeResizing.Height };
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

    private void SetWindowStatus(Models.WindowStatus? status)
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
}
