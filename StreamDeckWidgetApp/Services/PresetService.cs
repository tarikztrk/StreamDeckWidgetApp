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
            "Medya & Ses",
            "Klavye Kısayolları",
            "Metin Yazma"
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
                ActionType = "Hotkey",
                Command = "WIN_L",
                Color = "#FBC02D", // Sarı
                Icon = "\uE72E", // Segoe MDL2: Lock
                Description = "Windows'u kilitler (Win+L)"
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
                ActionType = "Hotkey",
                Command = "TASK_MANAGER",
                Color = "#43A047", // Yeşil
                Icon = "\uE7EE", // Segoe MDL2: System
                Description = "Ctrl+Shift+Esc kısayolu"
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
                ActionType = "Hotkey",
                Command = "WIN_E",
                Color = "#FDD835", // Sarı
                Icon = "\uE8B7", // Segoe MDL2: Folder
                Description = "Win+E kısayolu"
            },
            new PresetModel
            {
                Id = "app_run",
                Name = "Çalıştır (Run)",
                Category = "Windows Araçları",
                ActionType = "Hotkey",
                Command = "WIN+R",
                Color = "#455A64", // Slate
                Icon = "\uE756", // Segoe MDL2: CommandPrompt
                Description = "Win+R Çalıştır penceresi"
            },
            new PresetModel
            {
                Id = "win_desktop",
                Name = "Masaüstü Göster",
                Category = "Windows Araçları",
                ActionType = "Hotkey",
                Command = "WIN_D",
                Color = "#00796B", // Teal
                Icon = "\uE8FC", // Segoe MDL2: Desktop
                Description = "Tüm pencereleri simge durumuna küçült"
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
        
        // D. Kategori: Medya & Ses
        presets.AddRange(new[]
        {
            new PresetModel
            {
                Id = "media_playpause",
                Name = "Oynat / Durdur",
                Category = "Medya & Ses",
                ActionType = "MediaControl",
                Command = "PLAY_PAUSE",
                Color = "#43A047", // Yeşil
                Icon = "\uE768", // Segoe MDL2: Play
                Description = "Medya oynat/duraklat"
            },
            new PresetModel
            {
                Id = "media_next",
                Name = "Sonraki Parça",
                Category = "Medya & Ses",
                ActionType = "MediaControl",
                Command = "NEXT_TRACK",
                Color = "#26A69A", // Teal
                Icon = "\uE893", // Segoe MDL2: Next
                Description = "Sonraki medya parçası"
            },
            new PresetModel
            {
                Id = "media_prev",
                Name = "Önceki Parça",
                Category = "Medya & Ses",
                ActionType = "MediaControl",
                Command = "PREV_TRACK",
                Color = "#26A69A", // Teal
                Icon = "\uE892", // Segoe MDL2: Previous
                Description = "Önceki medya parçası"
            },
            new PresetModel
            {
                Id = "media_stop",
                Name = "Durdur",
                Category = "Medya & Ses",
                ActionType = "MediaControl",
                Command = "STOP",
                Color = "#EF5350", // Kırmızı
                Icon = "\uE71A", // Segoe MDL2: Stop
                Description = "Medyayı durdur"
            },
            new PresetModel
            {
                Id = "audio_mute",
                Name = "Sesi Kapat",
                Category = "Medya & Ses",
                ActionType = "AudioControl",
                Command = "MUTE",
                Color = "#D32F2F", // Kırmızı
                Icon = "\uE74F", // Segoe MDL2: Mute
                Description = "Sistem sesini kapat/aç"
            },
            new PresetModel
            {
                Id = "audio_volup",
                Name = "Ses Artır (+5)",
                Category = "Medya & Ses",
                ActionType = "AudioControl",
                Command = "VOL_UP",
                Color = "#1976D2", // Mavi
                Icon = "\uE995", // Segoe MDL2: Volume
                Description = "Sistem sesini 5 adım artır"
            },
            new PresetModel
            {
                Id = "audio_voldown",
                Name = "Ses Azalt (-5)",
                Category = "Medya & Ses",
                ActionType = "AudioControl",
                Command = "VOL_DOWN",
                Color = "#0288D1", // Açık Mavi
                Icon = "\uE992", // Segoe MDL2: Volume down
                Description = "Sistem sesini 5 adım azalt"
            }
        });
        
        // E. Kategori: Klavye Kısayolları
        presets.AddRange(new[]
        {
            // Temel düzenleme
            new PresetModel
            {
                Id = "key_copy",
                Name = "Kopyala",
                Category = "Klavye Kısayolları",
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
                Category = "Klavye Kısayolları",
                ActionType = "Hotkey",
                Command = "PASTE",
                Color = "#7B1FA2", // Koyu Mor
                Icon = "\uE77F", // Segoe MDL2: Paste
                Description = "Ctrl+V kısayolu"
            },
            new PresetModel
            {
                Id = "key_cut",
                Name = "Kes",
                Category = "Klavye Kısayolları",
                ActionType = "Hotkey",
                Command = "CUT",
                Color = "#9C27B0", // Mor
                Icon = "\uE8C6", // Segoe MDL2: Cut
                Description = "Ctrl+X kısayolu"
            },
            new PresetModel
            {
                Id = "key_undo",
                Name = "Geri Al",
                Category = "Klavye Kısayolları",
                ActionType = "Hotkey",
                Command = "UNDO",
                Color = "#FF7043", // Turuncu
                Icon = "\uE7A7", // Segoe MDL2: Undo
                Description = "Ctrl+Z kısayolu"
            },
            new PresetModel
            {
                Id = "key_redo",
                Name = "Yinele",
                Category = "Klavye Kısayolları",
                ActionType = "Hotkey",
                Command = "REDO",
                Color = "#FF5722", // Turuncu
                Icon = "\uE7A6", // Segoe MDL2: Redo
                Description = "Ctrl+Y kısayolu"
            },
            new PresetModel
            {
                Id = "key_selectall",
                Name = "Tümünü Seç",
                Category = "Klavye Kısayolları",
                ActionType = "Hotkey",
                Command = "SELECT_ALL",
                Color = "#1E88E5", // Mavi
                Icon = "\uE8B3", // Segoe MDL2: SelectAll
                Description = "Ctrl+A kısayolu"
            },
            
            // Dosya işlemleri
            new PresetModel
            {
                Id = "key_save",
                Name = "Kaydet",
                Category = "Klavye Kısayolları",
                ActionType = "Hotkey",
                Command = "SAVE",
                Color = "#43A047", // Yeşil
                Icon = "\uE74E", // Segoe MDL2: Save
                Description = "Ctrl+S kısayolu"
            },
            new PresetModel
            {
                Id = "key_new",
                Name = "Yeni",
                Category = "Klavye Kısayolları",
                ActionType = "Hotkey",
                Command = "NEW",
                Color = "#29B6F6", // Açık mavi
                Icon = "\uE8A5", // Segoe MDL2: NewWindow
                Description = "Ctrl+N kısayolu"
            },
            new PresetModel
            {
                Id = "key_open",
                Name = "Aç",
                Category = "Klavye Kısayolları",
                ActionType = "Hotkey",
                Command = "OPEN",
                Color = "#FFA726", // Turuncu
                Icon = "\uE8B7", // Segoe MDL2: OpenFile
                Description = "Ctrl+O kısayolu"
            },
            new PresetModel
            {
                Id = "key_print",
                Name = "Yazdır",
                Category = "Klavye Kısayolları",
                ActionType = "Hotkey",
                Command = "PRINT",
                Color = "#78909C", // Gri mavi
                Icon = "\uE749", // Segoe MDL2: Print
                Description = "Ctrl+P kısayolu"
            },
            new PresetModel
            {
                Id = "key_find",
                Name = "Bul",
                Category = "Klavye Kısayolları",
                ActionType = "Hotkey",
                Command = "FIND",
                Color = "#26A69A", // Teal
                Icon = "\uE721", // Segoe MDL2: Search
                Description = "Ctrl+F kısayolu"
            },
            new PresetModel
            {
                Id = "key_refresh",
                Name = "Yenile",
                Category = "Klavye Kısayolları",
                ActionType = "Hotkey",
                Command = "REFRESH",
                Color = "#42A5F5", // Mavi
                Icon = "\uE72C", // Segoe MDL2: Refresh
                Description = "F5 tuşu"
            },
            
            // Ekran ve pencere
            new PresetModel
            {
                Id = "key_screenshot",
                Name = "Ekran Alıntısı",
                Category = "Klavye Kısayolları",
                ActionType = "Hotkey",
                Command = "SCREENSHOT",
                Color = "#00897B", // Teal
                Icon = "\uE7B7", // Segoe MDL2: Camera
                Description = "Win+Shift+S (Snipping Tool)"
            },
            new PresetModel
            {
                Id = "key_alttab",
                Name = "Pencere Değiştir",
                Category = "Klavye Kısayolları",
                ActionType = "Hotkey",
                Command = "ALT_TAB",
                Color = "#546E7A", // Slate
                Icon = "\uE737", // Segoe MDL2: Switch
                Description = "Alt+Tab kısayolu"
            },
            new PresetModel
            {
                Id = "key_close",
                Name = "Sekmeyi Kapat",
                Category = "Klavye Kısayolları",
                ActionType = "Hotkey",
                Command = "CLOSE",
                Color = "#E53935", // Kırmızı
                Icon = "\uE8BB", // Segoe MDL2: Cancel
                Description = "Ctrl+W kısayolu"
            },
            new PresetModel
            {
                Id = "key_altf4",
                Name = "Pencereyi Kapat",
                Category = "Klavye Kısayolları",
                ActionType = "Hotkey",
                Command = "ALT+F4",
                Color = "#C62828", // Koyu kırmızı
                Icon = "\uE8BB", // Segoe MDL2: Cancel
                Description = "Alt+F4 ile pencereyi kapat"
            }
        });
        
        // F. Kategori: Metin Yazma
        presets.AddRange(new[]
        {
            new PresetModel
            {
                Id = "text_email",
                Name = "E-posta Adresi",
                Category = "Metin Yazma",
                ActionType = "TextType",
                Command = "ornek@email.com",
                Color = "#1565C0", // Mavi
                Icon = "\uE715", // Segoe MDL2: Mail
                Description = "E-posta adresinizi hızlıca yazın"
            },
            new PresetModel
            {
                Id = "text_phone",
                Name = "Telefon No",
                Category = "Metin Yazma",
                ActionType = "TextType",
                Command = "+90 5XX XXX XX XX",
                Color = "#2E7D32", // Yeşil
                Icon = "\uE717", // Segoe MDL2: Phone
                Description = "Telefon numaranızı hızlıca yazın"
            },
            new PresetModel
            {
                Id = "text_address",
                Name = "Adres",
                Category = "Metin Yazma",
                ActionType = "TextType",
                Command = "İstanbul, Türkiye",
                Color = "#6D4C41", // Kahverengi
                Icon = "\uE80F", // Segoe MDL2: Map
                Description = "Adresinizi hızlıca yazın"
            },
            new PresetModel
            {
                Id = "text_signature",
                Name = "İmza",
                Category = "Metin Yazma",
                ActionType = "TextType",
                Command = "Saygılarımla,\nAd Soyad",
                Color = "#5C6BC0", // İndigo
                Icon = "\uE8AC", // Segoe MDL2: Edit
                Description = "E-posta imzanızı hızlıca yazın"
            },
            new PresetModel
            {
                Id = "text_greeting",
                Name = "Selamlaşma",
                Category = "Metin Yazma",
                ActionType = "TextType",
                Command = "Merhaba,\n\nUmarım iyisinizdir.",
                Color = "#7986CB", // İndigo açık
                Icon = "\uE76E", // Segoe MDL2: People
                Description = "E-posta açılış metni"
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
