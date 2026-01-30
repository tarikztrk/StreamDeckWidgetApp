using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Abstractions;

/// <summary>
/// Handles file drop operations on deck items.
/// </summary>
public interface IFileDropHandler
{
    /// <summary>
    /// Processes a dropped file and configures the target deck item.
    /// </summary>
    /// <param name="targetItem">The deck item to configure.</param>
    /// <param name="filePath">The path of the dropped file.</param>
    /// <returns>True if the file was processed successfully, false otherwise.</returns>
    bool HandleFileDrop(DeckItem targetItem, string filePath);
}
