namespace StreamDeckWidgetApp.Models;

public class Profile
{
    public string Name { get; set; } = "Default Profile";
    public int Rows { get; set; } = 2;
    public int Columns { get; set; } = 3;
    public List<DeckItem> Items { get; set; } = new();
}
