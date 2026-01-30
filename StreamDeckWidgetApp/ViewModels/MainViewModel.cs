using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Core;
using StreamDeckWidgetApp.Core.Helpers;
using StreamDeckWidgetApp.Models;
using StreamDeckWidgetApp.Services;

namespace StreamDeckWidgetApp.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly IActionService _actionService;
    private readonly IProfileService _profileService;
    private readonly IGridService _gridService;
    private readonly IEditorWindowService _editorWindowService;

    // --- State Properties ---

    private DeckItem? _selectedDeckItem;
    public DeckItem? SelectedDeckItem
    {
        get => _selectedDeckItem;
        set
        {
            // Clear previous selection
            if (_selectedDeckItem != null)
                _selectedDeckItem.IsSelected = false;

            SetField(ref _selectedDeckItem, value);

            // Mark new selection
            if (_selectedDeckItem != null)
                _selectedDeckItem.IsSelected = true;
        }
    }

    // Grid size changed event - notifies MainWindow to update dimensions
    public event Action? GridSizeChanged;

    // Grid Dimensions - Dynamically changeable
    public int Rows
    {
        get => _profileService.CurrentProfile.Rows;
        set
        {
            int newVal = _gridService.ClampRows(value);
            if (_profileService.CurrentProfile.Rows != newVal)
            {
                _profileService.CurrentProfile.Rows = newVal;
                OnPropertyChanged();
                _gridService.RefreshGrid(_profileService.CurrentProfile);
                GridSizeChanged?.Invoke();
            }
        }
    }

    public int Columns
    {
        get => _profileService.CurrentProfile.Columns;
        set
        {
            int newVal = _gridService.ClampColumns(value);
            if (_profileService.CurrentProfile.Columns != newVal)
            {
                _profileService.CurrentProfile.Columns = newVal;
                OnPropertyChanged();
                _gridService.RefreshGrid(_profileService.CurrentProfile);
                GridSizeChanged?.Invoke();
            }
        }
    }

    // Button size options (Display Name -> Pixel Size)
    public Dictionary<string, int> ButtonSizeOptions { get; } = new()
    {
        { "Orta (48px)", 65 },
        { "Büyük (64px)", 85 }
    };

    // Active button size
    public int SelectedButtonSize
    {
        get => _profileService.CurrentProfile.ButtonSize;
        set
        {
            if (_profileService.CurrentProfile.ButtonSize != value)
            {
                _profileService.CurrentProfile.ButtonSize = value;
                OnPropertyChanged();
                GridSizeChanged?.Invoke();
            }
        }
    }

    // Action types for ComboBox
    public List<string> ActionTypes { get; } = new()
    {
        "Execute",
        "Hotkey",
        "Website",
        "MediaControl",
        "AudioControl",
        "TextType"
    };

    // DeckItems - proxied from GridService
    public ObservableCollection<DeckItem> DeckItems => _gridService.DeckItems;

    // Editor open state - proxied from EditorWindowService
    public bool IsEditorOpen => _editorWindowService.IsEditorOpen;

    // --- Preset Library ---
    private ObservableCollection<PresetModel> _libraryItems;
    public ObservableCollection<PresetModel> LibraryItems
    {
        get => _libraryItems;
        set => SetField(ref _libraryItems, value);
    }

    private string _librarySearchText = string.Empty;
    public string LibrarySearchText
    {
        get => _librarySearchText;
        set
        {
            if (SetField(ref _librarySearchText, value))
            {
                FilterLibrary();
            }
        }
    }

    private string _selectedCategory = "Tümü";
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

    public List<string> LibraryCategories { get; } = new() { "Tümü" };

    // --- Profile Properties (proxied from ProfileService) ---
    public IReadOnlyList<Profile> Profiles => _profileService.Profiles;
    public Profile CurrentProfile => _profileService.CurrentProfile;

    public string CurrentProfileName
    {
        get => _profileService.CurrentProfile?.Name ?? "Profil";
        set
        {
            if (_profileService.CurrentProfile != null && _profileService.CurrentProfile.Name != value)
            {
                _profileService.RenameCurrentProfile(value);
                OnPropertyChanged();
            }
        }
    }

    // --- Commands ---
    public ICommand ItemClickCommand { get; }
    public ICommand CloseAppCommand { get; }
    public ICommand OpenEditorCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand SwitchProfileCommand { get; }
    public ICommand CreateProfileCommand { get; }
    public ICommand DeleteProfileCommand { get; }
    public ICommand DuplicateProfileCommand { get; }

    public MainViewModel(
        IActionService actionService,
        IProfileService profileService,
        IGridService gridService,
        IEditorWindowService editorWindowService)
    {
        _actionService = actionService;
        _profileService = profileService;
        _gridService = gridService;
        _editorWindowService = editorWindowService;
        _libraryItems = new ObservableCollection<PresetModel>();

        // Load library categories
        LibraryCategories.AddRange(PresetService.GetCategories());

        // Subscribe to service events
        _profileService.ProfileChanged += OnProfileChanged;
        _gridService.GridRefreshed += OnGridRefreshed;
        _editorWindowService.EditorClosed += OnEditorClosed;

        // Initial grid refresh
        _gridService.RefreshGrid(_profileService.CurrentProfile);

        // Commands
        ItemClickCommand = new RelayCommand(OnItemClick);
        CloseAppCommand = new RelayCommand(_ =>
        {
            SaveChanges();
            Application.Current.Shutdown();
        });
        OpenEditorCommand = new RelayCommand(_ => OpenEditor());
        SaveCommand = new RelayCommand(_ => SaveChanges());

        // Profile commands
        SwitchProfileCommand = new RelayCommand(profileId => _profileService.SwitchProfile(profileId as string ?? ""));
        CreateProfileCommand = new RelayCommand(_ => _profileService.CreateProfile($"Profil {Profiles.Count + 1}"));
        DeleteProfileCommand = new RelayCommand(_ => DeleteCurrentProfile());
        DuplicateProfileCommand = new RelayCommand(_ => _profileService.DuplicateCurrentProfile($"{CurrentProfile.Name} (Kopya)"));

        // Load library
        LoadLibrary();
    }

    // --- Event Handlers ---

    private void OnProfileChanged()
    {
        // Refresh UI when profile changes
        OnPropertyChanged(nameof(Rows));
        OnPropertyChanged(nameof(Columns));
        OnPropertyChanged(nameof(SelectedButtonSize));
        OnPropertyChanged(nameof(CurrentProfileName));
        OnPropertyChanged(nameof(CurrentProfile));
        OnPropertyChanged(nameof(Profiles));

        _gridService.RefreshGrid(_profileService.CurrentProfile);
    }

    private void OnGridRefreshed()
    {
        OnPropertyChanged(nameof(DeckItems));
        OnPropertyChanged(nameof(Rows));
        OnPropertyChanged(nameof(Columns));
    }

    private void OnEditorClosed()
    {
        SelectedDeckItem = null;
        OnPropertyChanged(nameof(IsEditorOpen));
    }

    // --- Private Methods ---

    private void OnItemClick(object? parameter)
    {
        if (parameter is DeckItem item)
        {
            if (IsEditorOpen)
            {
                SelectedDeckItem = item;
                return;
            }

            if (string.IsNullOrEmpty(item.Command))
            {
                OpenEditor();
            }
            

            if (item.BehaviorType == "Toggle")
            {
                item.IsActive = !item.IsActive;
            }
            _actionService.ExecuteItem(item);

        }
    }

    private void OpenEditor()
    {
        _editorWindowService.OpenEditor();
        OnPropertyChanged(nameof(IsEditorOpen));
    }

    private void DeleteCurrentProfile()
    {
        if (Profiles.Count <= 1)
        {
            MessageBox.Show("En az bir profil olmalı!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show(
            $"\"{CurrentProfile.Name}\" profili silinecek. Emin misiniz?",
            "Profil Sil",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _profileService.DeleteCurrentProfile();
        }
    }

    /// <summary>
    /// Sets the main window reference for modal behavior.
    /// </summary>
    public void SetMainWindow(Window mainWindow)
    {
        _editorWindowService.SetMainWindow(mainWindow);
    }

    /// <summary>
    /// Forces layout refresh - for initial load spacing issues.
    /// </summary>
    public void ForceLayoutRefresh()
    {
        OnPropertyChanged(nameof(DeckItems));
        OnPropertyChanged(nameof(Rows));
        OnPropertyChanged(nameof(Columns));
        OnPropertyChanged(nameof(SelectedButtonSize));
    }

    public void SaveChanges()
    {
        _profileService.CurrentProfile.Items = DeckItems.ToList();
        _profileService.SaveCurrentProfile();
    }

    public void SaveAndCloseEditor()
    {
        SaveChanges();
        _editorWindowService.CloseEditor();
    }

    public void HandleFileDrop(DeckItem targetItem, string filePath)
    {
        string ext = System.IO.Path.GetExtension(filePath).ToLower();
        string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

        // 1. If image file
        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".ico")
        {
            targetItem.IconPath = filePath;
            targetItem.Title = "";
            if (IsEditorOpen) SelectedDeckItem = targetItem;
            return;
        }

        // 2. If EXE or shortcut
        if (ext == ".exe" || ext == ".lnk" || ext == ".bat")
        {
            targetItem.Title = fileName;
            targetItem.Command = filePath;
            targetItem.ActionType = "Execute";

            // Extract icon
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string iconsFolder = System.IO.Path.Combine(appData, "StreamDeckWidgetApp", "CachedIcons");
            string? newIconPath = IconHelper.ExtractAndSaveIcon(filePath, iconsFolder);

            if (!string.IsNullOrEmpty(newIconPath))
            {
                targetItem.IconPath = newIconPath;
            }

            if (IsEditorOpen) SelectedDeckItem = targetItem;

            // Auto-save
            SaveChanges();
        }
    }

    // --- Preset Library Methods ---

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

        if (SelectedCategory != "Tümü")
        {
            allPresets = allPresets.Where(p => p.Category == SelectedCategory).ToList();
        }

        if (!string.IsNullOrWhiteSpace(LibrarySearchText))
        {
            allPresets = PresetService.SearchPresets(LibrarySearchText);
            if (SelectedCategory != "Tümü")
            {
                allPresets = allPresets.Where(p => p.Category == SelectedCategory).ToList();
            }
        }

        LibraryItems.Clear();
        foreach (var preset in allPresets)
        {
            LibraryItems.Add(preset);
        }
    }

    /// <summary>
    /// Applies a preset to the selected deck item.
    /// </summary>
    public void ApplyPresetToSelectedItem(PresetModel preset)
    {
        if (SelectedDeckItem == null) return;

        var deckItem = preset.ToDeckItem();
        SelectedDeckItem.Title = deckItem.Title;
        SelectedDeckItem.ActionType = deckItem.ActionType;
        SelectedDeckItem.Command = deckItem.Command;
        SelectedDeckItem.Color = deckItem.Color;
        SelectedDeckItem.BehaviorType = deckItem.BehaviorType;
    }
}
