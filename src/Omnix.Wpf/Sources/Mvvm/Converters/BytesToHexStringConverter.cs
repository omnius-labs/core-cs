using System;
using System.Globalization;
using System.Windows.Data;
using Omnius.Base;

namespace Omnius.Wpf
{
    [ValueConversion(typeof(byte[]), typeof(string))]
    public class BytesToHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] item) return NetworkConverter.BytesToHexString(item);
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string item) return NetworkConverter.HexStringToBytes(item);
            return Array.Empty<byte>();
        }
    }
}
