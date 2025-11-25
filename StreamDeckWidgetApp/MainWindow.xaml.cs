using System.Windows;
using System.Windows.Input;
using StreamDeckWidgetApp.Models;
using StreamDeckWidgetApp.ViewModels;

namespace StreamDeckWidgetApp;

public partial class MainWindow : Window
{
    // Dependency Injection ile ViewModel'i aliyoruz
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel; // DataBinding baglantisi
    }

    // Pencereyi suruklemek icin gerekli (MVVM'de View logic kabul edilir)
    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    // Drag & Drop Event Handler
    private void Button_Drop(object sender, DragEventArgs e)
    {
        // 1. Suruklenen sey bir dosya mi?
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
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
}
