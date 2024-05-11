using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Core.Avalonia.Converters;

public class LevelPaddingConverter : IValueConverter
{
    // https://github.com/kekekeks/example-avalonia-huge-tree/blob/c77f1c32721dfa2ef8da1c65c0cce909b3b33eb2/AvaloniaHugeTree/LevelPaddingConverter.cs#L8
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is string unitText && int.TryParse(unitText, out int unit))
        {
            if (value is int level)
            {
                return new Thickness(unit * level, 0, 0, 0);
            }
        }

        return new Thickness();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
