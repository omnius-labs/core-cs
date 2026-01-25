using System.Text.Json;
using Omnius.Core.Base.Helpers;
using Omnius.Core.Base.Serialization;

namespace Omnius.Core.Avalonia;

public sealed class WindowStatus
{
    public WindowPosition? Position { get; set; }

    public WindowSize? Size { get; set; }

    public WindowState State { get; set; }

    public static WindowStatus? Load(string configPath)
    {
        return JsonHelper.ReadFile<WindowStatus>(configPath);
    }

    public void Save(string configPath)
    {
        DirectoryHelper.CreateDirectory(Path.GetDirectoryName(configPath)!);
        JsonHelper.WriteFile(configPath, this, new JsonSerializerOptions() { WriteIndented = true });
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
