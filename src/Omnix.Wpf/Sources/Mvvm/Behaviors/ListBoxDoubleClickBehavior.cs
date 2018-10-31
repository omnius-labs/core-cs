using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Omnius.Wpf;

namespace Omnius.Wpf
{
    public class ListBoxDoubleClickBehavior : Behavior<ListBox>
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ListBoxDoubleClickBehavior), new UIPropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseDoubleClick += this.AssociatedObject_PreviewMouseDoubleClick;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.PreviewMouseDoubleClick -= this.AssociatedObject_PreviewMouseDoubleClick;
            base.OnDetaching();
        }

        private void AssociatedObject_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var element = ((UIElement)e.OriginalSource).FindAncestor<ListBoxItem>();
            if (element == null) return;

            if (this.Command != null)
            {
                if (this.Command.CanExecute(element.DataContext))
                {
                    this.Command.Execute(element.DataContext);
                }
            }
        }
    }
}
