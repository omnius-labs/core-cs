using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Omnius.Wpf
{
    // http://stackoverflow.com/questions/1585462/bubbling-scroll-events-from-a-listview-to-its-parent
    public sealed class IgnoreMouseWheelBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseWheel += this.AssociatedObject_PreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.PreviewMouseWheel -= this.AssociatedObject_PreviewMouseWheel;
            base.OnDetaching();
        }

        void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

            e.Handled = true;

            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            e2.RoutedEvent = UIElement.MouseWheelEvent;
            this.AssociatedObject.RaiseEvent(e2);

        }
    }
}
