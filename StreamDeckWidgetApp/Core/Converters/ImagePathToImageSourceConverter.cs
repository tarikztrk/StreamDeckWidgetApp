using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace StreamDeckWidgetApp.Core.Converters;

public class ImagePathToImageSourceConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string? path = value as string;

        if (string.IsNullOrEmpty(path)) return null;
        if (!File.Exists(path)) return null;

        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad; // Resmi hafýzaya al ve dosyayý serbest býrak
            bitmap.UriSource = new Uri(path, UriKind.Absolute);
            bitmap.EndInit();
            bitmap.Freeze(); // UI Thread dýþýnda da eriþilebilmesi için dondur
            return bitmap;
        }
        catch
        {
            return null; // Resim bozuksa veya yüklenemezse boþ dön
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
