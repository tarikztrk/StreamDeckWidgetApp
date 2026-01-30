using System.Collections.ObjectModel;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Abstractions;

/// <summary>
/// Interface for grid and DeckItem management.
/// Handles grid sizing and item collection operations.
/// </summary>
public interface IGridService
{
    /// <summary>
    /// Maximum allowed rows.
    /// </summary>
    int MaxRows { get; }
    
    /// <summary>
    /// Maximum allowed columns.
    /// </summary>
    int MaxColumns { get; }
    
    /// <summary>
    /// The collection of deck items for UI binding.
    /// </summary>
    ObservableCollection<DeckItem> DeckItems { get; }
    
    /// <summary>
    /// Event raised when the grid is refreshed.
    /// </summary>
    event Action? GridRefreshed;
    
    /// <summary>
    /// Refreshes the grid based on the given profile's rows and columns.
    /// Updates DeckItems collection accordingly.
    /// </summary>
    void RefreshGrid(Profile profile);
    
    /// <summary>
    /// Clamps the given row value to valid range.
    /// </summary>
    int ClampRows(int value);
    
    /// <summary>
    /// Clamps the given column value to valid range.
    /// </summary>
    int ClampColumns(int value);
    
    /// <summary>
    /// Swaps two items in the DeckItems collection by their indices.
    /// Updates Row and Column properties after swap.
    /// </summary>
    void SwapItems(int fromIndex, int toIndex);
}
