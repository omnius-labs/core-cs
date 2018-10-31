using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Omnius.Wpf
{
    public class ListBoxUnselectBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseLeftButtonDown += this.AssociatedObject_PreviewMouseLeftButtonDown;
            this.AssociatedObject.PreviewMouseRightButtonDown += this.AssociatedObject_PreviewMouseRightButtonDown;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.PreviewMouseLeftButtonDown -= this.AssociatedObject_PreviewMouseLeftButtonDown;
            this.AssociatedObject.PreviewMouseRightButtonDown -= this.AssociatedObject_PreviewMouseRightButtonDown;
            base.OnDetaching();
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.OriginalSource is ContentControl)
            {
                this.AssociatedObject.UnselectAll();
                this.AssociatedObject.Focus();
            }
        }

        private void AssociatedObject_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.OriginalSource is ContentControl)
            {
                this.AssociatedObject.UnselectAll();
                this.AssociatedObject.Focus();
            }
        }
    }
}
