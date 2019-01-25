namespace Omnix.Avalonia.ViewModels
{
    public interface IDropable
    {
        bool TryAdd(object value);
        bool TryRemove(object value);
    }
}
