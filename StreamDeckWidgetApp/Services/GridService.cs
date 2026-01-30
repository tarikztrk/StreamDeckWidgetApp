using System.Collections.ObjectModel;
using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Services;

/// <summary>
/// Service for grid and DeckItem management.
/// Handles grid sizing and item collection operations.
/// </summary>
public class GridService : IGridService
{
    public int MaxRows => 8;
    public int MaxColumns => 10;

    public ObservableCollection<DeckItem> DeckItems { get; } = new();

    public event Action? GridRefreshed;

    public void RefreshGrid(Profile profile)
    {
        int rows = profile.Rows;
        int columns = profile.Columns;
        int totalSlots = rows * columns;

        // Add missing items to profile
        while (profile.Items.Count < totalSlots)
        {
            int i = profile.Items.Count;
            profile.Items.Add(new DeckItem
            {
                Title = "Boş",
                Color = "#222222",
                Row = i / columns,
                Column = i % columns
            });
        }

        // Remove excess items from profile
        while (profile.Items.Count > totalSlots)
        {
            profile.Items.RemoveAt(profile.Items.Count - 1);
        }

        // Update row/column info for all items
        for (int i = 0; i < totalSlots; i++)
        {
            var item = profile.Items[i];
            item.Row = i / columns;
            item.Column = i % columns;
        }

        // Rebuild DeckItems collection
        DeckItems.Clear();
        foreach (var item in profile.Items)
        {
            DeckItems.Add(item);
        }

        GridRefreshed?.Invoke();
    }

    public int ClampRows(int value)
    {
        return Math.Clamp(value, 1, MaxRows);
    }

    public int ClampColumns(int value)
    {
        return Math.Clamp(value, 1, MaxColumns);
    }
    
    public void SwapItems(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= DeckItems.Count || 
            toIndex < 0 || toIndex >= DeckItems.Count || 
            fromIndex == toIndex)
        {
            return;
        }
        
        // Calculate columns from first item's position
        int columns = 1;
        for (int i = 0; i < DeckItems.Count; i++)
        {
            if (DeckItems[i].Row == 1)
            {
                columns = i;
                break;
            }
        }
        
        if (columns == 1 && DeckItems.Count > 1)
        {
            // Fallback: find max column value + 1
            columns = DeckItems.Max(item => item.Column) + 1;
        }
        
        // Swap items in the collection
        var temp = DeckItems[fromIndex];
        DeckItems[fromIndex] = DeckItems[toIndex];
        DeckItems[toIndex] = temp;
        
        // Update Row and Column properties after swap
        DeckItems[fromIndex].Row = fromIndex / columns;
        DeckItems[fromIndex].Column = fromIndex % columns;
        DeckItems[toIndex].Row = toIndex / columns;
        DeckItems[toIndex].Column = toIndex % columns;
    }
}
