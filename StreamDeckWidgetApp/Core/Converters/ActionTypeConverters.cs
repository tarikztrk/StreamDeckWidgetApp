using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StreamDeckWidgetApp.Core.Converters;

/// <summary>
/// ActionType'a göre kullanıcıya yardım metni döndürür
/// </summary>
public class ActionTypeToHelpTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var actionType = value as string ?? "";
        
        return actionType switch
        {
            "Execute" => "💡 Örnek: calc.exe, notepad.exe, C:\\app.exe",
            "Website" => "🌐 Örnek: https://google.com, https://youtube.com",
            "Hotkey" => "⌨️ Hazır komut seçin veya özel kombinasyon girin",
            "MediaControl" => "🎵 Aşağıdan bir medya komutu seçin",
            "AudioControl" => "🔊 Aşağıdan bir ses komutu seçin",
            "TextType" => "📝 Yazılacak metni girin (e-posta, adres, imza vb.)",
            _ => "Komut veya dosya yolu girin"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}

/// <summary>
/// ActionType'a göre komut placeholder metni döndürür
/// </summary>
public class ActionTypeToPlaceholderConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var actionType = value as string ?? "";
        
        return actionType switch
        {
            "Execute" => "Program yolu (örn: calc.exe)",
            "Website" => "Web adresi (örn: https://google.com)",
            "Hotkey" => "Kısayol komutu (örn: CTRL+ALT+T)",
            "MediaControl" => "Medya komutu (örn: PLAY_PAUSE)",
            "AudioControl" => "Ses komutu (örn: MUTE)",
            "TextType" => "Yazılacak metin...",
            _ => "Komut girin..."
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}

/// <summary>
/// ActionType'a göre detaylı komut listesi döndürür
/// </summary>
public class ActionTypeToCommandListConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var actionType = value as string ?? "";
        
        return actionType switch
        {
            "Execute" => "• Program çalıştırma: calc.exe, notepad.exe\n• Tam yol: C:\\Program Files\\app.exe\n• Parametre ile: cmd.exe /c dir",
            
            "Website" => "• Web sitesi: https://google.com\n• URL protokolü: spotify:, discord:",
            
            "Hotkey" => "🔧 Özel Kombinasyon Formatı:\n" +
                        "• CTRL+SHIFT+N\n" +
                        "• ALT+F4\n" +
                        "• WIN+R\n\n" +
                        "🎹 Desteklenen Tuşlar:\n" +
                        "• F1-F24, ESC, ENTER, SPACE\n" +
                        "• TAB, DELETE, HOME, END",
            
            "MediaControl" => "Seçilen komut medya uygulamalarını kontrol eder (Spotify, YouTube, VLC vb.)",
            
            "AudioControl" => "Seçilen komut sistem ses seviyesini kontrol eder",
            
            "TextType" => "Girilen metin klavye ile otomatik yazılır.\n\n• E-posta: ornek@email.com\n• İmza: Saygılarımla, Ad Soyad",
            
            _ => "Komut veya dosya yolu girin"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}

/// <summary>
/// ActionType'a göre hangi panelin görüneceğini belirler
/// Parameter: Hangi panel için kontrol yapılacağı (Execute, Website, Hotkey, MediaControl, AudioControl, TextType)
/// </summary>
public class ActionTypeToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var actionType = value as string ?? "";
        var targetPanel = parameter as string ?? "";
        
        // Eşleşme varsa Visible, yoksa Collapsed
        return actionType == targetPanel ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}

/// <summary>
/// ActionType'ın basit TextBox gerektirip gerektirmediğini kontrol eder
/// Execute ve Website için true döner
/// </summary>
public class ActionTypeToSimpleInputVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var actionType = value as string ?? "";
        
        // Execute ve Website basit TextBox kullanır
        return (actionType == "Execute" || actionType == "Website") 
            ? Visibility.Visible 
            : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}
