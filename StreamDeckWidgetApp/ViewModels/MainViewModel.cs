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

    // ObservableCollection: Listeye eleman eklenince UI otomatik güncellenir.
    public ObservableCollection<DeckItem> DeckItems { get; set; }

    public ICommand ItemClickCommand { get; }
    public ICommand CloseAppCommand { get; }

    // Constructor Injection: Servisi dýþarýdan alýyoruz (Test edilebilirlik için önemli)
    public MainViewModel(IActionService actionService)
    {
        _actionService = actionService;
        
        DeckItems = new ObservableCollection<DeckItem>();
        LoadDummyData(); // Þimdilik test verisi yüklüyoruz

        ItemClickCommand = new RelayCommand(OnItemClick);
        CloseAppCommand = new RelayCommand(_ => Application.Current.Shutdown());
    }

    private void OnItemClick(object? parameter)
    {
        if (parameter is DeckItem item)
        {
            _actionService.ExecuteItem(item);
        }
    }

    private void LoadDummyData()
    {
        DeckItems.Add(new DeckItem { Title = "Hesap Mak.", Command = "calc.exe", Color = "#4CAF50" });
        DeckItems.Add(new DeckItem { Title = "Notepad", Command = "notepad.exe", Color = "#2196F3" });
        DeckItems.Add(new DeckItem { Title = "Google", Command = "https://google.com", Color = "#FF9800" });
        DeckItems.Add(new DeckItem { Title = "CMD", Command = "cmd.exe", Color = "#607D8B" });
    }
}
