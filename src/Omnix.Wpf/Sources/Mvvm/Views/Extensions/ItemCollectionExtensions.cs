using System.Collections.Generic;
using System.Windows.Controls;

namespace Omnius.Wpf
{
    public static class ItemCollectionExtensions
    {
        public static void AddRange(this ItemCollection itemCollection, IEnumerable<object> collection)
        {
            foreach (object item in collection)
            {
                itemCollection.Add(item);
            }
        }
    }
}
