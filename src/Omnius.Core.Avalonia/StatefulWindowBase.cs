using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Models = Omnius.Core.Avalonia.Models;

namespace Omnius.Xeus.Ui.Desktop.Windows.Primitives;

public abstract class StatefulWindowBase : Window
{
    private bool _isInitialized = false;
    private bool _isActivated = false;
    private bool _isDisposing = false;
    private bool _isDisposed = false;

    public StatefulWindowBase()
    {
        this.Initialized += (_, _) => this.OnInitialized();
        this.Activated += (_, _) => this.OnActivated();
    }

    private new async void OnInitialized()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        await this.OnInitializeAsync();
    }

    private async void OnActivated()
    {
        if (_isActivated) return;
        _isActivated = true;

        await this.OnActivatedAsync();
    }

    protected override async void OnClosing(CancelEventArgs e)
    {
        if (_isDisposed) return;

        e.Cancel = true;

        if (_isDisposing) return;

        _isDisposing = true;

        await this.OnDisposeAsync();

        _isDisposed = true;

        this.Close();
    }

    protected abstract ValueTask OnInitializeAsync();

    protected abstract ValueTask OnActivatedAsync();

    protected abstract ValueTask OnDisposeAsync();

    public void SetWindowStatus(Models.WindowStatus? status)
    {
        if (status is null) return;

        if (status.Position is not null) this.Position = new PixelPoint(status.Position.X, status.Position.Y);
        if (status.Size is not null) this.ClientSize = new Size(status.Size.Width, status.Size.Height);
        this.WindowState = status.State switch
        {
            Models.WindowState.Normal => Avalonia.Controls.WindowState.Normal,
            Models.WindowState.Minimized => Avalonia.Controls.WindowState.Minimized,
            Models.WindowState.Maximized => Avalonia.Controls.WindowState.Maximized,
            Models.WindowState.FullScreen => Avalonia.Controls.WindowState.FullScreen,
            _ => Avalonia.Controls.WindowState.Normal,
        };
    }

    public Models.WindowStatus GetWindowStatus()
    {
        var position = new Models.WindowPosition() { X = this.Position.X, Y = this.Position.Y };
        var size = new Models.WindowSize() { Width = this.ClientSize.Width, Height = this.ClientSize.Height };
        var state = this.WindowState switch
        {
            Avalonia.Controls.WindowState.Normal => Models.WindowState.Normal,
            Avalonia.Controls.WindowState.Minimized => Models.WindowState.Minimized,
            Avalonia.Controls.WindowState.Maximized => Models.WindowState.Maximized,
            Avalonia.Controls.WindowState.FullScreen => Models.WindowState.FullScreen,
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
