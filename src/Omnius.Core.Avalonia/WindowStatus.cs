using Omnius.Core.Helpers;
using Omnius.Core.Utils;

namespace Omnius.Core.Avalonia;

public sealed class WindowStatus
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    public WindowPosition? Position { get; set; }

    public WindowSize? Size { get; set; }

    public WindowState State { get; set; }

    public static WindowStatus Load(string configPath)
    {
        WindowStatus? result = null;

        try
        {
            result = JsonHelper.ReadFile<WindowStatus>(configPath);
        }
        catch (Exception e)
        {
            _logger.Debug(e, "Failed to Load file");
        }

        result ??= new WindowStatus();

        return result;
    }

    public void Save(string configPath)
    {
        DirectoryHelper.CreateDirectory(Path.GetDirectoryName(configPath)!);
        JsonHelper.WriteFile(configPath, this, true);
    }
}

public sealed class WindowPosition
{
    public int X { get; set; }

    public int Y { get; set; }
}

public sealed class WindowSize
{
    public double Width { get; set; }

    public double Height { get; set; }
}

public enum WindowState
{
    Normal = 0,
    Minimized = 1,
    Maximized = 2,
    FullScreen = 3,
}
