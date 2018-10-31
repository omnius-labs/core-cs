using System;
using System.Globalization;
using System.Windows.Data;

namespace Omnius.Wpf
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool item)
            {
                return item ? "+" : "-";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string item)
            {
                if (item == "+") return true;
                else if (item == "-") return false;
            }
            return false;
        }
    }
}
