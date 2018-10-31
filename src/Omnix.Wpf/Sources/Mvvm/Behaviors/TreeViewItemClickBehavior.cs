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
    public class TreeViewItemClickBehavior : Behavior<TreeView>
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(TreeViewItemClickBehavior), new UIPropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseDown += this.AssociatedObject_PreviewMouseDown;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.PreviewMouseDown -= this.AssociatedObject_PreviewMouseDown;
            base.OnDetaching();
        }

        private void AssociatedObject_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var element = ((UIElement)e.OriginalSource).FindAncestor<TreeViewItem>();
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
