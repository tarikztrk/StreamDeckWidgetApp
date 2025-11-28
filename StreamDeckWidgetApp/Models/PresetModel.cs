namespace StreamDeckWidgetApp.Models;

/// <summary>
/// Hazır şablon kütüphanesindeki preset tanımı
/// </summary>
public class PresetModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty; // Segoe MDL2 Assets veya Emoji
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Bu preset'i DeckItem'a dönüştür
    /// </summary>
    public DeckItem ToDeckItem()
    {
        return new DeckItem
        {
            Id = Guid.NewGuid().ToString(),
            Title = Name,
            ActionType = ActionType,
            Command = Command,
            Color = Color,
            Icon = Icon, // Segoe MDL2 veya Emoji ikonunu kopyala
            IconPath = null, // Özel ikon dosyası için
            BehaviorType = "Push"
        };
    }
}
