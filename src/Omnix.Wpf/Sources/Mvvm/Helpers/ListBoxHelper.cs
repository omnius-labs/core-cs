using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Omnius.Wpf
{
    //http://stackoverflow.com/questions/31176949/binding-selecteditems-of-listview-to-viewmodel
    public class ListBoxHelper
    {
        private static Dictionary<ListBox, SelectedItemsBinder> _elementToBinder = new Dictionary<ListBox, SelectedItemsBinder>();

        public static IList GetSelectedItems(DependencyObject obj)
        {
            return (IList)obj.GetValue(SelectedItemsProperty);
        }

        public static void SetSelectedItems(DependencyObject obj, IList value)
        {
            obj.SetValue(SelectedItemsProperty, value);
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached("SelectedItems", typeof(IList), typeof(ListBoxHelper), new UIPropertyMetadata(SelectedItemsChanged));

        private static void SelectedItemsChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            if (target is ListBox listBox)
            {
                if (_elementToBinder.TryGetValue(listBox, out var oldBinder))
                {
                    oldBinder.Unbind();
                }

                var newBinder = new SelectedItemsBinder(listBox, args.NewValue as IList);
                newBinder.Bind();
                _elementToBinder[listBox] = newBinder;
            }
        }

        public class SelectedItemsBinder
        {
            private ListBox _listBox;
            private IList _collection;

            public SelectedItemsBinder(ListBox listBox, IList collection)
            {
                _listBox = listBox;
                _collection = collection;

                _listBox.SelectedItems.Clear();

                foreach (object item in _collection)
                {
                    _listBox.SelectedItems.Add(item);
                }
            }

            public void Bind()
            {
                _listBox.SelectionChanged += this.ListBox_SelectionChanged;

                if (_collection is INotifyCollectionChanged observable)
                {
                    observable.CollectionChanged += this.Collection_CollectionChanged;
                }
            }

            public void Unbind()
            {
                _listBox.SelectionChanged -= this.ListBox_SelectionChanged;

                if (_collection is INotifyCollectionChanged observable)
                {
                    observable.CollectionChanged -= this.Collection_CollectionChanged;
                }
            }

            private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                foreach (object item in e.NewItems ?? Array.Empty<object>())
                {
                    if (!_listBox.SelectedItems.Contains(item))
                        _listBox.SelectedItems.Add(item);
                }
                foreach (object item in e.OldItems ?? Array.Empty<object>())
                {
                    _listBox.SelectedItems.Remove(item);
                }
            }

            private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                foreach (object item in e.AddedItems ?? Array.Empty<object>())
                {
                    if (!_collection.Contains(item))
                        _collection.Add(item);
                }

                foreach (object item in e.RemovedItems ?? Array.Empty<object>())
                {
                    _collection.Remove(item);
                }
            }
        }
    }
}
