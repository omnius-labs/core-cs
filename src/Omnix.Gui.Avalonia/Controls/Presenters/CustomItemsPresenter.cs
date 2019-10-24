// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using static Avalonia.Utilities.MathUtilities;

namespace Omnix.Avalonia.Controls.Presenters
{
    /// <summary>
    /// Displays items inside an <see cref="ItemsControl"/>.
    /// </summary>
    public class CustomItemsPresenter : ItemsPresenterBase, ILogicalScrollable
    {
        /// <summary>
        /// Defines the <see cref="VirtualizationMode"/> property.
        /// </summary>
        public static readonly StyledProperty<ItemVirtualizationMode> VirtualizationModeProperty =
            AvaloniaProperty.Register<CustomItemsPresenter, ItemVirtualizationMode>(
                nameof(VirtualizationMode),
                defaultValue: ItemVirtualizationMode.None);

        private bool _canHorizontallyScroll;
        private bool _canVerticallyScroll;

        /// <summary>
        /// Initializes static members of the <see cref="CustomItemsPresenter"/> class.
        /// </summary>
        static CustomItemsPresenter()
        {
            KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue(
                typeof(CustomItemsPresenter),
                KeyboardNavigationMode.Once);

            VirtualizationModeProperty.Changed
                .AddClassHandler<CustomItemsPresenter>(x => x.VirtualizationModeChanged);
        }

        /// <summary>
        /// Gets or sets the virtualization mode for the items.
        /// </summary>
        public ItemVirtualizationMode VirtualizationMode
        {
            get { return this.GetValue(VirtualizationModeProperty); }
            set { this.SetValue(VirtualizationModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the content can be scrolled horizontally.
        /// </summary>
        bool ILogicalScrollable.CanHorizontallyScroll
        {
            get { return _canHorizontallyScroll; }
            set
            {
                _canHorizontallyScroll = value;
                this.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the content can be scrolled horizontally.
        /// </summary>
        bool ILogicalScrollable.CanVerticallyScroll
        {
            get { return _canVerticallyScroll; }
            set
            {
                _canVerticallyScroll = value;
                this.InvalidateMeasure();
            }
        }
        /// <inheritdoc/>
        bool ILogicalScrollable.IsLogicalScrollEnabled
        {
            get { return this.Virtualizer?.IsLogicalScrollEnabled ?? false; }
        }

        /// <inheritdoc/>
        Size IScrollable.Extent => this.Virtualizer?.Extent ?? Size.Empty;

        /// <inheritdoc/>
        Vector IScrollable.Offset
        {
            get { return this.Virtualizer?.Offset ?? new Vector(); }
            set
            {
                if (this.Virtualizer != null)
                {
                    this.Virtualizer.Offset = this.CoerceOffset(value);
                }
            }
        }

        /// <inheritdoc/>
        Size IScrollable.Viewport => this.Virtualizer?.Viewport ?? this.Bounds.Size;

        /// <inheritdoc/>
        Action ILogicalScrollable.InvalidateScroll { get; set; }

        /// <inheritdoc/>
        Size ILogicalScrollable.ScrollSize => new Size(1, 1);

        /// <inheritdoc/>
        Size ILogicalScrollable.PageScrollSize => new Size(0, 1);

        internal CustomItemVirtualizerBase Virtualizer { get; private set; }

        /// <inheritdoc/>
        bool ILogicalScrollable.BringIntoView(IControl target, Rect targetRect)
        {
            return false;
        }

        /// <inheritdoc/>
        IControl ILogicalScrollable.GetControlInDirection(NavigationDirection direction, IControl from)
        {
            return this.Virtualizer?.GetControlInDirection(direction, from);
        }

        public override void ScrollIntoView(object item)
        {
            this.Virtualizer?.ScrollIntoView(item);
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            return this.Virtualizer?.MeasureOverride(availableSize) ?? Size.Empty;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return this.Virtualizer?.ArrangeOverride(finalSize) ?? Size.Empty;
        }

        /// <inheritdoc/>
        protected override void PanelCreated(IPanel panel)
        {
            this.Virtualizer?.Dispose();
            this.Virtualizer = CustomItemVirtualizerBase.Create(this);
            ((ILogicalScrollable)this).InvalidateScroll?.Invoke();

            KeyboardNavigation.SetTabNavigation(
                (InputElement)this.Panel,
                KeyboardNavigation.GetTabNavigation(this));
        }

        protected override void ItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            this.Virtualizer?.ItemsChanged(this.Items, e);
        }

        private Vector CoerceOffset(Vector value)
        {
            var scrollable = (ILogicalScrollable)this;
            var maxX = Math.Max(scrollable.Extent.Width - scrollable.Viewport.Width, 0);
            var maxY = Math.Max(scrollable.Extent.Height - scrollable.Viewport.Height, 0);
            return new Vector(Clamp(value.X, 0, maxX), Clamp(value.Y, 0, maxY));
        }

        private void VirtualizationModeChanged(AvaloniaPropertyChangedEventArgs e)
        {
            this.Virtualizer?.Dispose();
            this.Virtualizer = CustomItemVirtualizerBase.Create(this);
            ((ILogicalScrollable)this).InvalidateScroll?.Invoke();
        }
    }
}
