using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Omnius.Wpf
{
    // http://stackoverflow.com/questions/6446699/how-do-you-bind-a-collectioncontainer-to-a-collection-in-a-view-model
    public class CompositeCollectionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var compositeCollection = new CompositeCollection();

            foreach (object value in values)
            {
                if (value is IEnumerable enumerable)
                {
                    var binding = new Binding { Source = enumerable };
                    var collectionContainer = new CollectionContainer();
                    BindingOperations.SetBinding(collectionContainer, CollectionContainer.CollectionProperty, binding);

                    compositeCollection.Add(collectionContainer);
                }
                else
                {
                    compositeCollection.Add(value);
                }
            }

            return compositeCollection;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
