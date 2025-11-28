namespace StreamDeckWidgetApp.Models;

/// <summary>
/// Eylem tiplerine özel komut seçenekleri için model
/// </summary>
public class ActionCommandOption
{
    /// <summary>
    /// Gerçek komut değeri (JSON'a kaydedilecek)
    /// </summary>
    public string Value { get; set; }
    
    /// <summary>
    /// Kullanıcıya gösterilecek açıklama
    /// </summary>
    public string DisplayText { get; set; }
    
    public ActionCommandOption(string value, string displayText)
    {
        Value = value;
        DisplayText = displayText;
    }
    
    // ComboBox'ta doğru gösterim için
    public override string ToString() => DisplayText;
}
