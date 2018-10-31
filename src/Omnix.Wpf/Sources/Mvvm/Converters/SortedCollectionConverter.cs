using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Omnius.Wpf
{
    public class SortedCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var viewSource = new CollectionViewSource();
            viewSource.Source = value;
            viewSource.IsLiveSortingRequested = true;
            viewSource.LiveSortingProperties.Add((string)parameter);

            var sort = new SortDescription((string)parameter, ListSortDirection.Ascending);
            viewSource.SortDescriptions.Add(sort);

            return viewSource.View;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
