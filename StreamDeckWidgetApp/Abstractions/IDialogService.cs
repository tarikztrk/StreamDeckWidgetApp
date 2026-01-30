using System.Windows;

namespace StreamDeckWidgetApp.Abstractions;

/// <summary>
/// Abstracts dialog interactions (MessageBox) for testability and SRP.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows a confirmation dialog with Yes/No buttons.
    /// </summary>
    /// <param name="message">The dialog message.</param>
    /// <param name="title">The dialog title.</param>
    /// <returns>True if user clicked Yes, false otherwise.</returns>
    bool ShowConfirmation(string message, string title);

    /// <summary>
    /// Shows a warning message with OK button.
    /// </summary>
    void ShowWarning(string message, string title);

    /// <summary>
    /// Shows an error message with OK button.
    /// </summary>
    void ShowError(string message, string title);

    /// <summary>
    /// Shows an information message with OK button.
    /// </summary>
    void ShowInfo(string message, string title);
}
