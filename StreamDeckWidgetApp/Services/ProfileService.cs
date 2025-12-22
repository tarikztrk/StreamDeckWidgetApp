using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Services;

/// <summary>
/// Service for profile management operations.
/// Handles loading, switching, creating, and deleting profiles.
/// </summary>
public class ProfileService : IProfileService
{
    private readonly IConfigService _configService;
    private Profile _currentProfile = null!;
    private List<Profile> _profiles = new();

    public ProfileService(IConfigService configService)
    {
        _configService = configService;
        LoadCurrentProfile();
        ReloadProfiles();
    }

    public Profile CurrentProfile => _currentProfile;

    public IReadOnlyList<Profile> Profiles => _profiles.AsReadOnly();

    public event Action? ProfileChanged;

    public void SwitchProfile(string profileId)
    {
        if (string.IsNullOrEmpty(profileId) || profileId == _currentProfile.Id)
            return;

        // Save current profile before switching
        SaveCurrentProfile();

        // Switch to new profile
        _configService.SetActiveProfile(profileId);
        LoadCurrentProfile();
        ReloadProfiles();

        ProfileChanged?.Invoke();
    }

    public Profile CreateProfile(string name)
    {
        // Save current profile before creating new one
        SaveCurrentProfile();

        // Create new profile
        var newProfile = _configService.CreateProfile(name);

        // Switch to new profile
        _configService.SetActiveProfile(newProfile.Id);
        _currentProfile = newProfile;
        ReloadProfiles();

        ProfileChanged?.Invoke();
        return newProfile;
    }

    public bool DeleteCurrentProfile()
    {
        if (_profiles.Count <= 1)
            return false;

        var profileIdToDelete = _currentProfile.Id;
        _configService.DeleteProfile(profileIdToDelete);

        // Load the new active profile
        LoadCurrentProfile();
        ReloadProfiles();

        ProfileChanged?.Invoke();
        return true;
    }

    public Profile DuplicateCurrentProfile(string newName)
    {
        var duplicated = _configService.DuplicateProfile(_currentProfile.Id, newName);

        // Switch to duplicated profile
        _configService.SetActiveProfile(duplicated.Id);
        _currentProfile = duplicated;
        ReloadProfiles();

        ProfileChanged?.Invoke();
        return duplicated;
    }

    public void RenameCurrentProfile(string newName)
    {
        if (_currentProfile.Name != newName)
        {
            _configService.RenameProfile(_currentProfile.Id, newName);
            _currentProfile.Name = newName;
        }
    }

    public void SaveCurrentProfile()
    {
        _configService.SaveProfile(_currentProfile);
    }

    public void ReloadProfiles()
    {
        _profiles = _configService.GetAllProfiles();
    }

    private void LoadCurrentProfile()
    {
        _currentProfile = _configService.LoadProfile();

        // Set default button size if not set (legacy profiles)
        if (_currentProfile.ButtonSize == 0)
            _currentProfile.ButtonSize = 85;
    }
}
