using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Abstractions;

public interface IConfigService
{
    // Mevcut aktif profili yükle
    Profile LoadProfile();
    
    // Mevcut profili kaydet
    void SaveProfile(Profile profile);
    
    // Tüm profilleri getir
    List<Profile> GetAllProfiles();
    
    // Yeni profil oluştur
    Profile CreateProfile(string name);
    
    // Profil sil
    bool DeleteProfile(string profileId);
    
    // Profili aktif yap
    void SetActiveProfile(string profileId);
    
    // Profili klonla
    Profile DuplicateProfile(string profileId, string newName);
    
    // Profili yeniden adlandır
    void RenameProfile(string profileId, string newName);
}
