namespace StreamDeckWidgetApp.Models;

public class Profile
{
    public string Name { get; set; } = "Default Profile";
    public int Rows { get; set; } = 2;
    public int Columns { get; set; } = 3;
    public int ButtonSize { get; set; } = 85; // Varsayılan: Büyük (64px)
    public List<DeckItem> Items { get; set; } = new();
}
