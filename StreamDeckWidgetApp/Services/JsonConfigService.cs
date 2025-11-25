using System.IO;
using System.Text.Json;
using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Services;

public class JsonConfigService : IConfigService
{
    // Dosya Yolu: C:\Users\Kullanici\AppData\Roaming\StreamDeckWidgetApp\config.json
    private readonly string _configFolder;
    private readonly string _configFile;

    public JsonConfigService()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _configFolder = Path.Combine(appData, "StreamDeckWidgetApp");
        _configFile = Path.Combine(_configFolder, "config.json");
    }

    public Profile LoadProfile()
    {
        // Klasör yoksa oluþtur
        if (!Directory.Exists(_configFolder))
            Directory.CreateDirectory(_configFolder);

        // Dosya yoksa varsayýlan bir profil oluþtur ve kaydet
        if (!File.Exists(_configFile))
        {
            var defaultProfile = CreateDefaultProfile();
            SaveProfile(defaultProfile);
            return defaultProfile;
        }

        try
        {
            string json = File.ReadAllText(_configFile);
            return JsonSerializer.Deserialize<Profile>(json) ?? CreateDefaultProfile();
        }
        catch
        {
            return CreateDefaultProfile();
        }
    }

    public void SaveProfile(Profile profile)
    {
        var options = new JsonSerializerOptions { WriteIndented = true }; // Okunabilir JSON
        string json = JsonSerializer.Serialize(profile, options);
        File.WriteAllText(_configFile, json);
    }

    private Profile CreateDefaultProfile()
    {
        var profile = new Profile { Rows = 2, Columns = 3 };
        
        // Varsayýlan Örnek Veriler
        profile.Items.Add(new DeckItem { Title = "Hesap Mak.", Command = "calc.exe", Color = "#E91E63", Row = 0, Column = 0 });
        profile.Items.Add(new DeckItem { Title = "Notepad", Command = "notepad.exe", Color = "#2196F3", Row = 0, Column = 1 });
        profile.Items.Add(new DeckItem { Title = "Google", Command = "https://google.com", Color = "#FFC107", Row = 0, Column = 2 });
        
        return profile;
    }
}
