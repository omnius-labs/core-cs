using Avalonia;

namespace Omnius.Core.Avalonia;

public interface IClipboardService
{
    Task ClearAsync();

    Task<string> GetTextAsync();

    Task SetTextAsync(string text);
}

public class ClipboardService : IClipboardService
{
    public async Task ClearAsync()
    {
        if (Application.Current?.Clipboard is null) throw new NotSupportedException();
        await Application.Current.Clipboard.ClearAsync();
    }

    public async Task<string> GetTextAsync()
    {
        if (Application.Current?.Clipboard is null) throw new NotSupportedException();
        return await Application.Current.Clipboard.GetTextAsync();
    }

    public async Task SetTextAsync(string text)
    {
        if (Application.Current?.Clipboard is null) throw new NotSupportedException();
        await Application.Current.Clipboard.SetTextAsync(text);
    }
}
