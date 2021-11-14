namespace Omnius.Core.Avalonia;

public interface IDropable
{
    bool TryAdd(object value);

    bool TryRemove(object value);
}
