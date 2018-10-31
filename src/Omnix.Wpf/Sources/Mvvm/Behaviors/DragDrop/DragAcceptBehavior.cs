using System.Windows;
using System.Windows.Interactivity;

namespace Omnius.Wpf
{
    // http://b.starwing.net/?p=131
    public sealed class DragAcceptBehavior : Behavior<FrameworkElement>
    {
        public DragAcceptDescription Description
        {
            get { return (DragAcceptDescription)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(DragAcceptDescription),
            typeof(DragAcceptBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            this.AssociatedObject.PreviewDragOver += this.AssociatedObject_DragOver;
            this.AssociatedObject.PreviewDrop += this.AssociatedObject_Drop;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.PreviewDragOver -= this.AssociatedObject_DragOver;
            this.AssociatedObject.PreviewDrop -= this.AssociatedObject_Drop;
            base.OnDetaching();
        }

        void AssociatedObject_DragOver(object sender, DragEventArgs e)
        {
            var desc = this.Description;
            if (desc == null)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            if (e.AllowedEffects.HasFlag(DragDropEffects.Move))
            {
                e.Effects = desc.Effects;
                e.Handled = true;
            }
        }

        void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            var desc = this.Description;
            if (desc == null)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            var destination = this.GetDropDestination((UIElement)e.OriginalSource);
            if (destination == null) return;

            if (e.Data.GetDataPresent(this.Description.Format))
            {
                var source = e.Data.GetData(this.Description.Format) as IDragable;
                if (source == null) return;

                desc.OnDrop(new DragAcceptEventArgs(source, destination));
            }
            e.Handled = true;
        }

        private IDropable GetDropDestination(UIElement originalSource)
        {
            var element = originalSource.FindAncestor<FrameworkElement>((n) => n.DataContext is IDropable);
            if (element == null) return null;

            return element.DataContext as IDropable;
        }
    }
}
