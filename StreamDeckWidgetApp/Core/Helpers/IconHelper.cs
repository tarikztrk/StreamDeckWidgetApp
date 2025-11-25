using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StreamDeckWidgetApp.Core.Helpers;

public static class IconHelper
{
    public static string? ExtractAndSaveIcon(string sourceFilePath, string saveFolder)
    {
        try
        {
            // Klasör yoksa oluþtur
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            // Windows API kullanarak ikonunu çek (System.Drawing.Common kullanmadan)
            var icon = System.Drawing.Icon.ExtractAssociatedIcon(sourceFilePath);
            
            if (icon == null) return null;

            // Ýkonu stream'e çevir
            using var memoryStream = new MemoryStream();
            icon.Save(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            // WPF BitmapImage oluþtur
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            // PNG encoder ile kaydet
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

            // Benzersiz bir isim oluþtur (Örn: chrome_12345.png)
            string fileName = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}_{Guid.NewGuid().ToString()[..5]}.png";
            string fullPath = Path.Combine(saveFolder, fileName);

            // PNG olarak kaydet
            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }

            return fullPath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ýkon Hatasý: {ex.Message}");
            return null;
        }
    }
}
