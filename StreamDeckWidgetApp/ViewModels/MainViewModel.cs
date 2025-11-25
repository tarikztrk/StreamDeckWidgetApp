using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Core;
using StreamDeckWidgetApp.Core.Helpers;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly IActionService _actionService;
    private readonly IConfigService _configService;
    private Profile _currentProfile;

    // --- State Properties ---
    
    private bool _isEditMode;
    public bool IsEditMode
    {
        get => _isEditMode;
        set
        {
            if (SetField(ref _isEditMode, value))
            {
                // Mod değişince seçimi sıfırla
                if (!value) SelectedDeckItem = null;
                UpdateWindowSize(); // Panel açılınca pencere büyüsün
            }
        }
    }

    private DeckItem? _selectedDeckItem;
    public DeckItem? SelectedDeckItem
    {
        get => _selectedDeckItem;
        set => SetField(ref _selectedDeckItem, value);
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
                UpdateWindowSize(); // Boyut değişince pencereyi güncelle
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
                UpdateWindowSize(); // Boyut değişince pencereyi güncelle
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
                UpdateWindowSize(); // Boyut değişince pencereyi güncelle
            }
        }
    }

    // Pencere Boyut Özellikleri
    private double _windowWidth;
    public double WindowWidth
    {
        get => _windowWidth;
        set => SetField(ref _windowWidth, value);
    }

    private double _windowHeight;
    public double WindowHeight
    {
        get => _windowHeight;
        set => SetField(ref _windowHeight, value);
    }
    
    // ComboBox için Action Tipleri
    public List<string> ActionTypes { get; } = new() { "Execute", "Hotkey", "Website" };

    public ObservableCollection<DeckItem> DeckItems { get; set; }

    // --- Commands ---
    public ICommand ItemClickCommand { get; }
    public ICommand CloseAppCommand { get; }
    public ICommand ToggleEditModeCommand { get; }
    public ICommand SaveCommand { get; }

    public MainViewModel(IActionService actionService, IConfigService configService)
    {
        _actionService = actionService;
        _configService = configService;
        DeckItems = new ObservableCollection<DeckItem>();
        
        LoadData();

        ItemClickCommand = new RelayCommand(OnItemClick);
        CloseAppCommand = new RelayCommand(_ => 
        {
            SaveChanges();
            Application.Current.Shutdown();
        });
        
        // Edit Modunu A�/Kapat
        ToggleEditModeCommand = new RelayCommand(_ => IsEditMode = !IsEditMode);
        
        // De�i�iklikleri Kaydet
        SaveCommand = new RelayCommand(_ => SaveChanges());
    }

    private void LoadData()
    {
        _currentProfile = _configService.LoadProfile();
        
        // Eğer eski profillerde boyut yoksa varsayılanı ata
        if (_currentProfile.ButtonSize == 0) _currentProfile.ButtonSize = 85;
        
        // Veri yüklendikten sonra Grid'i olması gereken sayıya tamamla
        RefreshGrid();
        UpdateWindowSize(); // İlk yüklemede boyutu hesapla
    }

    // Buton listesini yeni boyutlara g�re ayarlar
    private void RefreshGrid()
    {
        int totalSlots = Rows * Columns;
        
        // Mevcut verileri koru ama g�r�n�m� yeni boyuta ayarla
        DeckItems.Clear();
        
        for (int i = 0; i < totalSlots; i++)
        {
            // E�er kay�tl� veri yetmiyorsa yeni bo� buton olu�tur
            if (i >= _currentProfile.Items.Count)
            {
                _currentProfile.Items.Add(new DeckItem 
                { 
                    Title = "Bo�", 
                    Color = "#222222",
                    Row = i / Columns,
                    Column = i % Columns
                });
            }
            DeckItems.Add(_currentProfile.Items[i]);
        }
    }

    // Pencere boyutunu hesapla
    private void UpdateWindowSize()
    {
        // Temel hesaplama: (Hücre Sayısı * Hücre Boyutu) + Kenar Payları
        double baseWidth = (Columns * SelectedButtonSize) + 30; // 30px kenar payı
        double baseHeight = (Rows * SelectedButtonSize) + 50;   // 50px header + kenar payı

        // Eğer Edit Modu açıksa, sağdaki panel için ekstra yer aç (örn: 220px)
        if (IsEditMode)
        {
            baseWidth += 220; 
        }

        // Minimum boyut kontrolü (Pencere çok küçülüp yok olmasın)
        WindowWidth = Math.Max(baseWidth, 200);
        WindowHeight = Math.Max(baseHeight, 150);
    }

    private void OnItemClick(object? parameter)
    {
        if (parameter is DeckItem item)
        {
            if (IsEditMode)
            {
                // D�zenleme modundaysak: Se�
                SelectedDeckItem = item; 
            }
            else
            {
                // Normal moddaysak: �al��t�r
                _actionService.ExecuteItem(item);
            }
        }
    }

    private void SaveChanges()
    {
        _currentProfile.Items = DeckItems.ToList();
        _configService.SaveProfile(_currentProfile);
        
        if (IsEditMode)
        {
            MessageBox.Show("Ayarlar Kaydedildi!", "Stream Deck", MessageBoxButton.OK, MessageBoxImage.Information);
            IsEditMode = false; // Kaydettikten sonra moddan ��k
        }
    }

    public void HandleFileDrop(DeckItem targetItem, string filePath)
    {
        string ext = System.IO.Path.GetExtension(filePath).ToLower();
        string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

        // 1. E�er resim dosas�ysa
        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".ico")
        {
            targetItem.IconPath = filePath;
            targetItem.Title = ""; // �ste�e ba�l�: Resim at�nca yaz�y� silebilirsiniz
            if (IsEditMode) SelectedDeckItem = targetItem;
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

            if (IsEditMode) SelectedDeckItem = targetItem;
            
            // Otomatik kaydet
            _currentProfile.Items = DeckItems.ToList();
            _configService.SaveProfile(_currentProfile);
        }
    }
}
