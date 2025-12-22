using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Abstractions;

/// <summary>
/// Interface for profile management operations.
/// Handles loading, switching, creating, and deleting profiles.
/// </summary>
public interface IProfileService
{
    /// <summary>
    /// Gets the currently active profile.
    /// </summary>
    Profile CurrentProfile { get; }
    
    /// <summary>
    /// Gets all available profiles.
    /// </summary>
    IReadOnlyList<Profile> Profiles { get; }
    
    /// <summary>
    /// Event raised when the current profile changes.
    /// </summary>
    event Action? ProfileChanged;
    
    /// <summary>
    /// Switches to a different profile by ID.
    /// </summary>
    void SwitchProfile(string profileId);
    
    /// <summary>
    /// Creates a new profile with the given name.
    /// </summary>
    Profile CreateProfile(string name);
    
    /// <summary>
    /// Deletes the current profile. Returns false if it's the last profile.
    /// </summary>
    bool DeleteCurrentProfile();
    
    /// <summary>
    /// Duplicates the current profile with a new name.
    /// </summary>
    Profile DuplicateCurrentProfile(string newName);
    
    /// <summary>
    /// Renames the current profile.
    /// </summary>
    void RenameCurrentProfile(string newName);
    
    /// <summary>
    /// Saves changes to the current profile.
    /// </summary>
    void SaveCurrentProfile();
    
    /// <summary>
    /// Reloads all profiles from storage.
    /// </summary>
    void ReloadProfiles();
}
