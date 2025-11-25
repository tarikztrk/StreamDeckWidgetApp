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

    // ObservableCollection: Listeye eleman eklenince UI otomatik güncellenir.
    public ObservableCollection<DeckItem> DeckItems { get; set; }

    public ICommand ItemClickCommand { get; }
    public ICommand CloseAppCommand { get; }

    // Constructor Injection: Servisleri dýþarýdan alýyoruz
    public MainViewModel(IActionService actionService, IConfigService configService)
    {
        _actionService = actionService;
        _configService = configService;

        DeckItems = new ObservableCollection<DeckItem>();
        
        // Verileri JSON'dan Yükle
        LoadData();

        ItemClickCommand = new RelayCommand(OnItemClick);
        CloseAppCommand = new RelayCommand(_ => 
        {
            // Kapanmadan önce profili kaydet
            SaveData();
            Application.Current.Shutdown();
        });
    }

    private void LoadData()
    {
        _currentProfile = _configService.LoadProfile();
        
        DeckItems.Clear();
        foreach (var item in _currentProfile.Items)
        {
            DeckItems.Add(item);
        }
    }

    private void SaveData()
    {
        // Observable collection'daki güncel veriyi profile'a aktar
        _currentProfile.Items = DeckItems.ToList();
        _configService.SaveProfile(_currentProfile);
    }

    private void OnItemClick(object? parameter)
    {
        if (parameter is DeckItem item)
        {
            _actionService.ExecuteItem(item);
        }
    }
    
    // Test Amaçlý: Yeni buton ekleyip kaydetmeyi denemek için
    public void AddNewItem()
    {
        var newItem = new DeckItem 
        { 
            Title = "Yeni Buton", 
            Color = "#9C27B0",
            Command = "notepad.exe",
            Row = 1,
            Column = 0
        };
        DeckItems.Add(newItem);
        SaveData();
    }
}
