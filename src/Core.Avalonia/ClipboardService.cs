using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;

namespace Core.Avalonia;

public interface IClipboardService
{
    Task ClearAsync();
    Task<string?> GetTextAsync();
    Task SetTextAsync(string text);
}

public class ClipboardService : IClipboardService
{
    private IClipboard GetClipboard()
    {
        var applicationLifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var clipboard = applicationLifetime?.MainWindow?.Clipboard;
        if (clipboard is null) throw new NotSupportedException();
        return clipboard;
    }

    public async Task ClearAsync()
    {
        await this.GetClipboard().ClearAsync();
    }

    public async Task<string?> GetTextAsync()
    {
        return await this.GetClipboard().GetTextAsync();
    }

    public async Task SetTextAsync(string text)
    {
        await this.GetClipboard().SetTextAsync(text);
    }
}
