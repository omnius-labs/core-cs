using System.Windows;
using System.Windows.Interactivity;

namespace Omnius.Wpf
{
    public sealed class IgnoreRequestBringIntoViewBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.RequestBringIntoView += this.AssociatedObject_RequestBringIntoView;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.RequestBringIntoView -= this.AssociatedObject_RequestBringIntoView;
            base.OnDetaching();
        }

        private void AssociatedObject_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }
}
