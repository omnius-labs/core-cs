using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;

namespace Omnius.Core.Avalonia;

public interface IClipboardService
{
    Task ClearAsync();

    Task<string> GetTextAsync();

    Task SetTextAsync(string text);
}

public class ClipboardService : IClipboardService
{
    private IClipboard? GetClipboard()
    {
        var applicationLifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        return applicationLifetime?.MainWindow?.Clipboard;
    }

    private void ThrowIfNotSupported()
    {
        if (this.GetClipboard() is null) throw new NotSupportedException();
    }

    public async Task ClearAsync()
    {
        this.ThrowIfNotSupported();
        await this.GetClipboard()!.ClearAsync();
    }

    public async Task<string> GetTextAsync()
    {
        this.ThrowIfNotSupported();
        return await this.GetClipboard()!.GetTextAsync();
    }

    public async Task SetTextAsync(string text)
    {
        this.ThrowIfNotSupported();
        await this.GetClipboard()!.SetTextAsync(text);
    }
}
