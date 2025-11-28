namespace StreamDeckWidgetApp.Models;

public class Profile
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "Default Profile";
    public int Rows { get; set; } = 2;
    public int Columns { get; set; } = 3;
    public int ButtonSize { get; set; } = 85; // Varsayılan: Büyük (64px)
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime ModifiedAt { get; set; } = DateTime.Now;
    public List<DeckItem> Items { get; set; } = new();
}

/// <summary>
/// Tüm profilleri ve ayarları tutan ana konfigürasyon
/// </summary>
public class AppConfig
{
    public string Version { get; set; } = "1.0";
    public string ActiveProfileId { get; set; } = string.Empty;
    public List<Profile> Profiles { get; set; } = new();
}
