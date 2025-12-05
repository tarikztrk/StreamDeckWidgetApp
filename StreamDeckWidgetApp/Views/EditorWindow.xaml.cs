using System.Windows;
using System.Windows.Input;
using StreamDeckWidgetApp.Models;
using StreamDeckWidgetApp.ViewModels;
using Wpf.Ui.Controls;

namespace StreamDeckWidgetApp.Views;

public partial class EditorWindow : FluentWindow
{
    private Point _dragStartPoint;
    private bool _isDragging = false;

    public EditorWindow(EditorViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    /// <summary>
    /// Kütüphane listesinden preset sürükleme başlangıcı
    /// </summary>
    private void PresetList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _dragStartPoint = e.GetPosition(null);
        _isDragging = false;
    }

    /// <summary>
    /// Mouse hareket ettiğinde drag işlemini başlat
    /// </summary>
    private void PresetList_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
        {
            Point currentPosition = e.GetPosition(null);
            Vector diff = _dragStartPoint - currentPosition;

            // Minimum hareket eşiği
            if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                // Hangi preset seçili?
                if (sender is System.Windows.Controls.ListBox listBox &&
                    listBox.SelectedItem is PresetModel preset)
                {
                    _isDragging = true;

                    // Veriyi paketle
                    DataObject dragData = new DataObject("StreamDeckPreset", preset);
                    
                    // Drag işlemini başlat
                    DragDrop.DoDragDrop(listBox, dragData, DragDropEffects.Copy);
                    
                    _isDragging = false;
                }
            }
        }
    }

    /// <summary>
    /// Sanal grid üzerine drop (Preset veya Swap)
    /// </summary>
    private void PreviewGrid_Drop(object sender, DragEventArgs e)
    {
        if (DataContext is not EditorViewModel vm) return;
        if (sender is not System.Windows.FrameworkElement element) return;
        if (element.DataContext is not DeckItem targetItem) return;

        // 1. Preset sürükleme
        if (e.Data.GetDataPresent("StreamDeckPreset"))
        {
            if (e.Data.GetData("StreamDeckPreset") is PresetModel preset)
            {
                var deckItem = preset.ToDeckItem();
                targetItem.Title = deckItem.Title;
                targetItem.ActionType = deckItem.ActionType;
                targetItem.Command = deckItem.Command;
                targetItem.Color = deckItem.Color;
                targetItem.BehaviorType = deckItem.BehaviorType;
                targetItem.Icon = deckItem.Icon; // Segoe MDL2 veya Emoji ikonu kopyala
                targetItem.IconPath = deckItem.IconPath; // Özel dosya yolu (varsa)

                // Seçili yap
                vm.SelectedDeckItem = targetItem;
            }
        }
        // 2. İleride: Buton swap mantığı buraya eklenecek
    }

    /// <summary>
    /// Drag over için görsel geri bildirim
    /// </summary>
    private void PreviewGrid_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent("StreamDeckPreset"))
        {
            e.Effects = DragDropEffects.Copy;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
        e.Handled = true;
    }
    
    /// <summary>
    /// Hızlı ikon butonlarına tıklama
    /// </summary>
    private void QuickIcon_Click(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.Button btn && 
            btn.Tag is string iconCode &&
            DataContext is EditorViewModel vm &&
            vm.SelectedDeckItem is DeckItem item)
        {
            item.Icon = iconCode;
            item.IconPath = null; // Dosya ikonunu temizle
        }
    }
    
    /// <summary>
    /// İkon önizlemesine resim dosyası sürükleme
    /// </summary>
    private void IconPreview_Drop(object sender, DragEventArgs e)
    {
        if (DataContext is not EditorViewModel vm) return;
        if (vm.SelectedDeckItem is not DeckItem item) return;
        
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files?.Length > 0)
            {
                var file = files[0];
                var ext = System.IO.Path.GetExtension(file).ToLower();
                
                // Resim dosyası mı kontrol et
                if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".ico" || ext == ".bmp")
                {
                    item.IconPath = file;
                    item.Icon = null; // Segoe MDL2 ikonunu temizle
                }
            }
        }
        
        e.Handled = true;
    }
}
