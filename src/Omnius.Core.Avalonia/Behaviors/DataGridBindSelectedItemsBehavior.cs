using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Omnius.Core.Avalonia.Behaviors;

public class DataGridBindSelectedItemsBehavior : Behavior<DataGrid>
{
    protected override void OnAttached()
    {
        base.OnAttached();

        var grid = this.AssociatedObject as DataGrid;
        if (grid == null) return;

        grid.SelectionChanged += this.SelectionChanged;
    }

    protected override void OnDetaching()
    {
        var grid = this.AssociatedObject as DataGrid;
        if (grid == null) return;

        grid.SelectionChanged -= this.SelectionChanged;

        base.OnDetaching();
    }

    public static readonly DirectProperty<DataGridBindSelectedItemsBehavior, IList> SelectedItemsProperty =
        AvaloniaProperty.RegisterDirect<DataGridBindSelectedItemsBehavior, IList>(
            nameof(SelectedItems),
            o => o.SelectedItems,
            (o, v) => o.SelectedItems = v);

    private IList _selectedItems = new List<object>();

    public IList SelectedItems
    {
        get { return _selectedItems; }
        set { SetAndRaise(SelectedItemsProperty, ref _selectedItems, value); }
    }

    private void SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        foreach (var addedItem in e.AddedItems)
        {
            SelectedItems.Add(addedItem);
        }

        foreach (var removedItem in e.RemovedItems)
        {
            SelectedItems.Remove(removedItem);
        }
    }
}
