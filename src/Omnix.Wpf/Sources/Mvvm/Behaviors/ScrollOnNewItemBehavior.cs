using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Omnius.Wpf
{
    // https://stackoverflow.com/questions/2006729/how-can-i-have-a-listbox-auto-scroll-when-a-new-item-is-added
    public class ScrollOnNewItemBehavior : Behavior<ItemsControl>
    {
        protected override void OnAttached()
        {
            this.AssociatedObject.Loaded += this.OnLoaded;
            this.AssociatedObject.Unloaded += this.OnUnLoaded;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.Loaded -= this.OnLoaded;
            this.AssociatedObject.Unloaded -= this.OnUnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var incc = this.AssociatedObject.ItemsSource as INotifyCollectionChanged;
            if (incc == null) return;

            incc.CollectionChanged += this.OnCollectionChanged;
        }

        private void OnUnLoaded(object sender, RoutedEventArgs e)
        {
            var incc = this.AssociatedObject.ItemsSource as INotifyCollectionChanged;
            if (incc == null) return;

            incc.CollectionChanged -= this.OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int count = this.AssociatedObject.Items.Count;
                if (count == 0)
                    return;

                object item = this.AssociatedObject.Items[count - 1];

                var frameworkElement = this.AssociatedObject.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                if (frameworkElement == null) return;

                frameworkElement.BringIntoView();
            }
        }
    }
}
