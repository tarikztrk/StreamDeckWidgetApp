using System.Collections.ObjectModel;
using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Core;
using StreamDeckWidgetApp.Models;
using StreamDeckWidgetApp.Services;

namespace StreamDeckWidgetApp.ViewModels;

/// <summary>
/// ViewModel for preset library management (search, filter, apply).
/// Implements Single Responsibility Principle by separating library logic from MainViewModel.
/// </summary>
public class LibraryViewModel : ObservableObject, ILibraryViewModel
{
    private ObservableCollection<PresetModel> _libraryItems;
    private string _searchText = string.Empty;
    private string _selectedCategory = "Tümü";

    public ObservableCollection<PresetModel> LibraryItems
    {
        get => _libraryItems;
        private set => SetField(ref _libraryItems, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetField(ref _searchText, value))
            {
                FilterLibrary();
            }
        }
    }

    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetField(ref _selectedCategory, value))
            {
                FilterLibrary();
            }
        }
    }

    public IReadOnlyList<string> Categories { get; }

    public LibraryViewModel()
    {
        _libraryItems = new ObservableCollection<PresetModel>();

        // Load categories
        var categories = new List<string> { "Tümü" };
        categories.AddRange(PresetService.GetCategories());
        Categories = categories;

        // Load initial library
        LoadLibrary();
    }

    public void ApplyPreset(PresetModel preset, DeckItem targetItem)
    {
        if (preset == null || targetItem == null)
            return;

        var deckItem = preset.ToDeckItem();
        targetItem.Title = deckItem.Title;
        targetItem.ActionType = deckItem.ActionType;
        targetItem.Command = deckItem.Command;
        targetItem.Color = deckItem.Color;
        targetItem.BehaviorType = deckItem.BehaviorType;
    }

    private void LoadLibrary()
    {
        var allPresets = PresetService.GetAllPresets();
        LibraryItems.Clear();
        foreach (var preset in allPresets)
        {
            LibraryItems.Add(preset);
        }
    }

    private void FilterLibrary()
    {
        var allPresets = PresetService.GetAllPresets();

        // Apply category filter
        if (SelectedCategory != "Tümü")
        {
            allPresets = allPresets.Where(p => p.Category == SelectedCategory).ToList();
        }

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            allPresets = PresetService.SearchPresets(SearchText);
            if (SelectedCategory != "Tümü")
            {
                allPresets = allPresets.Where(p => p.Category == SelectedCategory).ToList();
            }
        }

        // Update collection
        LibraryItems.Clear();
        foreach (var preset in allPresets)
        {
            LibraryItems.Add(preset);
        }
    }
}
