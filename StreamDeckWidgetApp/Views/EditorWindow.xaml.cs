using System.Windows;
using System.Windows.Input;

namespace StreamDeckWidgetApp.Views;

public partial class EditorWindow : Window
{
    public EditorWindow()
    {
        InitializeComponent();
    }

    // Pencereyi sürükleyebilmek için (Çünkü WindowStyle=None yaptık)
    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            this.DragMove();
    }

    // Özel X butonu için
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
