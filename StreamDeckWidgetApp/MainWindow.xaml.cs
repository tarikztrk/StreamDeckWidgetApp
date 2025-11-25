using System.Windows;
using System.Windows.Input;
using StreamDeckWidgetApp.ViewModels;

namespace StreamDeckWidgetApp;

public partial class MainWindow : Window
{
    // Dependency Injection ile ViewModel'i alýyoruz
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel; // DataBinding baðlantýsý
    }

    // Pencereyi sürüklemek için gerekli (MVVM'de View logic kabul edilir)
    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }
}
