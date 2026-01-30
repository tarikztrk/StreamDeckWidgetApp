using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Core;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.ViewModels;

/// <summary>
/// Main ViewModel - refactored to follow Single Responsibility Principle.
/// Now delegates specialized concerns to dedicated services and ViewModels.
/// </summary>
public class MainViewModel : ObservableObject
{
    private readonly IActionService _actionService;
    private readonly IProfileService _profileService;
    private readonly IGridService _gridService;
    private readonly IEditorWindowService _editorWindowService;
    private readonly ISelectionManager _selectionManager;
    private readonly IFileDropHandler _fileDropHandler;
    private readonly IDialogService _dialogService;
    private readonly ILibraryViewModel _libraryViewModel;

    // --- State Properties ---

    /// <summary>
    /// Gets the currently selected deck item (proxied from SelectionManager).
    /// </summary>
    public DeckItem? SelectedDeckItem
    {
        get => _selectionManager.SelectedItem;
        set => _selectionManager.SelectItem(value);
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

    // --- Preset Library (Delegated to LibraryViewModel) ---
    
    /// <summary>
    /// Gets the library ViewModel that manages preset library functionality.
    /// </summary>
    public ILibraryViewModel LibraryViewModel => _libraryViewModel;

    // Proxy properties for XAML binding compatibility
    public ObservableCollection<PresetModel> LibraryItems => _libraryViewModel.LibraryItems;
    public string LibrarySearchText
    {
        get => _libraryViewModel.SearchText;
        set => _libraryViewModel.SearchText = value;
    }
    public string SelectedCategory
    {
        get => _libraryViewModel.SelectedCategory;
        set => _libraryViewModel.SelectedCategory = value;
    }
    public IReadOnlyList<string> LibraryCategories => _libraryViewModel.Categories;

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
        IEditorWindowService editorWindowService,
        ISelectionManager selectionManager,
        IFileDropHandler fileDropHandler,
        IDialogService dialogService,
        ILibraryViewModel libraryViewModel)
    {
        _actionService = actionService;
        _profileService = profileService;
        _gridService = gridService;
        _editorWindowService = editorWindowService;
        _selectionManager = selectionManager;
        _fileDropHandler = fileDropHandler;
        _dialogService = dialogService;
        _libraryViewModel = libraryViewModel;

        // Subscribe to service events
        _profileService.ProfileChanged += OnProfileChanged;
        _gridService.GridRefreshed += OnGridRefreshed;
        _editorWindowService.EditorClosed += OnEditorClosed;
        _selectionManager.SelectionChanged += OnSelectionChanged;
        _libraryViewModel.PropertyChanged += (s, e) =>
        {
            // Forward library property changes
            if (e.PropertyName == nameof(ILibraryViewModel.LibraryItems))
                OnPropertyChanged(nameof(LibraryItems));
            else if (e.PropertyName == nameof(ILibraryViewModel.SearchText))
                OnPropertyChanged(nameof(LibrarySearchText));
            else if (e.PropertyName == nameof(ILibraryViewModel.SelectedCategory))
                OnPropertyChanged(nameof(SelectedCategory));
        };

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
        _selectionManager.ClearSelection();
        OnPropertyChanged(nameof(IsEditorOpen));
    }

    private void OnSelectionChanged(DeckItem? item)
    {
        OnPropertyChanged(nameof(SelectedDeckItem));
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
            _dialogService.ShowWarning("En az bir profil olmalı!", "Uyarı");
            return;
        }

        bool confirmed = _dialogService.ShowConfirmation(
            $"\"{CurrentProfile.Name}\" profili silinecek. Emin misiniz?",
            "Profil Sil");

        if (confirmed)
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
        bool success = _fileDropHandler.HandleFileDrop(targetItem, filePath);
        
        if (success)
        {
            // Update selection if editor is open
            if (IsEditorOpen)
            {
                SelectedDeckItem = targetItem;
            }

            // Auto-save after successful drop
            SaveChanges();
        }
    }

    /// <summary>
    /// Applies a preset to the selected deck item (delegates to LibraryViewModel).
    /// </summary>
    public void ApplyPresetToSelectedItem(PresetModel preset)
    {
        if (SelectedDeckItem == null) return;
        _libraryViewModel.ApplyPreset(preset, SelectedDeckItem);
    }
    
    /// <summary>
    /// Swaps two deck items by their indices in the collection.
    /// </summary>
    public void SwapDeckItems(int fromIndex, int toIndex)
    {
        _gridService.SwapItems(fromIndex, toIndex);
    }
}
