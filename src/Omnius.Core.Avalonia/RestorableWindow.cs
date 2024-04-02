using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Omnius.Core.Helpers;

namespace Omnius.Core.Avalonia;

public class RestorableWindow : Window
{
    private readonly string? _configDirectoryPath;

    private PixelPoint _positionBeforeResizing;
    private Size _clientSizeBeforeResizing;

    private const string WindowStatesFileName = "window_status.json";

    public RestorableWindow()
    {
        this.Closing += (sender, e) => this.SaveWindowStatus();
        this.PositionChanged += (sender, e) => this.OnPositionChanged(e.Point);
        this.GetObservable(ClientSizeProperty).Subscribe(this.OnClientSizeChanged);
    }

    public RestorableWindow(string configDirectoryPath) : this()
    {
        _configDirectoryPath = configDirectoryPath;

        this.LoadWindowStatus();
    }

    private void LoadWindowStatus()
    {
        if (_configDirectoryPath is not null)
        {
            DirectoryHelper.CreateDirectory(_configDirectoryPath!);

            var filePath = Path.Combine(_configDirectoryPath!, WindowStatesFileName);

            if (File.Exists(filePath))
            {
                var windowStatus = WindowStatus.Load(filePath);
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
            if (this.WindowState == global::Avalonia.Controls.WindowState.Normal)
            {
                _positionBeforeResizing = this.Position;
            }
        }, DispatcherPriority.Background);
    }

    private void OnClientSizeChanged(Size size)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (this.WindowState == global::Avalonia.Controls.WindowState.Normal)
            {
                _clientSizeBeforeResizing = this.ClientSize;
            }
        }, DispatcherPriority.Background);
    }

    private WindowStatus GetWindowStatus()
    {
        var position = new WindowPosition() { X = _positionBeforeResizing.X, Y = _positionBeforeResizing.Y };
        var size = new WindowSize() { Width = _clientSizeBeforeResizing.Width, Height = _clientSizeBeforeResizing.Height };
        var state = this.WindowState switch
        {
            global::Avalonia.Controls.WindowState.Normal => Avalonia.WindowState.Normal,
            global::Avalonia.Controls.WindowState.Minimized => Avalonia.WindowState.Minimized,
            global::Avalonia.Controls.WindowState.Maximized => Avalonia.WindowState.Maximized,
            global::Avalonia.Controls.WindowState.FullScreen => Avalonia.WindowState.FullScreen,
            _ => Avalonia.WindowState.Normal,
        };

        return new WindowStatus()
        {
            Position = position,
            Size = size,
            State = state,
        };
    }

    private void SetWindowStatus(WindowStatus? status)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (status is null) return;

            if (status.Position is not null) this.Position = new PixelPoint(status.Position.X, status.Position.Y);
            if (status.Size is not null) this.ClientSize = new Size(status.Size.Width, status.Size.Height);
            this.WindowState = status.State switch
            {
                Avalonia.WindowState.Normal => global::Avalonia.Controls.WindowState.Normal,
                Avalonia.WindowState.Minimized => global::Avalonia.Controls.WindowState.Minimized,
                Avalonia.WindowState.Maximized => global::Avalonia.Controls.WindowState.Maximized,
                Avalonia.WindowState.FullScreen => global::Avalonia.Controls.WindowState.FullScreen,
                _ => global::Avalonia.Controls.WindowState.Normal,
            };
        }, DispatcherPriority.Background);
    }
}
