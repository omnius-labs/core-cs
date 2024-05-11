using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Core.Avalonia;

public class MainWindowProvider : IMainWindowProvider
{
    public Window GetMainWindow()
    {
        var applicationLifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var mainWindow = applicationLifetime?.MainWindow;
        if (mainWindow is null) throw new NotSupportedException();
        return mainWindow;
    }
}
