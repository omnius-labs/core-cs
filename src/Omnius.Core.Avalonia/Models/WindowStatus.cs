namespace Omnius.Core.Avalonia.Models;

public sealed class WindowStatus
{
    public WindowPosition? Position { get; set; }
    public WindowSize? Size { get; set; }
    public WindowState State { get; set; }
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
    FullScreen = 3
}
