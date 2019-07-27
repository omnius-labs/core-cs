// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Utils;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Utilities;
using Avalonia.VisualTree;
using Omnix.Avalonia.Controls.Primitives;
using Omnix.Base.Extensions;

namespace Omnix.Avalonia.Controls.Presenters
{
    /// <summary>
    /// Handles virtualization in an <see cref="ItemsPresenter"/> for
    /// <see cref="ItemVirtualizationMode.Simple"/>.
    /// </summary>
    public class CustomItemVirtualizer : CustomItemVirtualizerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomItemVirtualizer"/> class.
        /// </summary>
        /// <param name="owner"></param>
        public CustomItemVirtualizer(CustomItemsPresenter owner)
            : base(owner)
        {
            // Don't need to add children here as UpdateControls should be called by the panel
            // measure/arrange.
        }

        public event EventHandler ChildrenChanged;

        private int ScrollQuantum => (this.VirtualizingPanel as IScrollQuantum)?.ScrollQuantum ?? 1;

        /// <inheritdoc/>
        public override bool IsLogicalScrollEnabled => true;

        /// <inheritdoc/>
        public override double ExtentValue => Math.Ceiling((double)this.ItemCount / this.ScrollQuantum);

        /// <inheritdoc/>
        public override double OffsetValue
        {
            get
            {
                var offset = this.VirtualizingPanel.PixelOffset > 0 ? 1 : 0;
                return this.FirstIndex / this.ScrollQuantum + offset;
            }

            set
            {
                var panel = this.VirtualizingPanel;
                var offset = panel.PixelOffset > 0 ? 1 : 0;
                var current = this.FirstIndex / this.ScrollQuantum + offset;
                var delta = ((int)value - current);

                delta *= this.ScrollQuantum;

                if (delta != 0)
                {
                    var newLastIndex = (this.NextIndex - 1) + delta;

                    if (newLastIndex < this.ItemCount)
                    {
                        if (panel.PixelOffset > 0)
                        {
                            panel.PixelOffset = 0;
                            delta += this.ScrollQuantum;
                        }

                        if (delta != 0)
                        {
                            this.RecycleContainersForMove(delta);
                        }
                    }
                    else
                    {
                        // We're moving to a partially obscured item at the end of the list so
                        // offset the panel by the height of the first item.
                        var firstIndex = this.ItemCount - panel.Children.Count;
                        firstIndex = (int)Math.Ceiling(firstIndex / (double)this.ScrollQuantum) * this.ScrollQuantum;
                        this.RecycleContainersForMove(firstIndex - this.FirstIndex);

                        double pixelOffset;
                        var child = panel.Children[0];

                        if (child.IsArrangeValid)
                        {
                            pixelOffset = this.VirtualizingPanel.ScrollDirection == Orientation.Vertical ?
                                                    child.Bounds.Height :
                                                    child.Bounds.Width;
                        }
                        else
                        {
                            pixelOffset = this.VirtualizingPanel.ScrollDirection == Orientation.Vertical ?
                                                    child.DesiredSize.Height :
                                                    child.DesiredSize.Width;
                        }

                        panel.PixelOffset = pixelOffset;
                    }

                    this.ChildrenChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <inheritdoc/>
        public override double ViewportValue
        {
            get
            {
                // If we can't fit the last items in the panel fully, subtract 1 line from the viewport.
                var overflow = this.VirtualizingPanel.PixelOverflow > 0 ? 1 : 0;
                return Math.Ceiling((double)this.VirtualizingPanel.Children.Count / this.ScrollQuantum) - overflow;
            }
        }

        /// <inheritdoc/>
        public override Size MeasureOverride(Size availableSize)
        {
            var scrollable = (ILogicalScrollable)this.Owner;
            var visualRoot = this.Owner.GetVisualRoot();
            var maxAvailableSize = (visualRoot as WindowBase)?.PlatformImpl?.MaxClientSize
                 ?? (visualRoot as TopLevel)?.ClientSize;

            // If infinity is passed as the available size and we're virtualized then we need to
            // fill the available space, but to do that we *don't* want to materialize all our
            // items! Take a look at the root of the tree for a MaxClientSize and use that as
            // the available size.
            if (this.VirtualizingPanel.ScrollDirection == Orientation.Vertical)
            {
                if (availableSize.Height == double.PositiveInfinity)
                {
                    if (maxAvailableSize.HasValue)
                    {
                        availableSize = availableSize.WithHeight(maxAvailableSize.Value.Height);
                    }
                }

                if (scrollable.CanHorizontallyScroll)
                {
                    availableSize = availableSize.WithWidth(double.PositiveInfinity);
                }
            }
            else
            {
                if (availableSize.Width == double.PositiveInfinity)
                {
                    if (maxAvailableSize.HasValue)
                    {
                        availableSize = availableSize.WithWidth(maxAvailableSize.Value.Width);
                    }
                }

                if (scrollable.CanVerticallyScroll)
                {
                    availableSize = availableSize.WithHeight(double.PositiveInfinity);
                }
            }

            this.Owner.Panel.Measure(availableSize);
            return this.Owner.Panel.DesiredSize;
        }

        /// <inheritdoc/>
        public override void UpdateControls()
        {
            this.CreateAndRemoveContainers();
            this.InvalidateScroll();

            this.ChildrenChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        public override void ItemsChanged(IEnumerable items, NotifyCollectionChangedEventArgs e)
        {
            base.ItemsChanged(items, e);

            var panel = this.VirtualizingPanel;

            if (items != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        this.CreateAndRemoveContainers();

                        if (e.NewStartingIndex < this.NextIndex)
                        {
                            this.RecycleContainers();
                        }

                        panel.ForceInvalidateMeasure();
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldStartingIndex >= this.FirstIndex &&
                            e.OldStartingIndex < this.NextIndex)
                        {
                            this.RecycleContainersOnRemove();
                        }

                        panel.ForceInvalidateMeasure();
                        break;

                    case NotifyCollectionChangedAction.Move:
                    case NotifyCollectionChangedAction.Replace:
                        this.RecycleContainers();
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        this.RecycleContainersOnRemove();
                        this.CreateAndRemoveContainers();
                        panel.ForceInvalidateMeasure();
                        break;
                }
            }
            else
            {
                this.Owner.ItemContainerGenerator.Clear();
                this.VirtualizingPanel.Children.Clear();
                this.FirstIndex = this.NextIndex = 0;
            }

            // If we are scrolled to view a partially visible last item but controls were added
            // then we need to return to a non-offset scroll position.
            if (panel.PixelOffset != 0 && this.FirstIndex + panel.Children.Count < this.ItemCount)
            {
                panel.PixelOffset = 0;
                this.RecycleContainersForMove(this.ScrollQuantum);
            }

            this.InvalidateScroll();
        }

        public override IControl GetControlInDirection(NavigationDirection direction, IControl from)
        {
            var generator = this.Owner.ItemContainerGenerator;
            var panel = this.VirtualizingPanel;
            var itemIndex = generator.IndexFromContainer(from);
            var vertical = this.VirtualizingPanel.ScrollDirection == Orientation.Vertical;

            if (itemIndex == -1)
            {
                return null;
            }

            var newItemIndex = -1;

            switch (direction)
            {
                case NavigationDirection.First:
                    newItemIndex = 0;
                    break;

                case NavigationDirection.Last:
                    newItemIndex = this.ItemCount - 1;
                    break;

                case NavigationDirection.Up:
                    if (vertical)
                    {
                        newItemIndex = itemIndex - this.ScrollQuantum;
                    }
                    else if (this.ScrollQuantum > 1)
                    {
                        newItemIndex = itemIndex - 1;
                    }

                    break;
                case NavigationDirection.Down:
                    if (vertical)
                    {
                        newItemIndex = itemIndex + this.ScrollQuantum;
                    }
                    else if (this.ScrollQuantum > 1)
                    {
                        newItemIndex = itemIndex + 1;
                    }

                    break;

                case NavigationDirection.Left:
                    if (!vertical)
                    {
                        newItemIndex = itemIndex - this.ScrollQuantum;
                    }
                    else if (this.ScrollQuantum > 1)
                    {
                        newItemIndex = itemIndex - 1;
                    }
                    break;

                case NavigationDirection.Right:
                    if (!vertical)
                    {
                        newItemIndex = itemIndex + this.ScrollQuantum;
                    }
                    else if (this.ScrollQuantum > 1)
                    {
                        newItemIndex = itemIndex + 1;
                    }
                    break;

                case NavigationDirection.PageUp:
                    newItemIndex = Math.Max(0, itemIndex - (int)this.ViewportValue);
                    break;

                case NavigationDirection.PageDown:
                    newItemIndex = Math.Min(this.ItemCount - 1, itemIndex + (int)this.ViewportValue);
                    break;
            }
            return this.ScrollIntoView(newItemIndex);
        }

        /// <inheritdoc/>
        public override void ScrollIntoView(object item)
        {
            var index = this.Items.IndexOf(item);

            if (index != -1)
            {
                this.ScrollIntoView(index);
            }
        }

        /// <summary>
        /// Creates and removes containers such that we have at most enough containers to fill
        /// the panel.
        /// </summary>
        private void CreateAndRemoveContainers()
        {
            var generator = this.Owner.ItemContainerGenerator;
            var panel = this.VirtualizingPanel;

            if (this.Items != null && panel.IsAttachedToVisualTree)
            {
                var memberSelector = this.Owner.MemberSelector;
                var index = this.FirstIndex - 1;
                var step = 1;

                // check scroll alignment - add to start until aligned
                var toAdd = this.FirstIndex % this.ScrollQuantum;

                // add to start of panel
                for (int i = 0; i < toAdd; i++)
                {
                    var materialized = generator.Materialize(index, this.Items.ElementAt(index), memberSelector);
                    panel.Children.Insert(0, materialized.ContainerControl);
                    --index;
                    --this.FirstIndex;
                }
                index = this.NextIndex;

                if (!panel.IsFull)
                {
                    while (!panel.IsFull && index >= 0)
                    {
                        if (index >= this.ItemCount)
                        {
                            // We can fit more containers in the panel, but we're at the end of the
                            // items. If we're scrolled to the top (FirstIndex == 0), then there are
                            // no more items to create. Otherwise, go backwards adding containers to
                            // the beginning of the panel.
                            if (this.FirstIndex == 0)
                            {
                                break;
                            }
                            else
                            {
                                index = this.FirstIndex - 1;
                                step = -1;
                            }
                        }
                        //make sure first is scroll quantum
                        //allow panel to be partially empty on last line
                        toAdd = 1;
                        if (step < 0)
                        {
                            if (panel.PixelOverflow > 0)
                            {
                                break;
                            }
                            toAdd = (index + 1) % this.ScrollQuantum == 0 ? this.ScrollQuantum : (index + 1) % this.ScrollQuantum;
                        }

                        for (int i = 0; i < toAdd; i++)
                        {

                            var materialized = generator.Materialize(index, this.Items.ElementAt(index), memberSelector);

                            if (step == 1)
                            {
                                panel.Children.Add(materialized.ContainerControl);
                            }
                            else
                            {

                                panel.Children.Insert(0, materialized.ContainerControl);
                            }

                            index += step;
                        }
                    }

                    if (step == 1)
                    {
                        this.NextIndex = index;
                    }
                    else
                    {
                        this.NextIndex = this.ItemCount;
                        this.FirstIndex = index + 1;
                    }
                }
            }

            if (panel.OverflowCount > 0)
            {
                this.RemoveContainers(panel.OverflowCount);
            }
        }

        /// <summary>
        /// Updates the containers in the panel to make sure they are displaying the correct item
        /// based on <see cref="CustomItemVirtualizerBase.FirstIndex"/>.
        /// </summary>
        /// <remarks>
        /// This method requires that <see cref="CustomItemVirtualizerBase.FirstIndex"/> + the number of
        /// materialized containers is not more than <see cref="CustomItemVirtualizerBase.ItemCount"/>.
        /// </remarks>
        private void RecycleContainers()
        {
            var panel = this.VirtualizingPanel;
            var generator = this.Owner.ItemContainerGenerator;
            var selector = this.Owner.MemberSelector;
            var containers = generator.Containers.ToList();
            var itemIndex = this.FirstIndex;

            foreach (var container in containers)
            {
                var item = this.Items.ElementAt(itemIndex);

                if (!object.Equals(container.Item, item))
                {
                    if (!generator.TryRecycle(itemIndex, itemIndex, item, selector))
                    {
                        throw new NotImplementedException();
                    }
                }

                ++itemIndex;
            }
        }

        /// <summary>
        /// Recycles containers when a move occurs.
        /// </summary>
        /// <param name="delta">The delta of the move.</param>
        /// <remarks>
        /// If the move is less than a page, then this method moves the containers for the items
        /// that are still visible to the correct place, and recycles and moves the others. For
        /// example: if there are 20 items and 10 containers visible and the user scrolls 5
        /// items down, then the bottom 5 containers will be moved to the top and the top 5 will
        /// be moved to the bottom and recycled to display the newly visible item. Updates 
        /// <see cref="CustomItemVirtualizerBase.FirstIndex"/> and <see cref="CustomItemVirtualizerBase.NextIndex"/>
        /// with their new values.
        /// </remarks>
        private void RecycleContainersForMove(int delta)
        {
            var panel = this.VirtualizingPanel;
            var generator = this.Owner.ItemContainerGenerator;
            var selector = this.Owner.MemberSelector;

            // validate delta it should never overflow last index or generate index < 0 
            var clampedDelta = MathUtilities.Clamp(delta, -this.FirstIndex, this.ItemCount - this.FirstIndex - panel.Children.Count);
            if (clampedDelta == 0)
            {
                return;
            }

            var sign = delta < 0 ? -1 : 1;
            var count = Math.Min(Math.Abs(delta), panel.Children.Count);
            var move = count < panel.Children.Count;
            var first = delta < 0 && move ? panel.Children.Count + delta : 0;
            //adjust recycle count if not enough
            int toAdd = 0;
            if (delta < 0)
            {
                toAdd = panel.Children.Count - panel.Children.Count / this.ScrollQuantum * this.ScrollQuantum;
                toAdd = (this.ScrollQuantum - toAdd) % this.ScrollQuantum;
                first += toAdd;
                count -= toAdd;
            }

            var oldItemIndex = this.FirstIndex + first;
            var newItemIndex = oldItemIndex + delta + ((panel.Children.Count - count) * sign);

            for (var i = 0; i < count - (delta - clampedDelta); ++i)
            {
                var item = this.Items.ElementAt(newItemIndex);

                if (!generator.TryRecycle(oldItemIndex, newItemIndex, item, selector))
                {
                    throw new NotImplementedException();
                }

                oldItemIndex++;
                newItemIndex++;
            }
            for (var i = 0; i < toAdd; i++)
            {
                var materialized = generator.Materialize(newItemIndex, this.Items.ElementAt(newItemIndex), selector);
                panel.Children.Add(materialized.ContainerControl);
                newItemIndex++;
            }

            if (move)
            {
                if (delta > 0)
                {
                    panel.Children.MoveRange(first, count, panel.Children.Count);
                }
                else
                {
                    panel.Children.MoveRange(first, count + toAdd, 0);
                }
            }

            if (clampedDelta < delta)
            {
                panel.Children.RemoveRange(panel.Children.Count - (delta - clampedDelta), (delta - clampedDelta));
                this.Owner.ItemContainerGenerator.Dematerialize(oldItemIndex, (delta - clampedDelta));
            }

            this.FirstIndex += delta;
            this.NextIndex = this.FirstIndex + panel.Children.Count;
        }

        /// <summary>
        /// Recycles containers due to items being removed.
        /// </summary>
        private void RecycleContainersOnRemove()
        {
            var panel = this.VirtualizingPanel;

            if (this.NextIndex <= this.ItemCount)
            {
                // Items have been removed but FirstIndex..NextIndex is still a valid range in the
                // items, so just recycle the containers to adapt to the new state.
                this.RecycleContainers();
            }
            else
            {
                // Items have been removed and now the range FirstIndex..NextIndex goes out of 
                // the item bounds. Remove any excess containers, try to scroll up and then recycle
                // the containers to make sure they point to the correct item.
                var newFirstIndex = Math.Max(0, this.FirstIndex - (this.NextIndex - this.ItemCount));
                newFirstIndex += newFirstIndex % this.ScrollQuantum;
                var delta = newFirstIndex - this.FirstIndex;
                var newNextIndex = this.NextIndex + delta;

                if (newNextIndex > this.ItemCount)
                {
                    this.RemoveContainers(newNextIndex - this.ItemCount);
                }

                if (delta != 0)
                {
                    this.RecycleContainersForMove(delta);
                }

                this.RecycleContainers();
            }
        }

        /// <summary>
        /// Removes the specified number of containers from the end of the panel and updates
        /// <see cref="CustomItemVirtualizerBase.NextIndex"/>.
        /// </summary>
        /// <param name="count">The number of containers to remove.</param>
        private void RemoveContainers(int count)
        {
            var index = this.VirtualizingPanel.Children.Count - count;

            this.VirtualizingPanel.Children.RemoveRange(index, count);
            this.Owner.ItemContainerGenerator.Dematerialize(this.FirstIndex + index, count);
            this.NextIndex -= count;
        }

        /// <summary>
        /// Scrolls the item with the specified index into view.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <returns>The container that was brought into view.</returns>
        private IControl ScrollIntoView(int index)
        {
            var panel = this.VirtualizingPanel;
            var generator = this.Owner.ItemContainerGenerator;
            var newOffset = -1.0;

            if (index >= 0 && index < this.ItemCount)
            {
                var offset = panel.PixelOffset > 0 ? this.ScrollQuantum : 0;

                if (index <= this.FirstIndex + offset)
                {
                    newOffset = index;
                }
                else if (index >= this.NextIndex)
                {
                    newOffset = index - Math.Ceiling(this.ViewportValue - 1);
                }

                if (newOffset != -1)
                {
                    this.OffsetValue = newOffset / this.ScrollQuantum;
                }

                var container = generator.ContainerFromIndex(index);
                var layoutManager = (this.Owner.GetVisualRoot() as ILayoutRoot)?.LayoutManager;

                // We need to do a layout here because it's possible that the container we moved to
                // is only partially visible due to differing item sizes. If the container is only 
                // partially visible, scroll again. Don't do this if there's no layout manager:
                // it means we're running a unit test.
                if (container != null && layoutManager != null)
                {
                    layoutManager.ExecuteLayoutPass();

                    if (panel.ScrollDirection == Orientation.Vertical)
                    {
                        if (container.Bounds.Y < panel.Bounds.Y || container.Bounds.Bottom > panel.Bounds.Bottom)
                        {
                            this.OffsetValue += 1;
                        }
                    }
                    else
                    {
                        if (container.Bounds.X < panel.Bounds.X || container.Bounds.Right > panel.Bounds.Right)
                        {
                            this.OffsetValue += 1;
                        }
                    }
                }

                return container;
            }

            return null;
        }

        /// <summary>
        /// Ensures an offset value is within the value range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The coerced value.</returns>
        private double CoerceOffset(double value)
        {
            var max = Math.Max(this.ExtentValue - this.ViewportValue, 0);
            return MathUtilities.Clamp(value, 0, max);
        }
    }
}
