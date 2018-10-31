using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Omnius.Wpf
{
    [ValueConversion(typeof(double), typeof(GridLength))]
    public class DoubleToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double item = double.NaN;
            if (value is double) item = (double)value;

            if (double.IsNaN(item)) return GridLength.Auto;
            else return new GridLength(item);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = GridLength.Auto;
            if (value is GridLength) item = (GridLength)value;

            if (item == GridLength.Auto) return double.NaN;
            else return item.Value;
        }
    }
}
