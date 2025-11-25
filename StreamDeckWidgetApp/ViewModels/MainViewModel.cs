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
                // Mod deðiþince seçimi sýfýrla
                if (!value) SelectedDeckItem = null;
            }
        }
    }

    private DeckItem? _selectedDeckItem;
    public DeckItem? SelectedDeckItem
    {
        get => _selectedDeckItem;
        set => SetField(ref _selectedDeckItem, value);
    }
    
    // Grid Boyutlarý - Dinamik Olarak Deðiþtirilebilir
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
        
        // Edit Modunu Aç/Kapat
        ToggleEditModeCommand = new RelayCommand(_ => IsEditMode = !IsEditMode);
        
        // Deðiþiklikleri Kaydet
        SaveCommand = new RelayCommand(_ => SaveChanges());
    }

    private void LoadData()
    {
        _currentProfile = _configService.LoadProfile();
        
        // Veri yüklendikten sonra Grid'i olmasý gereken sayýya tamamla
        RefreshGrid();
    }

    // Buton listesini yeni boyutlara göre ayarlar
    private void RefreshGrid()
    {
        int totalSlots = Rows * Columns;
        
        // Mevcut verileri koru ama görünümü yeni boyuta ayarla
        DeckItems.Clear();
        
        for (int i = 0; i < totalSlots; i++)
        {
            // Eðer kayýtlý veri yetmiyorsa yeni boþ buton oluþtur
            if (i >= _currentProfile.Items.Count)
            {
                _currentProfile.Items.Add(new DeckItem 
                { 
                    Title = "Boþ", 
                    Color = "#222222",
                    Row = i / Columns,
                    Column = i % Columns
                });
            }
            DeckItems.Add(_currentProfile.Items[i]);
        }
    }

    private void OnItemClick(object? parameter)
    {
        if (parameter is DeckItem item)
        {
            if (IsEditMode)
            {
                // Düzenleme modundaysak: Seç
                SelectedDeckItem = item; 
            }
            else
            {
                // Normal moddaysak: Çalýþtýr
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
            IsEditMode = false; // Kaydettikten sonra moddan çýk
        }
    }

    public void HandleFileDrop(DeckItem targetItem, string filePath)
    {
        string ext = System.IO.Path.GetExtension(filePath).ToLower();
        string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

        // 1. Eðer resim dosasýysa
        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".ico")
        {
            targetItem.IconPath = filePath;
            targetItem.Title = ""; // Ýsteðe baðlý: Resim atýnca yazýyý silebilirsiniz
            if (IsEditMode) SelectedDeckItem = targetItem;
            return; 
        }

        // 2. Eðer EXE veya Kýsayol ise (GÜNCELLENDÝ)
        if (ext == ".exe" || ext == ".lnk" || ext == ".bat")
        {
            targetItem.Title = fileName;
            targetItem.Command = filePath;
            targetItem.ActionType = "Execute";

            // --- ÝKON ÇIKARMA ÝÞLEMÝ ---
            
            // Ýkonlarýn kaydedileceði klasör: %AppData%/StreamDeckWidgetApp/CachedIcons
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string iconsFolder = System.IO.Path.Combine(appData, "StreamDeckWidgetApp", "CachedIcons");

            // Helper sýnýfýný çaðýr
            string? newIconPath = IconHelper.ExtractAndSaveIcon(filePath, iconsFolder);

            if (!string.IsNullOrEmpty(newIconPath))
            {
                targetItem.IconPath = newIconPath; // Butonun resmi artýk EXE'nin ikonu!
            }
            
            // ---------------------------

            if (IsEditMode) SelectedDeckItem = targetItem;
            
            // Otomatik kaydet
            _currentProfile.Items = DeckItems.ToList();
            _configService.SaveProfile(_currentProfile);
        }
    }
}
