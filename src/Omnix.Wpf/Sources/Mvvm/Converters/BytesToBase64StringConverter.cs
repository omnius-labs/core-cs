using System;
using System.Globalization;
using System.Windows.Data;
using Omnius.Base;

namespace Omnius.Wpf
{
    [ValueConversion(typeof(byte[]), typeof(string))]
    public class BytesToBase64StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] item) return NetworkConverter.ToBase64UrlString(item);
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string item) return NetworkConverter.FromBase64UrlString(item);
            return Array.Empty<byte>();
        }
    }
}
