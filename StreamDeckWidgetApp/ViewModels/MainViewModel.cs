using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Core;
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
        DeckItems.Clear();
        foreach (var item in _currentProfile.Items) DeckItems.Add(item);
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
}
