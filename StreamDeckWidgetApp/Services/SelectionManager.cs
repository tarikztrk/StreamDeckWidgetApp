using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Services;

/// <summary>
/// Manages the selection state of deck items.
/// Implements Single Responsibility Principle by separating selection logic from ViewModel.
/// </summary>
public class SelectionManager : ISelectionManager
{
    private DeckItem? _selectedItem;

    public DeckItem? SelectedItem => _selectedItem;

    public event Action<DeckItem?>? SelectionChanged;

    public void SelectItem(DeckItem? item)
    {
        // Clear previous selection
        if (_selectedItem != null)
        {
            _selectedItem.IsSelected = false;
        }

        _selectedItem = item;

        // Mark new selection
        if (_selectedItem != null)
        {
            _selectedItem.IsSelected = true;
        }

        // Notify listeners
        SelectionChanged?.Invoke(_selectedItem);
    }

    public void ClearSelection()
    {
        SelectItem(null);
    }
}
