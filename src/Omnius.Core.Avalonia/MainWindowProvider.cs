using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Omnius.Core.Avalonia;

public class MainWindowProvider : IMainWindowProvider
{
    public Window GetMainWindow()
    {
        if (Application.Current is null) throw new NotSupportedException();
        var lifetime = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        if (lifetime is null) throw new NotSupportedException();
        return lifetime.MainWindow;
    }
}
