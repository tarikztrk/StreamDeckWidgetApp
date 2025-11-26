using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Core;
using StreamDeckWidgetApp.Core.Helpers;
using StreamDeckWidgetApp.Models;
using StreamDeckWidgetApp.Services;
using StreamDeckWidgetApp.Views;

namespace StreamDeckWidgetApp.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly IActionService _actionService;
    private readonly IConfigService _configService;
    private readonly Func<EditorViewModel> _editorViewModelFactory;
    private readonly Func<EditorWindow> _editorWindowFactory;
    private Profile _currentProfile = null!;

    // --- State Properties ---
    
    // Editör penceresinin referansı (Aynı anda sadece 1 tane açık olsun)
    private EditorWindow? _editorWindow;

    // Editör açık mı? (Widget butonlarına basınca işlem mi yapsın, seçim mi?)
    public bool IsEditorOpen => _editorWindow != null;

    private DeckItem? _selectedDeckItem;
    public DeckItem? SelectedDeckItem
    {
        get => _selectedDeckItem;
        set
        {
            // Önceki seçimi temizle
            if (_selectedDeckItem != null)
                _selectedDeckItem.IsSelected = false;
            
            SetField(ref _selectedDeckItem, value);
            
            // Yeni seçimi işaretle
            if (_selectedDeckItem != null)
                _selectedDeckItem.IsSelected = true;
        }
    }
    
    // Grid Boyutları - Dinamik Olarak Değiştirilebilir
    public int Rows
    {
        get => _currentProfile.Rows;
        set
        {
            if (_currentProfile.Rows != value && value > 0)
            {
                _currentProfile.Rows = value;
                OnPropertyChanged();
                RefreshGrid(); // Grid boyutunu yeniden hesapla
            }
        }
    }

    public int Columns
    {
        get => _currentProfile.Columns;
        set
        {
            if (_currentProfile.Columns != value && value > 0)
            {
                _currentProfile.Columns = value;
                OnPropertyChanged();
                RefreshGrid(); // Grid boyutunu yeniden hesapla
            }
        }
    }
    
    // Seçenekler (Display Name -> Pixel Size)
    public Dictionary<string, int> ButtonSizeOptions { get; } = new()
    {
        { "Küçük (32px)", 45 }, // 32px buton + boşluk payı
        { "Orta (48px)", 65 },  // 48px buton + boşluk payı
        { "Büyük (64px)", 85 }  // 64px buton + boşluk payı
    };

    // Aktif Buton Boyutu
    public int SelectedButtonSize
    {
        get => _currentProfile.ButtonSize;
        set
        {
            if (_currentProfile.ButtonSize != value)
            {
                _currentProfile.ButtonSize = value;
                OnPropertyChanged();
            }
        }
    }

    // ComboBox için Action Tipleri
    public List<string> ActionTypes { get; } = new() { "Execute", "Hotkey", "Website" };

    public ObservableCollection<DeckItem> DeckItems { get; set; }
    
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

    // --- Commands ---
    public ICommand ItemClickCommand { get; }
    public ICommand CloseAppCommand { get; }
    public ICommand OpenEditorCommand { get; }
    public ICommand SaveCommand { get; }

    public MainViewModel(
        IActionService actionService, 
        IConfigService configService,
        Func<EditorViewModel> editorViewModelFactory,
        Func<EditorWindow> editorWindowFactory)
    {
        _actionService = actionService;
        _configService = configService;
        _editorViewModelFactory = editorViewModelFactory;
        _editorWindowFactory = editorWindowFactory;
        DeckItems = new ObservableCollection<DeckItem>();
        _libraryItems = new ObservableCollection<PresetModel>();
        
        // Kütüphane kategorilerini yükle
        LibraryCategories.AddRange(PresetService.GetCategories());
        
        LoadData();

        ItemClickCommand = new RelayCommand(OnItemClick);
        CloseAppCommand = new RelayCommand(_ => 
        {
            SaveChanges();
            Application.Current.Shutdown();
        });
        
        // Editör Penceresini Aç
        OpenEditorCommand = new RelayCommand(_ => OpenEditor());
        
        // Değişiklikleri Kaydet
        SaveCommand = new RelayCommand(_ => SaveChanges());
        
        // Kütüphaneyi ilk yükle
        LoadLibrary();
    }

    private void LoadData()
    {
        _currentProfile = _configService.LoadProfile();
        
        // Eğer eski profillerde boyut yoksa varsayılanı ata
        if (_currentProfile.ButtonSize == 0) _currentProfile.ButtonSize = 85;
        
        // Veri yüklendikten sonra Grid'i olması gereken sayıya tamamla
        RefreshGrid();
    }

    // Buton listesini yeni boyutlara g�re ayarlar
    private void RefreshGrid()
    {
        int totalSlots = Rows * Columns;
        
        // Mevcut verileri koru ama g�r�n�m� yeni boyuta ayarla
        DeckItems.Clear();
        
        for (int i = 0; i < totalSlots; i++)
        {
            // Eğer kayıtlı veri yetmiyorsa yeni boş buton oluştur
            if (i >= _currentProfile.Items.Count)
            {
                _currentProfile.Items.Add(new DeckItem
                {
                    Title = "Boş",
                    Color = "#222222",
                    Row = i / Columns,
                    Column = i % Columns
                });
            }

            // Her elemanın satır/sütun bilgisini güncelle - tutarlılık için önemli
            var item = _currentProfile.Items[i];
            item.Row = i / Columns;
            item.Column = i % Columns;

            DeckItems.Add(item);
        }
    }



    private void OnItemClick(object? parameter)
    {
        if (parameter is DeckItem item)
        {
            if (IsEditorOpen) // Editör açıksa SEÇ (Toggle durumu değişmez!)
            {
                SelectedDeckItem = item;
            }
            else // Kapalıysa ÇALIŞTIR
            {
                // Toggle buton ise durumu değiştir
                if (item.BehaviorType == "Toggle")
                {
                    item.IsActive = !item.IsActive;
                }
                
                // Aksiyonu çalıştır
                _actionService.ExecuteItem(item);
            }
        }
    }

    private void OpenEditor()
    {
        // Eğer zaten açıksa öne getir
        if (_editorWindow != null)
        {
            _editorWindow.Activate();
            return;
        }

        // DI ile EditorWindow ve EditorViewModel oluştur
        _editorWindow = _editorWindowFactory();
        var editorViewModel = _editorViewModelFactory();
        
        // EditorViewModel'e window referansını ver
        editorViewModel.SetWindow(_editorWindow);
        _editorWindow.DataContext = editorViewModel;
        
        // Pencere kapanınca referansı temizle
        _editorWindow.Closed += (s, e) => OnEditorClosed();

        _editorWindow.Show();
        OnPropertyChanged(nameof(IsEditorOpen));
    }

    /// <summary>
    /// Called by EditorViewModel when editor window closes
    /// </summary>
    public void OnEditorClosed()
    {
        _editorWindow = null;
        SelectedDeckItem = null;
        OnPropertyChanged(nameof(IsEditorOpen));
    }

    public void SaveChanges()
    {
        _currentProfile.Items = DeckItems.ToList();
        _configService.SaveProfile(_currentProfile);
    }
    
    public void SaveAndCloseEditor()
    {
        SaveChanges();
        // Editör penceresini kapat
        _editorWindow?.Close();
    }

    public void HandleFileDrop(DeckItem targetItem, string filePath)
    {
        string ext = System.IO.Path.GetExtension(filePath).ToLower();
        string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

        // 1. E�er resim dosas�ysa
        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".ico")
        {
            targetItem.IconPath = filePath;
            targetItem.Title = ""; // İsteğe bağlı: Resim atınca yazıyı silebilirsiniz
            if (IsEditorOpen) SelectedDeckItem = targetItem;
            return; 
        }

        // 2. E�er EXE veya K�sayol ise (G�NCELLEND�)
        if (ext == ".exe" || ext == ".lnk" || ext == ".bat")
        {
            targetItem.Title = fileName;
            targetItem.Command = filePath;
            targetItem.ActionType = "Execute";

            // --- �KON �IKARMA ��LEM� ---
            
            // �konlar�n kaydedilece�i klas�r: %AppData%/StreamDeckWidgetApp/CachedIcons
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string iconsFolder = System.IO.Path.Combine(appData, "StreamDeckWidgetApp", "CachedIcons");

            // Helper s�n�f�n� �a��r
            string? newIconPath = IconHelper.ExtractAndSaveIcon(filePath, iconsFolder);

            if (!string.IsNullOrEmpty(newIconPath))
            {
                targetItem.IconPath = newIconPath; // Butonun resmi art�k EXE'nin ikonu!
            }
            
            // ---------------------------

            if (IsEditorOpen) SelectedDeckItem = targetItem;
            
            // Otomatik kaydet
            _currentProfile.Items = DeckItems.ToList();
            _configService.SaveProfile(_currentProfile);
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
        
        // Kategori filtresi
        if (SelectedCategory != "Tümü")
        {
            allPresets = allPresets.Where(p => p.Category == SelectedCategory).ToList();
        }
        
        // Arama filtresi
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
    /// Preset'i seçili butona uygula
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
