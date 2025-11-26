using System.Windows;
using StreamDeckWidgetApp.ViewModels;

namespace StreamDeckWidgetApp.Views;

public partial class EditorWindow : Window
{
    public EditorWindow(EditorViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
