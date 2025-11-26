using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using StreamDeckWidgetApp.Core;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.ViewModels;

/// <summary>
/// ViewModel for EditorWindow - shares data with MainViewModel
/// </summary>
public class EditorViewModel : ObservableObject
{
    private readonly MainViewModel _mainViewModel;
    private Window? _editorWindow;
    private DispatcherTimer? _saveTimer;
    private string _saveStatus = "Tüm değişiklikler kaydedildi";
    private bool _hasUnsavedChanges;

    // --- Commands ---
    public ICommand CloseCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand ItemClickCommand => _mainViewModel.ItemClickCommand; // Proxy

    // --- Save Status Property ---
    public string SaveStatus
    {
        get => _saveStatus;
        set => SetField(ref _saveStatus, value);
    }

    // --- Proxied Properties from MainViewModel ---
    public DeckItem? SelectedDeckItem
    {
        get => _mainViewModel.SelectedDeckItem;
        set => _mainViewModel.SelectedDeckItem = value;
    }

    public int Rows
    {
        get => _mainViewModel.Rows;
        set => _mainViewModel.Rows = value;
    }

    public int Columns
    {
        get => _mainViewModel.Columns;
        set => _mainViewModel.Columns = value;
    }

    public int SelectedButtonSize
    {
        get => _mainViewModel.SelectedButtonSize;
        set => _mainViewModel.SelectedButtonSize = value;
    }

    public Dictionary<string, int> ButtonSizeOptions => _mainViewModel.ButtonSizeOptions;
    public List<string> ActionTypes => _mainViewModel.ActionTypes;
    public ObservableCollection<DeckItem> DeckItems => _mainViewModel.DeckItems;
    
    // --- Library Properties (Proxy from MainViewModel) ---
    public ObservableCollection<PresetModel> LibraryItems => _mainViewModel.LibraryItems;
    
    public string LibrarySearchText
    {
        get => _mainViewModel.LibrarySearchText;
        set => _mainViewModel.LibrarySearchText = value;
    }
    
    public string SelectedCategory
    {
        get => _mainViewModel.SelectedCategory;
        set => _mainViewModel.SelectedCategory = value;
    }
    
    public List<string> LibraryCategories => _mainViewModel.LibraryCategories;

    public EditorViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;

        // Close command - notify MainViewModel to clean up
        CloseCommand = new RelayCommand(_ => CloseEditor());

        // Save command - delegate to MainViewModel
        SaveCommand = new RelayCommand(_ => SaveNow());
        
        // Initialize debounce timer (500ms delay)
        _saveTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };
        _saveTimer.Tick += (s, e) =>
        {
            _saveTimer.Stop();
            SaveNow();
        };

        // Subscribe to MainViewModel property changes to update UI
        _mainViewModel.PropertyChanged += (s, e) =>
        {
            // Relay property change notifications
            if (e.PropertyName == nameof(MainViewModel.SelectedDeckItem))
            {
                OnPropertyChanged(nameof(SelectedDeckItem));
                SubscribeToSelectedItemChanges();
            }
            else if (e.PropertyName == nameof(MainViewModel.Rows))
            {
                OnPropertyChanged(nameof(Rows));
                TriggerDebouncedSave();
            }
            else if (e.PropertyName == nameof(MainViewModel.Columns))
            {
                OnPropertyChanged(nameof(Columns));
                TriggerDebouncedSave();
            }
            else if (e.PropertyName == nameof(MainViewModel.SelectedButtonSize))
            {
                OnPropertyChanged(nameof(SelectedButtonSize));
                TriggerDebouncedSave();
            }
        };
        
        SubscribeToSelectedItemChanges();
    }
    
    private void SubscribeToSelectedItemChanges()
    {
        if (SelectedDeckItem != null)
        {
            SelectedDeckItem.PropertyChanged += (s, e) =>
            {
                // Kullanıcı değişiklik yapınca otomatik kaydet tetikle
                TriggerDebouncedSave();
            };
        }
    }
    
    private void TriggerDebouncedSave()
    {
        _hasUnsavedChanges = true;
        SaveStatus = "Kaydediliyor...";
        
        // Timer'ı sıfırla ve yeniden başlat
        _saveTimer?.Stop();
        _saveTimer?.Start();
    }
    
    private void SaveNow()
    {
        if (_hasUnsavedChanges)
        {
            _mainViewModel.SaveChanges();
            _hasUnsavedChanges = false;
            SaveStatus = "✓ Tüm değişiklikler kaydedildi";
            
            // 2 saniye sonra mesajı gizle
            Task.Delay(2000).ContinueWith(_ =>
            {
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    if (!_hasUnsavedChanges)
                        SaveStatus = "";
                });
            });
        }
    }

    /// <summary>
    /// Set the window reference (called by MainViewModel.OpenEditor)
    /// </summary>
    public void SetWindow(Window window)
    {
        _editorWindow = window;
    }

    /// <summary>
    /// Close the editor window
    /// </summary>
    private void CloseEditor()
    {
        _editorWindow?.Close();
        _mainViewModel.OnEditorClosed();
    }
}
