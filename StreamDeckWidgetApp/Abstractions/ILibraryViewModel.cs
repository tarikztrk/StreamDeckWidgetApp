using System.Collections.ObjectModel;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Abstractions;

/// <summary>
/// Manages the preset library functionality (search, filter, categories).
/// </summary>
public interface ILibraryViewModel
{
    /// <summary>
    /// Gets the filtered collection of preset items to display.
    /// </summary>
    ObservableCollection<PresetModel> LibraryItems { get; }

    /// <summary>
    /// Gets or sets the search filter text.
    /// </summary>
    string SearchText { get; set; }

    /// <summary>
    /// Gets or sets the selected category filter.
    /// </summary>
    string SelectedCategory { get; set; }

    /// <summary>
    /// Gets the list of available categories.
    /// </summary>
    IReadOnlyList<string> Categories { get; }

    /// <summary>
    /// Event raised when a property changes.
    /// </summary>
    event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Applies a preset to the specified deck item.
    /// </summary>
    /// <param name="preset">The preset to apply.</param>
    /// <param name="targetItem">The deck item to configure.</param>
    void ApplyPreset(PresetModel preset, DeckItem targetItem);
}
