using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Omnius.Wpf
{
    // http://pro.art55.jp/?eid=1160884
    public static class ItemsControlExtensions
    {
        public static void GoBottom(this ItemsControl itemsControl)
        {
            try
            {
                var panel = itemsControl.FindItemsHostPanel() as IScrollInfo;
                if (panel == null) return;

                panel.SetVerticalOffset(double.PositiveInfinity);
            }
            catch (Exception)
            {

            }
        }

        public static void GoTop(this ItemsControl itemsControl)
        {
            try
            {
                var panel = itemsControl.FindItemsHostPanel() as IScrollInfo;
                if (panel == null) return;

                panel.SetVerticalOffset(0);
            }
            catch (Exception)
            {

            }
        }

        public static void GoRight(this ItemsControl itemsControl)
        {
            try
            {
                var panel = itemsControl.FindItemsHostPanel() as IScrollInfo;
                if (panel == null) return;

                panel.SetHorizontalOffset(double.PositiveInfinity);
            }
            catch (Exception)
            {

            }
        }

        public static void GoLeft(this ItemsControl itemsControl)
        {
            try
            {
                var panel = itemsControl.FindItemsHostPanel() as IScrollInfo;
                if (panel == null) return;

                panel.SetHorizontalOffset(0);
            }
            catch (Exception)
            {

            }
        }

        public static void PageDown(this ItemsControl itemsControl)
        {
            try
            {
                var panel = itemsControl.FindItemsHostPanel() as IScrollInfo;
                if (panel == null) return;

                panel.PageDown();
            }
            catch (Exception)
            {

            }
        }

        public static void PageUp(this ItemsControl itemsControl)
        {
            try
            {
                var panel = itemsControl.FindItemsHostPanel() as IScrollInfo;
                if (panel == null) return;

                panel.PageUp();
            }
            catch (Exception)
            {

            }
        }

        public static void PageRight(this ItemsControl itemsControl)
        {
            try
            {
                var panel = itemsControl.FindItemsHostPanel() as IScrollInfo;
                if (panel == null) return;

                panel.PageRight();
            }
            catch (Exception)
            {

            }
        }

        public static void PageLeft(this ItemsControl itemsControl)
        {
            try
            {
                var panel = itemsControl.FindItemsHostPanel() as IScrollInfo;
                if (panel == null) return;

                panel.PageLeft();
            }
            catch (Exception)
            {

            }
        }

        public static void LineDown(this ItemsControl itemsControl)
        {
            try
            {
                var panel = itemsControl.FindItemsHostPanel() as IScrollInfo;
                if (panel == null) return;

                panel.LineDown();
            }
            catch (Exception)
            {

            }
        }

        public static void LineUp(this ItemsControl itemsControl)
        {
            try
            {
                var panel = itemsControl.FindItemsHostPanel() as IScrollInfo;
                if (panel == null) return;

                panel.LineUp();
            }
            catch (Exception)
            {

            }
        }

        public static void LineRight(this ItemsControl itemsControl)
        {
            try
            {
                var panel = itemsControl.FindItemsHostPanel() as IScrollInfo;
                if (panel == null) return;

                panel.LineRight();
            }
            catch (Exception)
            {

            }
        }

        public static void LineLeft(this ItemsControl itemsControl)
        {
            try
            {
                var panel = itemsControl.FindItemsHostPanel() as IScrollInfo;
                if (panel == null) return;

                panel.LineLeft();
            }
            catch (Exception)
            {

            }
        }

        private static Panel FindItemsHostPanel(this ItemsControl itemsControl)
        {
            return Find(itemsControl.ItemContainerGenerator, itemsControl);
        }

        private static Panel Find(this IItemContainerGenerator generator, DependencyObject control)
        {
            int count = VisualTreeHelper.GetChildrenCount(control);

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(control, i);

                if (IsItemsHostPanel(generator, child))
                {
                    return (Panel)child;
                }

                var panel = Find(generator, child);

                if (panel != null)
                {
                    return panel;
                }
            }

            return null;
        }

        private static bool IsItemsHostPanel(IItemContainerGenerator generator, DependencyObject target)
        {
            var panel = target as Panel;
            return panel != null && panel.IsItemsHost && generator == generator.GetItemContainerGeneratorForPanel(panel);
        }
    }
}
