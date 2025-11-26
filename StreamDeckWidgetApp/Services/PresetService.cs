using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Services;

/// <summary>
/// Hazır şablon kütüphanesi veri sağlayıcısı
/// </summary>
public static class PresetService
{
    /// <summary>
    /// Tüm preset kategorileri
    /// </summary>
    public static List<string> GetCategories()
    {
        return new List<string>
        {
            "Sistem Kontrolü",
            "Windows Araçları",
            "Web & Yayın",
            "Medya & Kısayollar"
        };
    }
    
    /// <summary>
    /// Tüm preset'leri döndür
    /// </summary>
    public static List<PresetModel> GetAllPresets()
    {
        var presets = new List<PresetModel>();
        
        // A. Kategori: Sistem Kontrolü
        presets.AddRange(new[]
        {
            new PresetModel
            {
                Id = "sys_shutdown",
                Name = "Bilgisayarı Kapat",
                Category = "Sistem Kontrolü",
                ActionType = "Execute",
                Command = "shutdown /s /t 0",
                Color = "#D32F2F", // Kırmızı
                Icon = "\uE7E8", // Segoe MDL2: Power Button
                Description = "Bilgisayarı hemen kapatır"
            },
            new PresetModel
            {
                Id = "sys_restart",
                Name = "Yeniden Başlat",
                Category = "Sistem Kontrolü",
                ActionType = "Execute",
                Command = "shutdown /r /t 0",
                Color = "#FF6F00", // Turuncu
                Icon = "\uE777", // Segoe MDL2: Refresh
                Description = "Bilgisayarı yeniden başlatır"
            },
            new PresetModel
            {
                Id = "sys_sleep",
                Name = "Uyku Modu",
                Category = "Sistem Kontrolü",
                ActionType = "Execute",
                Command = "rundll32.exe powrprof.dll,SetSuspendState 0,1,0",
                Color = "#1976D2", // Mavi
                Icon = "\uE708", // Segoe MDL2: Half Moon
                Description = "Bilgisayarı uyku moduna alır"
            },
            new PresetModel
            {
                Id = "sys_lock",
                Name = "Kilitle (Lock)",
                Category = "Sistem Kontrolü",
                ActionType = "Execute",
                Command = "rundll32.exe user32.dll,LockWorkStation",
                Color = "#FBC02D", // Sarı
                Icon = "\uE72E", // Segoe MDL2: Lock
                Description = "Windows'u kilitler"
            },
            new PresetModel
            {
                Id = "sys_recycle",
                Name = "Çöpü Boşalt",
                Category = "Sistem Kontrolü",
                ActionType = "Execute",
                Command = "PowerShell.exe -Command \"Clear-RecycleBin -Force -ErrorAction SilentlyContinue\"",
                Color = "#757575", // Gri
                Icon = "\uE74D", // Segoe MDL2: Delete
                Description = "Geri dönüşüm kutusunu boşaltır"
            }
        });
        
        // B. Kategori: Windows Araçları
        presets.AddRange(new[]
        {
            new PresetModel
            {
                Id = "app_calc",
                Name = "Hesap Makinesi",
                Category = "Windows Araçları",
                ActionType = "Execute",
                Command = "calc.exe",
                Color = "#00ACC1", // Turkuaz
                Icon = "\uE8EF", // Segoe MDL2: Calculator
                Description = "Windows Hesap Makinesi"
            },
            new PresetModel
            {
                Id = "app_notepad",
                Name = "Not Defteri",
                Category = "Windows Araçları",
                ActionType = "Execute",
                Command = "notepad.exe",
                Color = "#1E88E5", // Mavi
                Icon = "\uE70F", // Segoe MDL2: Page
                Description = "Notepad uygulaması"
            },
            new PresetModel
            {
                Id = "app_paint",
                Name = "Paint",
                Category = "Windows Araçları",
                ActionType = "Execute",
                Command = "mspaint.exe",
                Color = "#8E24AA", // Mor
                Icon = "\uE771", // Segoe MDL2: Color
                Description = "Microsoft Paint"
            },
            new PresetModel
            {
                Id = "app_taskmgr",
                Name = "Görev Yöneticisi",
                Category = "Windows Araçları",
                ActionType = "Execute",
                Command = "taskmgr.exe",
                Color = "#43A047", // Yeşil
                Icon = "\uE7EE", // Segoe MDL2: System
                Description = "Windows Görev Yöneticisi"
            },
            new PresetModel
            {
                Id = "app_cmd",
                Name = "Komut İstemi",
                Category = "Windows Araçları",
                ActionType = "Execute",
                Command = "cmd.exe",
                Color = "#212121", // Siyah
                Icon = "\uE756", // Segoe MDL2: CommandPrompt
                Description = "CMD (Command Prompt)"
            },
            new PresetModel
            {
                Id = "app_explorer",
                Name = "Dosya Gezgini",
                Category = "Windows Araçları",
                ActionType = "Execute",
                Command = "explorer.exe",
                Color = "#FDD835", // Sarı
                Icon = "\uE8B7", // Segoe MDL2: Folder
                Description = "Windows Dosya Gezgini"
            }
        });
        
        // C. Kategori: Web & Yayın
        presets.AddRange(new[]
        {
            new PresetModel
            {
                Id = "web_google",
                Name = "Google",
                Category = "Web & Yayın",
                ActionType = "Website",
                Command = "https://google.com",
                Color = "#FFFFFF", // Beyaz
                Icon = "\uE721", // Segoe MDL2: Globe
                Description = "Google Arama"
            },
            new PresetModel
            {
                Id = "web_youtube",
                Name = "YouTube",
                Category = "Web & Yayın",
                ActionType = "Website",
                Command = "https://youtube.com",
                Color = "#FF0000", // YouTube Kırmızı
                Icon = "\uE714", // Segoe MDL2: Video
                Description = "YouTube"
            },
            new PresetModel
            {
                Id = "web_twitch",
                Name = "Twitch",
                Category = "Web & Yayın",
                ActionType = "Website",
                Command = "https://twitch.tv",
                Color = "#9146FF", // Twitch Mor
                Icon = "\uE8B3", // Segoe MDL2: Streaming
                Description = "Twitch Canlı Yayın"
            },
            new PresetModel
            {
                Id = "app_spotify",
                Name = "Spotify",
                Category = "Web & Yayın",
                ActionType = "Execute",
                Command = "spotify:",
                Color = "#1DB954", // Spotify Yeşil
                Icon = "\uE8D6", // Segoe MDL2: MusicInfo
                Description = "Spotify uygulamasını aç"
            },
            new PresetModel
            {
                Id = "app_discord",
                Name = "Discord",
                Category = "Web & Yayın",
                ActionType = "Execute",
                Command = "discord:",
                Color = "#5865F2", // Discord Mor
                Icon = "\uE8BD", // Segoe MDL2: Message
                Description = "Discord uygulamasını aç"
            },
            new PresetModel
            {
                Id = "web_chatgpt",
                Name = "ChatGPT",
                Category = "Web & Yayın",
                ActionType = "Website",
                Command = "https://chatgpt.com",
                Color = "#10A37F", // ChatGPT Yeşil
                Icon = "\uE8F2", // Segoe MDL2: Robot
                Description = "ChatGPT Web"
            }
        });
        
        // D. Kategori: Medya & Kısayollar
        presets.AddRange(new[]
        {
            new PresetModel
            {
                Id = "key_playpause",
                Name = "Oynat / Durdur",
                Category = "Medya & Kısayollar",
                ActionType = "Hotkey",
                Command = "MEDIA_PLAY_PAUSE",
                Color = "#43A047", // Yeşil
                Icon = "\uE768", // Segoe MDL2: Play
                Description = "Medya oynat/duraklat"
            },
            new PresetModel
            {
                Id = "key_mute",
                Name = "Sesi Kapat",
                Category = "Medya & Kısayollar",
                ActionType = "Hotkey",
                Command = "MUTE",
                Color = "#D32F2F", // Kırmızı
                Icon = "\uE74F", // Segoe MDL2: Mute
                Description = "Sistem sesini kapat/aç"
            },
            new PresetModel
            {
                Id = "key_volup",
                Name = "Ses Artır",
                Category = "Medya & Kısayollar",
                ActionType = "Hotkey",
                Command = "VOL_UP",
                Color = "#1976D2", // Mavi
                Icon = "\uE995", // Segoe MDL2: Volume
                Description = "Sistem sesini artır"
            },
            new PresetModel
            {
                Id = "key_voldown",
                Name = "Ses Azalt",
                Category = "Medya & Kısayollar",
                ActionType = "Hotkey",
                Command = "VOL_DOWN",
                Color = "#0288D1", // Açık Mavi
                Icon = "\uE992", // Segoe MDL2: Volume down
                Description = "Sistem sesini azalt"
            },
            new PresetModel
            {
                Id = "key_copy",
                Name = "Kopyala",
                Category = "Medya & Kısayollar",
                ActionType = "Hotkey",
                Command = "COPY",
                Color = "#5E35B1", // Mor
                Icon = "\uE8C8", // Segoe MDL2: Copy
                Description = "Ctrl+C kısayolu"
            },
            new PresetModel
            {
                Id = "key_paste",
                Name = "Yapıştır",
                Category = "Medya & Kısayollar",
                ActionType = "Hotkey",
                Command = "PASTE",
                Color = "#7B1FA2", // Koyu Mor
                Icon = "\uE77F", // Segoe MDL2: Paste
                Description = "Ctrl+V kısayolu"
            },
            new PresetModel
            {
                Id = "key_screenshot",
                Name = "Ekran Alıntısı",
                Category = "Medya & Kısayollar",
                ActionType = "Hotkey",
                Command = "SCREENSHOT",
                Color = "#00897B", // Teal
                Icon = "\uE7B7", // Segoe MDL2: Camera
                Description = "Win+Shift+S (Snipping Tool)"
            }
        });
        
        return presets;
    }
    
    /// <summary>
    /// Kategoriye göre filtrele
    /// </summary>
    public static List<PresetModel> GetPresetsByCategory(string category)
    {
        return GetAllPresets().Where(p => p.Category == category).ToList();
    }
    
    /// <summary>
    /// Arama yap
    /// </summary>
    public static List<PresetModel> SearchPresets(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return GetAllPresets();
            
        var search = searchText.ToLower();
        return GetAllPresets()
            .Where(p => 
                p.Name.ToLower().Contains(search) || 
                p.Description.ToLower().Contains(search) ||
                p.Category.ToLower().Contains(search))
            .ToList();
    }
}
