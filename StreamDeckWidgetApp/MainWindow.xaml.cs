using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Controls;
using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Models;
using StreamDeckWidgetApp.ViewModels;

namespace StreamDeckWidgetApp;

public partial class MainWindow : FluentWindow
{
    private readonly IWindowSizingService _windowSizingService;

    // Dependency Injection ile ViewModel ve servisler
    public MainWindow(MainViewModel viewModel, IWindowSizingService windowSizingService)
    {
        InitializeComponent();
        DataContext = viewModel;
        _windowSizingService = windowSizingService;
        
        // ViewModel'e MainWindow referansını ver (Modal mod için)
        viewModel.SetMainWindow(this);
        
        // İlk boyutu hemen hesapla
        UpdateWindowSize(viewModel);
        
        // Grid değişikliğinde güncelle
        viewModel.GridSizeChanged += () => UpdateWindowSize(viewModel);
        
        // İlk yüklemede layout sorununu önlemek için
        Loaded += MainWindow_Loaded;
    }
    
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Layout'u zorla güncelle - ilk açılışta boşluk sorununu düzeltir
        Dispatcher.InvokeAsync(() =>
        {
            InvalidateMeasure();
            InvalidateArrange();
            UpdateLayout();
            
            // ViewModel'e de bildir
            if (DataContext is MainViewModel vm)
            {
                vm.ForceLayoutRefresh();
                // Layout sonrası tekrar boyut kontrol
                UpdateWindowSize(vm);
            }
        }, System.Windows.Threading.DispatcherPriority.Loaded);
    }
    
    private void UpdateWindowSize(MainViewModel vm)
    {
        // Delegate sizing calculation to WindowSizingService (SRP)
        var (width, height) = _windowSizingService.CalculateWindowSize(
            vm.Rows,
            vm.Columns,
            vm.SelectedButtonSize);
        
        // Min = Max yaparak resize'ı engelle
        MinWidth = MaxWidth = Width = width;
        MinHeight = MaxHeight = Height = height;
    }

    // Drag & Drop Event Handler
    private void Button_Drop(object sender, DragEventArgs e)
    {
        // 1. Önce preset kontrolü yap (Özel format)
        if (e.Data.GetDataPresent("StreamDeckPreset"))
        {
            // Kütüphaneden sürüklenen preset
            if (e.Data.GetData("StreamDeckPreset") is PresetModel preset)
            {
                // Hangi butona bırakıldı?
                if (sender is FrameworkElement element && element.DataContext is DeckItem targetItem)
                {
                    // Preset verilerini butona uygula
                    var deckItem = preset.ToDeckItem();
                    targetItem.Title = deckItem.Title;
                    targetItem.ActionType = deckItem.ActionType;
                    targetItem.Command = deckItem.Command;
                    targetItem.Color = deckItem.Color;
                    targetItem.BehaviorType = deckItem.BehaviorType;

                    // ViewModel'e bildir (seçili buton olarak işaretle)
                    if (DataContext is MainViewModel vm)
                    {
                        if (vm.IsEditorOpen)
                        {
                            vm.SelectedDeckItem = targetItem;
                        }
                    }
                }
            }
        }
        // 2. Sürüklenen şey bir dosya mı?
        else if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            // Dosya yollarini dizi olarak al (Birden fazla dosya suruklenebilir, biz ilkini alacagiz)
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null && files.Length > 0)
            {
                string droppedFile = files[0];

                // 2. Hangi butona birakildi? (DataContext uzerinden Model'i buluyoruz)
                if (sender is FrameworkElement element && element.DataContext is DeckItem targetItem)
                {
                    // 3. ViewModel'e isi devret
                    if (DataContext is MainViewModel vm)
                    {
                        vm.HandleFileDrop(targetItem, droppedFile);
                    }
                }
            }
        }
    }
    // Make the window topmost only while mouse is over it so desktop icons can be clicked
    private void FluentWindow_MouseEnter(object sender, MouseEventArgs e)
    {
        this.Topmost = true;
    }

    private void FluentWindow_MouseLeave(object sender, MouseEventArgs e)
    {
        // Lower topmost so user can interact with desktop behind the widget
        this.Topmost = false;
    }
}

