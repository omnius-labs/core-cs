using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace Omnius.Wpf
{
    public class EnumerateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new List<object> { value };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is List<object> list)
            {
                return list[0];
            }

            return null;
        }
    }
}
