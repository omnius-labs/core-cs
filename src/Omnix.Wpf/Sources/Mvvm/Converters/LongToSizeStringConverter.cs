using System;
using System.Globalization;
using System.Windows.Data;
using Omnius.Base;

namespace Omnius.Wpf
{
    [ValueConversion(typeof(long), typeof(string))]
    public class LongToSizeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long item) return NetworkConverter.DecimalToSizeString(item);
            return NetworkConverter.DecimalToSizeString(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string item) return (long)Math.Min(Math.Max(NetworkConverter.SizeStringToDecimal(item), long.MinValue), long.MaxValue);
            return (long)0;
        }
    }
}
