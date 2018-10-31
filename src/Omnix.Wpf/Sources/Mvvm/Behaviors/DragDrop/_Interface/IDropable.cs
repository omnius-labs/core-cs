namespace Omnius.Wpf
{
    public interface IDropable
    {
        bool TryAdd(object value);
        bool TryRemove(object value);
    }
}
