using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Controls;
using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Models;
using StreamDeckWidgetApp.ViewModels;

namespace StreamDeckWidgetApp;

public partial class MainWindow : FluentWindow
{
    private readonly IWindowSizingService _windowSizingService;
    private Point _dragStartPoint;
    private bool _isDragging;
    private DeckItem? _draggedItem;

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

    // Drag Start Event Handler
    private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _dragStartPoint = e.GetPosition(null);
        _isDragging = false;
        
        if (sender is FrameworkElement element && element.DataContext is DeckItem item)
        {
            _draggedItem = item;
            System.Diagnostics.Debug.WriteLine($"MouseDown on: {item.Title}");
        }
    }
    
    // Mouse Up Event Handler - Handle click if not dragging
    private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"MouseUp - isDragging={_isDragging}, draggedItem={_draggedItem?.Title ?? "null"}");
        
        if (!_isDragging && _draggedItem != null)
        {
            Point currentPosition = e.GetPosition(null);
            Vector diff = _dragStartPoint - currentPosition;
            
            // If mouse didn't move much, treat it as a click
            if (Math.Abs(diff.X) <= 5 && Math.Abs(diff.Y) <= 5)
            {
                System.Diagnostics.Debug.WriteLine($"Click on: {_draggedItem.Title}");
                
                // Execute the click command
                if (DataContext is MainViewModel vm)
                {
                    vm.ItemClickCommand.Execute(_draggedItem);
                }
            }
        }
        
        _draggedItem = null;
        _isDragging = false;
    }
    
    // Mouse Move Event Handler - Initiate drag after minimum distance
    private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && _draggedItem != null && !_isDragging)
        {
            Point currentPosition = e.GetPosition(null);
            Vector diff = _dragStartPoint - currentPosition;
            
            // Minimum drag distance threshold (5 pixels)
            if (Math.Abs(diff.X) > 5 || Math.Abs(diff.Y) > 5)
            {
                System.Diagnostics.Debug.WriteLine($"Starting drag for: {_draggedItem.Title}");
                _isDragging = true;
                
                // Create drag data
                DataObject dragData = new DataObject("StreamDeckButton", _draggedItem);
                
                // Start drag operation
                DragDrop.DoDragDrop(sender as DependencyObject, dragData, DragDropEffects.Move);
                
                System.Diagnostics.Debug.WriteLine($"Drag completed");
                _isDragging = false;
                _draggedItem = null;
            }
        }
    }
    
    // DragOver Event Handler - Allow drop operation
    private void Button_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent("StreamDeckButton") || 
            e.Data.GetDataPresent("StreamDeckPreset") ||
            e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
    }
    
    // Drag & Drop Event Handler
    private void Button_Drop(object sender, DragEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"Drop event triggered");
        
        // 1. Buton reorder kontrolü (StreamDeckButton format)
        if (e.Data.GetDataPresent("StreamDeckButton"))
        {
            System.Diagnostics.Debug.WriteLine($"StreamDeckButton data present");
            
            if (e.Data.GetData("StreamDeckButton") is DeckItem sourceItem &&
                sender is FrameworkElement element && 
                element.DataContext is DeckItem targetItem &&
                DataContext is MainViewModel vm)
            {
                System.Diagnostics.Debug.WriteLine($"Swapping {sourceItem.Title} with {targetItem.Title}");
                
                // Find indices of source and target items
                var gridService = vm.DeckItems;
                int fromIndex = -1, toIndex = -1;
                
                for (int i = 0; i < gridService.Count; i++)
                {
                    if (gridService[i] == sourceItem) fromIndex = i;
                    if (gridService[i] == targetItem) toIndex = i;
                }
                
                System.Diagnostics.Debug.WriteLine($"Indices: from={fromIndex}, to={toIndex}");
                
                if (fromIndex >= 0 && toIndex >= 0 && fromIndex != toIndex)
                {
                    // Call GridService to swap items
                    vm.SwapDeckItems(fromIndex, toIndex);
                    
                    System.Diagnostics.Debug.WriteLine($"Swap completed");
                    
                    // Save changes
                    vm.SaveChanges();
                }
            }
        }
        // 2. Önce preset kontrolü yap (Özel format)
        else if (e.Data.GetDataPresent("StreamDeckPreset"))
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
        // 3. Sürüklenen şey bir dosya mı?
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

