namespace StreamDeckWidgetApp.Models;

public class DeckItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); // Benzersiz ID
    public string Title { get; set; } = "Yeni Buton";
    public string ActionType { get; set; } = "Execute"; // Örn: Execute, Hotkey, Website
    public string Command { get; set; } = ""; // Exe yolu veya URL
    public string Color { get; set; } = "#333333"; // Buton rengi
    
    // Grid Konumu
    public int Row { get; set; }
    public int Column { get; set; }
}
