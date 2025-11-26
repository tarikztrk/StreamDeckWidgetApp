using System.Windows;
using System.Windows.Input;
using StreamDeckWidgetApp.Models;
using StreamDeckWidgetApp.ViewModels;

namespace StreamDeckWidgetApp.Views;

public partial class EditorWindow : Window
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
}
