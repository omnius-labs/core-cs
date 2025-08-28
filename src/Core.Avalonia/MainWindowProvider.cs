using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Omnius.Core.Avalonia;

public class MainWindowProvider : IMainWindowProvider
{
    public Window GetMainWindow()
    {
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime ?? throw new NotSupportedException("want to classic desktop style");
        return lifetime.MainWindow ?? throw new NotSupportedException("failed to get MainWindow");
    }
}
