namespace Omnix.Avalonia.Models.Primitives
{
    public interface IDropable
    {
        bool TryAdd(object value);
        bool TryRemove(object value);
    }
}
