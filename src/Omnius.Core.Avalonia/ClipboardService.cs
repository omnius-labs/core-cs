using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Omnius.Core.Avalonia
{
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
            await Application.Current.Clipboard.ClearAsync();
        }

        public async Task<string> GetTextAsync()
        {
            return await Application.Current.Clipboard.GetTextAsync();
        }

        public async Task SetTextAsync(string text)
        {
            await Application.Current.Clipboard.SetTextAsync(text);
        }
    }
}
