using System;
using System.Globalization;
using System.Windows.Data;

namespace Omnius.Wpf
{
    [ValueConversion(typeof(ushort), typeof(string))]
    public class UshortToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ushort result) return result.ToString();
            else return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ushort.TryParse(value as string, out ushort result))
            {
                return result;
            }

            return 0;
        }
    }
}
