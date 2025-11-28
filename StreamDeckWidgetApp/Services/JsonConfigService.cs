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
    private AppConfig _appConfig = null!;

    public JsonConfigService()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _configFolder = Path.Combine(appData, "StreamDeckWidgetApp");
        _configFile = Path.Combine(_configFolder, "config.json");
        
        LoadAppConfig();
    }

    /// <summary>
    /// Tüm uygulama konfigürasyonunu yükle
    /// </summary>
    private void LoadAppConfig()
    {
        // Klasör yoksa oluştur
        if (!Directory.Exists(_configFolder))
            Directory.CreateDirectory(_configFolder);

        // Dosya yoksa varsayılan config oluştur
        if (!File.Exists(_configFile))
        {
            _appConfig = CreateDefaultAppConfig();
            SaveAppConfig();
            return;
        }

        try
        {
            string json = File.ReadAllText(_configFile);
            _appConfig = JsonSerializer.Deserialize<AppConfig>(json) ?? CreateDefaultAppConfig();
            
            // Profil listesi boşsa varsayılan ekle
            if (_appConfig.Profiles.Count == 0)
            {
                var defaultProfile = CreateDefaultProfile();
                _appConfig.Profiles.Add(defaultProfile);
                _appConfig.ActiveProfileId = defaultProfile.Id;
                SaveAppConfig();
            }
        }
        catch
        {
            _appConfig = CreateDefaultAppConfig();
            SaveAppConfig();
        }
    }

    /// <summary>
    /// Uygulama konfigürasyonunu dosyaya kaydet
    /// </summary>
    private void SaveAppConfig()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(_appConfig, options);
        File.WriteAllText(_configFile, json);
    }

    public Profile LoadProfile()
    {
        // Aktif profili bul
        var profile = _appConfig.Profiles.FirstOrDefault(p => p.Id == _appConfig.ActiveProfileId);
        
        // Bulunamazsa ilk profili döndür
        if (profile == null && _appConfig.Profiles.Count > 0)
        {
            profile = _appConfig.Profiles[0];
            _appConfig.ActiveProfileId = profile.Id;
            SaveAppConfig();
        }
        
        return profile ?? CreateDefaultProfile();
    }

    public void SaveProfile(Profile profile)
    {
        profile.ModifiedAt = DateTime.Now;
        
        // Mevcut profili güncelle
        var index = _appConfig.Profiles.FindIndex(p => p.Id == profile.Id);
        if (index >= 0)
        {
            _appConfig.Profiles[index] = profile;
        }
        else
        {
            _appConfig.Profiles.Add(profile);
        }
        
        SaveAppConfig();
    }

    public List<Profile> GetAllProfiles()
    {
        return _appConfig.Profiles.ToList();
    }

    public Profile CreateProfile(string name)
    {
        var profile = new Profile
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Rows = 2,
            Columns = 3,
            ButtonSize = 85,
            CreatedAt = DateTime.Now,
            ModifiedAt = DateTime.Now,
            Items = new List<DeckItem>()
        };
        
        _appConfig.Profiles.Add(profile);
        SaveAppConfig();
        
        return profile;
    }

    public bool DeleteProfile(string profileId)
    {
        // En az 1 profil kalmalı
        if (_appConfig.Profiles.Count <= 1)
            return false;
        
        var profile = _appConfig.Profiles.FirstOrDefault(p => p.Id == profileId);
        if (profile == null)
            return false;
        
        _appConfig.Profiles.Remove(profile);
        
        // Silinen profil aktifse, ilk profile geç
        if (_appConfig.ActiveProfileId == profileId)
        {
            _appConfig.ActiveProfileId = _appConfig.Profiles[0].Id;
        }
        
        SaveAppConfig();
        return true;
    }

    public void SetActiveProfile(string profileId)
    {
        if (_appConfig.Profiles.Any(p => p.Id == profileId))
        {
            _appConfig.ActiveProfileId = profileId;
            SaveAppConfig();
        }
    }

    public Profile DuplicateProfile(string profileId, string newName)
    {
        var source = _appConfig.Profiles.FirstOrDefault(p => p.Id == profileId);
        if (source == null)
            return CreateProfile(newName);
        
        var duplicate = new Profile
        {
            Id = Guid.NewGuid().ToString(),
            Name = newName,
            Rows = source.Rows,
            Columns = source.Columns,
            ButtonSize = source.ButtonSize,
            CreatedAt = DateTime.Now,
            ModifiedAt = DateTime.Now,
            Items = source.Items.Select(item => new DeckItem
            {
                Id = Guid.NewGuid().ToString(),
                Title = item.Title,
                ActionType = item.ActionType,
                Command = item.Command,
                Color = item.Color,
                Icon = item.Icon,
                IconPath = item.IconPath,
                BehaviorType = item.BehaviorType,
                Row = item.Row,
                Column = item.Column
            }).ToList()
        };
        
        _appConfig.Profiles.Add(duplicate);
        SaveAppConfig();
        
        return duplicate;
    }

    public void RenameProfile(string profileId, string newName)
    {
        var profile = _appConfig.Profiles.FirstOrDefault(p => p.Id == profileId);
        if (profile != null)
        {
            profile.Name = newName;
            profile.ModifiedAt = DateTime.Now;
            SaveAppConfig();
        }
    }

    private AppConfig CreateDefaultAppConfig()
    {
        var defaultProfile = CreateDefaultProfile();
        return new AppConfig
        {
            Version = "1.0",
            ActiveProfileId = defaultProfile.Id,
            Profiles = new List<Profile> { defaultProfile }
        };
    }

    private Profile CreateDefaultProfile()
    {
        var profile = new Profile 
        { 
            Id = Guid.NewGuid().ToString(),
            Name = "Varsayılan",
            Rows = 2, 
            Columns = 3,
            CreatedAt = DateTime.Now,
            ModifiedAt = DateTime.Now
        };
        
        // Varsayılan örnek Veriler
        profile.Items.Add(new DeckItem { Title = "Hesap Mak.", Command = "calc.exe", Color = "#E91E63", Row = 0, Column = 0, Icon = "\uE8EF" });
        profile.Items.Add(new DeckItem { Title = "Notepad", Command = "notepad.exe", Color = "#2196F3", Row = 0, Column = 1, Icon = "\uE70F" });
        profile.Items.Add(new DeckItem { Title = "Google", Command = "https://google.com", ActionType = "Website", Color = "#FFC107", Row = 0, Column = 2, Icon = "\uE721" });
        
        return profile;
    }
}
