namespace StreamDeckWidgetApp.Services;

/// <summary>
/// Segoe MDL2 Assets ikon kütüphanesi
/// </summary>
public static class IconService
{
    /// <summary>
    /// Kategorize edilmiş popüler ikonlar
    /// </summary>
    public static Dictionary<string, List<IconItem>> GetIconsByCategory()
    {
        return new Dictionary<string, List<IconItem>>
        {
            ["Sistem"] = new List<IconItem>
            {
                new("\uE7E8", "Power"),
                new("\uE777", "Refresh"),
                new("\uE708", "Moon"),
                new("\uE72E", "Lock"),
                new("\uE74D", "Delete"),
                new("\uE713", "Settings"),
                new("\uE710", "Edit"),
                new("\uE711", "Add"),
                new("\uE74E", "Cancel"),
                new("\uE73E", "Checkmark"),
            },
            ["Medya"] = new List<IconItem>
            {
                new("\uE768", "Play"),
                new("\uE769", "Pause"),
                new("\uE71A", "Stop"),
                new("\uE893", "Next"),
                new("\uE892", "Previous"),
                new("\uE767", "FastForward"),
                new("\uE74F", "Mute"),
                new("\uE995", "Volume"),
                new("\uE992", "VolumeLow"),
                new("\uE8D6", "Music"),
            },
            ["Uygulama"] = new List<IconItem>
            {
                new("\uE8EF", "Calculator"),
                new("\uE70F", "Document"),
                new("\uE771", "Color"),
                new("\uE7EE", "System"),
                new("\uE756", "CommandPrompt"),
                new("\uE8B7", "Folder"),
                new("\uE8F1", "Camera"),
                new("\uE714", "Video"),
                new("\uE717", "Phone"),
                new("\uE715", "Mail"),
            },
            ["Web"] = new List<IconItem>
            {
                new("\uE721", "Globe"),
                new("\uE71B", "Search"),
                new("\uE72D", "Home"),
                new("\uE774", "Link"),
                new("\uE8BD", "Message"),
                new("\uE8B3", "Stream"),
                new("\uE753", "Share"),
                new("\uE8F2", "Robot"),
                new("\uE8D4", "People"),
                new("\uE716", "Contact"),
            },
            ["Klavye"] = new List<IconItem>
            {
                new("\uE8C8", "Copy"),
                new("\uE77F", "Paste"),
                new("\uE8AC", "Edit"),
                new("\uE7B7", "Screenshot"),
                new("\uE765", "Keyboard"),
                new("\uE80F", "Map"),
                new("\uE81D", "Clock"),
                new("\uE787", "Favorite"),
                new("\uE735", "Star"),
                new("\uE8E1", "Heart"),
            },
            ["Oklar"] = new List<IconItem>
            {
                new("\uE72B", "Up"),
                new("\uE72C", "Down"),
                new("\uE72A", "Left"),
                new("\uE72D", "Right"),
                new("\uE711", "Plus"),
                new("\uE738", "Minus"),
                new("\uE76C", "ExpandUp"),
                new("\uE76B", "ExpandDown"),
                new("\uE700", "Previous"),
                new("\uE701", "Next"),
            }
        };
    }

    /// <summary>
    /// Tüm ikonları düz liste olarak döndür
    /// </summary>
    public static List<IconItem> GetAllIcons()
    {
        var all = new List<IconItem>();
        foreach (var category in GetIconsByCategory().Values)
        {
            all.AddRange(category);
        }
        return all;
    }

    /// <summary>
    /// Tüm kategori isimlerini döndür
    /// </summary>
    public static List<string> GetCategories()
    {
        return GetIconsByCategory().Keys.ToList();
    }
}

/// <summary>
/// İkon bilgisi
/// </summary>
public class IconItem
{
    public string Code { get; set; }
    public string Name { get; set; }

    public IconItem(string code, string name)
    {
        Code = code;
        Name = name;
    }
}
