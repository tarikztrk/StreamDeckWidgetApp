Harika. Her iki planı birleştirerek, hem **teknik mimariyi (SOLID/MVVM)** koruyan hem de **zengin özellikleri (OBS, Makro, Ses Kontrolü)** kapsayan nihai **Master Roadmap**'i oluşturuyorum.

Bu plan, projenin "kutsal kitabı" olacak. Karışıklığı önlemek için projeyi mantıksal fazlara böldüm.

---

# 🗺️ Virtual Stream Deck - Master Development Roadmap

Bu yol haritası, .NET 10 ve WPF kullanılarak, SOLID prensiplerine uygun, ölçeklenebilir bir masaüstü widget uygulaması geliştirmek için hazırlanmıştır.

## 🏗️ Faz 1: Temel Mimari ve Veri Kalıcılığı (Foundation & Persistence)
*Hedef: Uygulamanın beynini oluşturmak. Ayarların kaydedilmesi ve dinamik yapı.*

1.  **JSON Konfigürasyon Servisi:**
    *   Butonların başlık, renk, komut ve konum bilgilerinin JSON dosyasına kaydedilmesi/okunması.
    *   `IConfigService` arayüzü ile bağımlılığın soyutlanması.
2.  **Dinamik Grid Yapısı:**
    *   Kullanıcının satır/sütun sayısını (örn: 3x3, 4x2, 5x4) ayarlayabilmesi.
    *   Pencere boyutunun grid yapısına göre otomatik ölçeklenmesi.
3.  **Temel Veri Modeli Genişletmesi:**
    *   `DeckItem` modeline `ID`, `Row`, `Column`, `IconPath` gibi özelliklerin eklenmesi.

## ⚙️ Faz 2: Aksiyon Motoru (The Action Engine)
*Hedef: "Strategy Pattern" kullanarak, kod karmaşası yaratmadan sonsuz çeşitlilikte görev ekleyebilmek.*

1.  **Action Factory & Strategy Pattern:**
    *   `IAction` arayüzünün oluşturulması.
    *   **Core Actions:**
        *   `ProcessAction`: Uygulama/Dosya çalıştırır.
        *   `UrlAction`: Web sitesi açar.
        *   `HotkeyAction`: Klavye kısayolu gönderir (örn: CTRL+C, Win+Shift+S).
        *   `TextAction`: Belirlenen metni (snippet) aktif pencereye yapıştırır.
2.  **Hotkey Simülasyon Servisi:**
    *   Windows API (User32.dll) veya `SendInput` kullanılarak klavye tuşlamalarının simüle edilmesi.

## 🎨 Faz 3: Düzenleme Modu ve UI (Editor & Interaction)
*Hedef: Kullanıcının kod/JSON ile uğraşmadan arayüz üzerinden ayar yapması.*

1.  **Edit Mode Toggle:**
    *   "Düzenle" butonuna basınca butonların tıklanabilir halden seçilebilir hale geçmesi.
2.  **Property Panel (Özellik Paneli):**
    *   Seçili butona tıklandığında sağda veya popup olarak açılan ayar ekranı.
    *   Başlık, Renk Seçici (Color Picker), Dosya/İkon Seçimi.
3.  **Drag & Drop (Sürükle Bırak):**
    *   Masaüstünden bir `.exe` veya dosyayı butonun üzerine sürükleyince otomatik tanımlama.
4.  **İkon Desteği:**
    *   Butonlarda metin yerine resim (PNG/JPG) veya FontAwesome/Material ikon kullanımı.

## ⚡ Faz 4: Gelişmiş Mantık ve Otomasyon (Advanced Logic)
*Hedef: Uygulamayı basit bir "kısayol başlatıcı"dan "otomasyon aracı"na dönüştürmek.*

1.  **Multi-Action (Makro) Sistemi:**
    *   Bir butona birden fazla `IAction` ekleyebilme.
    *   Araya `DelayAction` (Bekle) ekleyerek sıralı işlem yapma (Örn: Oyunu aç -> 5sn bekle -> Müziği aç).
2.  **Profil ve Klasörleme:**
    *   **Profiller:** "Yayın Modu", "İş Modu", "Oyun Modu" gibi farklı sayfalar oluşturma.
    *   **Klasörler:** Bir butona basınca alt menüye inme (Navigasyon yapısı).
3.  **Otomatik Profil Geçişi (Context Awareness):**
    *   Aktif pencereyi dinleyen bir servis (Watcher). Photoshop açılınca otomatik "Tasarım Profili"ne geçiş.

## 🔌 Faz 5: Dış Entegrasyonlar (Integrations & Plugins)
*Hedef: Üçüncü parti yazılım ve donanımlarla konuşmak.*

1.  **OBS Studio Entegrasyonu:**
    *   `OBS-WebSocket` ile sahne değiştirme, yayını başlatma/durdurma, kaynak gizleme/açma.
2.  **Ses Mikseri (Audio Control):**
    *   Windows Core Audio API (WASAPI) kullanarak uygulama bazlı ses kontrolü (Örn: Discord sesini kıs, Spotify aç).
3.  **Sistem İzleme (Dashboard):**
    *   CPU, RAM, Ağ kullanımı gibi verilerin buton üzerinde canlı gösterimi.
4.  **Medya Kontrolü:**
    *   Spotify/System medya tuşları (Oynat, Durdur, Sonraki).

## ✨ Faz 6: Cila ve Dağıtım (Polish & Deploy)
1.  **Görsel Efektler:** Hover, Click animasyonları, Acrylic/Blur efektleri.
2.  **Tray Icon:** Uygulamanın sistem tepsisine küçülmesi ve arka planda çalışması.
3.  **Installer:** MSIX veya Setup dosyası ile dağıtım paketi oluşturma.

---

### 📂 Hedeflenen Klasör Yapısı (Architecture)

Proje ilerledikçe dosya yapımız şu şekilde evrilecek:

```text
StreamDeckWidgetApp/
├── Core/
│   ├── Actions/            # IAction, ActionFactory
│   ├── Events/             # EventAggregator (Modüller arası iletişim)
│   └── Helpers/            # WinAPI çağrıları, Json Converter'lar
├── Features/
│   ├── Configuration/      # Json Load/Save işlemleri
│   ├── Hotkeys/            # Klavye dinleme ve gönderme
│   ├── Audio/              # Ses kontrol işlemleri
│   └── OBS/                # OBS entegrasyon kodları
├── Models/
│   ├── Actions/            # Action veri modelleri
│   ├── DeckItem.cs         # Buton modeli
│   └── Profile.cs          # Sayfa/Profil modeli
├── Services/               # ViewModel'lerin kullandığı servisler
├── ViewModels/             # MVVM Logic
├── Views/                  # XAML Dosyaları
│   ├── Controls/           # Özelleştirilmiş UserControl'ler
│   └── Windows/            # MainWindow, SettingsWindow
└── App.xaml.cs
```

---

### 🚀 Başlangıç: Faz 1 - Veri Kalıcılığı

Yol haritası onaylandıysa, kodlamaya **Faz 1: JSON Konfigürasyonu ve Veri Modeli** ile devam edelim.

Şu anki kodumuzda veriler `LoadDummyData` ile kodun içinde (Hardcoded) duruyor. Bunu dinamik hale getireceğiz.

**Devam etmek için onayınızı bekliyorum:**
*"Faz 1'e başlayalım ve JSON kaydetme/yükleme altyapısını kuralım."* diyorsanız kodları paylaşmaya başlayacağım.