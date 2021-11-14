using System.ComponentModel;
using System.Dynamic;

namespace Omnius.Core.Avalonia;

// http://blog.okazuki.jp/entry/20100702/1278056325
public class DynamicState : DynamicObject, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string name)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public DynamicState() { }

    public Dictionary<string, object?> Properties { get; } = new();

    public T? GetValue<T>(string propertyName)
    {
        return (T?)Properties[propertyName];
    }

    public T? GetValueOrDefault<T>(string propertyName, Func<T> defaultValueFactory)
    {
        if (Properties.TryGetValue(propertyName, out object? value)) return (T?)value;

        return defaultValueFactory();
    }

    public T? GetValueOrDefault<T>(string propertyName)
    {
        if (Properties.TryGetValue(propertyName, out object? value)) return (T?)value;

        return default;
    }

    public void SetValue<T>(string propertyName, T? value)
    {
        Properties[propertyName] = value;
        this.OnPropertyChanged(propertyName);
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (Properties.TryGetValue(binder.Name, out var value))
        {
            result = value;
            return true;
        }
        else
        {
            result = null;
            return false;
        }
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        Properties[binder.Name] = value;
        this.OnPropertyChanged(binder.Name);

        return true;
    }
}
