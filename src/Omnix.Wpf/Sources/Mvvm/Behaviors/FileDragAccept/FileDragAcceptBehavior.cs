using System.Windows;
using System.Windows.Interactivity;

namespace Omnius.Wpf
{
    public sealed class FileDragAcceptBehavior : Behavior<FrameworkElement>
    {
        public FileDragAcceptDescription Description
        {
            get { return (FileDragAcceptDescription)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(FileDragAcceptDescription),
            typeof(FileDragAcceptBehavior), new PropertyMetadata(null));

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

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var source = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (source == null) return;

                desc.OnDrop(new FileDragAcceptEventArgs(source));
            }
            e.Handled = true;
        }
    }
}
