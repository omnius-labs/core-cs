using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Omnius.Core.Avalonia;

public class MainWindowProvider : IMainWindowProvider
{
    public Window GetMainWindow()
    {
        var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
        return lifetime.MainWindow;
    }
}
