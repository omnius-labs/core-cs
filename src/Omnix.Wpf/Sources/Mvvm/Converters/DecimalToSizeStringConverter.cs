using System;
using System.Globalization;
using System.Windows.Data;
using Omnius.Base;

namespace Omnius.Wpf
{
    [ValueConversion(typeof(decimal), typeof(string))]
    public class DecimalToSizeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal item) return NetworkConverter.DecimalToSizeString(item);
            return NetworkConverter.DecimalToSizeString(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string item) return (decimal)Math.Min(Math.Max(NetworkConverter.SizeStringToDecimal(item), decimal.MinValue), decimal.MaxValue);
            return (decimal)0;
        }
    }
}
