namespace StreamDeckWidgetApp.Models;

public class DeckItem
{
    public string Title { get; set; } = string.Empty;
    public string ActionType { get; set; } = "Execute"; // Örn: Execute, Hotkey, Website
    public string Command { get; set; } = string.Empty; // Exe yolu veya URL
    public string Color { get; set; } = "#333333"; // Buton rengi
}
