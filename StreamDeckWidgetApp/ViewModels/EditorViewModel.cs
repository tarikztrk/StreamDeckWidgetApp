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
    public ICommand SaveAndCloseCommand { get; }
    public ICommand ItemClickCommand => _mainViewModel.ItemClickCommand; // Proxy

    // --- Save Status Property ---
    public string SaveStatus
    {
        get => _saveStatus;
        set => SetField(ref _saveStatus, value);
    }
    
    // --- Save Button Text Property ---
    private string _saveButtonText = "💾  KAYDET VE KAPAT";
    public string SaveButtonText
    {
        get => _saveButtonText;
        set => SetField(ref _saveButtonText, value);
    }
    
    private bool _isSaving;
    public bool IsSaving
    {
        get => _isSaving;
        set => SetField(ref _isSaving, value);
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
    
    // --- Profile Properties (Proxy from MainViewModel) ---
    public IReadOnlyList<Profile> Profiles => _mainViewModel.Profiles;
    public Profile CurrentProfile => _mainViewModel.CurrentProfile;
    
    public string CurrentProfileName
    {
        get => _mainViewModel.CurrentProfileName;
        set => _mainViewModel.CurrentProfileName = value;
    }
    
    // Profile Commands
    public ICommand SwitchProfileCommand => _mainViewModel.SwitchProfileCommand;
    public ICommand CreateProfileCommand => _mainViewModel.CreateProfileCommand;
    public ICommand DeleteProfileCommand => _mainViewModel.DeleteProfileCommand;
    public ICommand DuplicateProfileCommand => _mainViewModel.DuplicateProfileCommand;
    
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
    
    // ================================================================
    // ACTION-SPECIFIC COMMAND OPTIONS
    // ================================================================
    
    /// <summary>
    /// MediaControl için kullanılabilir komutlar
    /// </summary>
    public List<ActionCommandOption> MediaControlCommands { get; } = new()
    {
        new("PLAY_PAUSE", "⏯️ Oynat / Duraklat"),
        new("NEXT_TRACK", "⏭️ Sonraki Parça"),
        new("PREV_TRACK", "⏮️ Önceki Parça"),
        new("STOP", "⏹️ Durdur")
    };
    
    /// <summary>
    /// AudioControl için kullanılabilir komutlar
    /// </summary>
    public List<ActionCommandOption> AudioControlCommands { get; } = new()
    {
        new("MUTE", "🔇 Sesi Kapat/Aç"),
        new("VOL_UP", "🔊 Ses Artır (+5)"),
        new("VOL_DOWN", "🔉 Ses Azalt (-5)")
    };
    
    /// <summary>
    /// Hotkey için hazır komutlar
    /// </summary>
    public List<ActionCommandOption> HotkeyPresetCommands { get; } = new()
    {
        // Temel Düzenleme
        new("COPY", "📋 Kopyala (Ctrl+C)"),
        new("PASTE", "📋 Yapıştır (Ctrl+V)"),
        new("CUT", "✂️ Kes (Ctrl+X)"),
        new("UNDO", "↩️ Geri Al (Ctrl+Z)"),
        new("REDO", "↪️ Yinele (Ctrl+Y)"),
        new("SELECT_ALL", "☑️ Tümünü Seç (Ctrl+A)"),
        
        // Dosya İşlemleri
        new("SAVE", "💾 Kaydet (Ctrl+S)"),
        new("NEW", "📄 Yeni (Ctrl+N)"),
        new("OPEN", "📂 Aç (Ctrl+O)"),
        new("PRINT", "🖨️ Yazdır (Ctrl+P)"),
        new("FIND", "🔍 Bul (Ctrl+F)"),
        new("CLOSE", "❌ Sekmeyi Kapat (Ctrl+W)"),
        new("REFRESH", "🔄 Yenile (F5)"),
        
        // Windows Kısayolları
        new("SCREENSHOT", "📸 Ekran Alıntısı (Win+Shift+S)"),
        new("TASK_MANAGER", "📊 Görev Yöneticisi (Ctrl+Shift+Esc)"),
        new("WIN_D", "🖥️ Masaüstü Göster (Win+D)"),
        new("WIN_E", "📁 Dosya Gezgini (Win+E)"),
        new("WIN_L", "🔒 Kilitle (Win+L)"),
        new("ALT_TAB", "🔄 Pencere Değiştir (Alt+Tab)"),
        
        // Ses/Medya
        new("MUTE", "🔇 Sesi Kapat"),
        new("VOL_UP", "🔊 Ses Artır"),
        new("VOL_DOWN", "🔉 Ses Azalt"),
        new("MEDIA_PLAY", "⏯️ Medya Oynat/Durdur"),
        
        // Özel (Kullanıcı girecek)
        new("", "⌨️ Özel Kombinasyon (elle girin)")
    };

    public EditorViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;

        // Close command - notify MainViewModel to clean up
        CloseCommand = new RelayCommand(_ => CloseEditor());

        // Save and Close command - save, show feedback, then close
        SaveAndCloseCommand = new RelayCommand(_ => SaveAndClose());
        
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
            else if (e.PropertyName == nameof(MainViewModel.CurrentProfile) ||
                     e.PropertyName == nameof(MainViewModel.CurrentProfileName))
            {
                OnPropertyChanged(nameof(CurrentProfile));
                OnPropertyChanged(nameof(CurrentProfileName));
                OnPropertyChanged(nameof(Profiles));
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
    /// Save changes, show feedback, then close the editor
    /// </summary>
    private async void SaveAndClose()
    {
        if (IsSaving) return;
        IsSaving = true;
        
        // Kaydet
        _mainViewModel.SaveChanges();
        _hasUnsavedChanges = false;
        
        // Buton feedback
        SaveButtonText = "✓ Kaydedildi!";
        SaveStatus = "✓ Tüm değişiklikler kaydedildi";
        
        // Kısa gecikme sonra kapat (kullanıcı feedback'i görsün)
        await Task.Delay(600);
        
        // Pencereyi kapat
        IsSaving = false;
        CloseEditor();
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
    }
}
