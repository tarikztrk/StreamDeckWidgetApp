using System;
using System.IO;
using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Core.Helpers;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Services;

/// <summary>
/// Handles file drop operations on deck items.
/// Implements Single Responsibility Principle by extracting file processing logic.
/// </summary>
public class FileDropHandler : IFileDropHandler
{
    public bool HandleFileDrop(DeckItem targetItem, string filePath)
    {
        if (targetItem == null || string.IsNullOrEmpty(filePath))
            return false;

        string ext = Path.GetExtension(filePath).ToLower();
        string fileName = Path.GetFileNameWithoutExtension(filePath);

        // Handle image files
        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".ico")
        {
            targetItem.IconPath = filePath;
            targetItem.Title = "";
            return true;
        }

        // Handle executable files and shortcuts
        if (ext == ".exe" || ext == ".lnk" || ext == ".bat")
        {
            targetItem.Title = fileName;
            targetItem.Command = filePath;
            targetItem.ActionType = "Execute";

            // Extract and cache icon
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string iconsFolder = Path.Combine(appData, "StreamDeckWidgetApp", "CachedIcons");
            string? newIconPath = IconHelper.ExtractAndSaveIcon(filePath, iconsFolder);

            if (!string.IsNullOrEmpty(newIconPath))
            {
                targetItem.IconPath = newIconPath;
            }

            return true;
        }

        // Unsupported file type
        return false;
    }
}
