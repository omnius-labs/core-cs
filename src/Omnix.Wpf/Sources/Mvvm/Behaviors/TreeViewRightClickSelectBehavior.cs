using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Omnius.Wpf
{
    public class TreeViewRightClickSelectBehavior : Behavior<TreeView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseRightButtonDown += this.AssociatedObject_PreviewMouseRightButtonDown;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.PreviewMouseRightButtonDown -= this.AssociatedObject_PreviewMouseRightButtonDown;
            base.OnDetaching();
        }

        private void AssociatedObject_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var treeViewItem = ((UIElement)e.OriginalSource).FindAncestor<TreeViewItem>();
            if (treeViewItem == null) return;

            treeViewItem.IsSelected = true;
        }
    }
}
