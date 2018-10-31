using System;
using System.Windows;
using System.Windows.Media;

namespace Omnius.Wpf
{
    public static class UIElementExtensions
    {
        public static TAncestor FindAncestor<TAncestor>(this UIElement element)
            where TAncestor : UIElement
        {
            var temp = element;

            while ((temp != null) && !(temp is TAncestor))
            {
                temp = VisualTreeHelper.GetParent(temp) as UIElement;
            }

            return temp as TAncestor;
        }

        public static TAncestor FindAncestor<TAncestor>(this UIElement element, Predicate<TAncestor> match)
            where TAncestor : UIElement
        {
            var temp = element;

            while ((temp != null) && (!(temp is TAncestor) || !match.Invoke((TAncestor)temp)))
            {
                temp = VisualTreeHelper.GetParent(temp) as UIElement;
            }

            return temp as TAncestor;
        }
    }
}
