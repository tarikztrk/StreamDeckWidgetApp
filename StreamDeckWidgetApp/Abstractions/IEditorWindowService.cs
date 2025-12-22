using System.Windows;

namespace StreamDeckWidgetApp.Abstractions;

/// <summary>
/// Interface for editor window management.
/// Handles opening, closing, and state tracking of the editor window.
/// </summary>
public interface IEditorWindowService
{
    /// <summary>
    /// Gets whether the editor window is currently open.
    /// </summary>
    bool IsEditorOpen { get; }
    
    /// <summary>
    /// Event raised when the editor window is closed.
    /// </summary>
    event Action? EditorClosed;
    
    /// <summary>
    /// Sets the main window reference (required for modal behavior).
    /// </summary>
    void SetMainWindow(Window mainWindow);
    
    /// <summary>
    /// Opens the editor window. If already open, brings it to front.
    /// </summary>
    void OpenEditor();
    
    /// <summary>
    /// Closes the editor window if open.
    /// </summary>
    void CloseEditor();
}
