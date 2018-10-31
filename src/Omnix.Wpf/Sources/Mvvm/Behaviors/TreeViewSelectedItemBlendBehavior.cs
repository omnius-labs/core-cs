using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Omnius.Wpf
{
    // https://blog.magnusmontin.net/2014/01/30/wpf-using-behaviours-to-bind-to-readonly-properties-in-mvvm/
    public sealed class TreeViewSelectedItemBlendBehavior : Behavior<TreeView>
    {
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(TreeViewSelectedItemBlendBehavior), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectedItemChanged += this.AssociatedObject_SelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.SelectedItemChanged -= this.AssociatedObject_SelectedItemChanged;
            base.OnDetaching();
        }

        private void AssociatedObject_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.SelectedItem = e.NewValue;
        }
    }
}
