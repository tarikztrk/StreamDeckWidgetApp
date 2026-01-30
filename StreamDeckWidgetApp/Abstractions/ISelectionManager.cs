using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Abstractions;

/// <summary>
/// Manages the selection state of deck items.
/// </summary>
public interface ISelectionManager
{
    /// <summary>
    /// Gets the currently selected deck item.
    /// </summary>
    DeckItem? SelectedItem { get; }

    /// <summary>
    /// Event raised when the selected item changes.
    /// </summary>
    event Action<DeckItem?>? SelectionChanged;

    /// <summary>
    /// Selects the specified deck item and clears previous selection.
    /// </summary>
    /// <param name="item">The item to select, or null to clear selection.</param>
    void SelectItem(DeckItem? item);

    /// <summary>
    /// Clears the current selection.
    /// </summary>
    void ClearSelection();
}
