using System;
using System.Globalization;
using System.Windows.Data;
using Omnius.Base;

namespace Omnius.Wpf
{
    [ValueConversion(typeof(int), typeof(string))]
    public class IntToSizeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int item) return NetworkConverter.DecimalToSizeString(item);
            return NetworkConverter.DecimalToSizeString(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string item) return (int)Math.Min(Math.Max(NetworkConverter.SizeStringToDecimal(item), int.MinValue), int.MaxValue);
            return (int)0;
        }
    }
}
