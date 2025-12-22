using System.Windows;
using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.ViewModels;
using StreamDeckWidgetApp.Views;

namespace StreamDeckWidgetApp.Services;

/// <summary>
/// Service for editor window management.
/// Handles opening, closing, and state tracking of the editor window.
/// </summary>
public class EditorWindowService : IEditorWindowService
{
    private readonly Func<EditorViewModel> _editorViewModelFactory;
    private readonly Func<EditorWindow> _editorWindowFactory;
    private Window? _mainWindow;
    private EditorWindow? _editorWindow;

    public EditorWindowService(
        Func<EditorViewModel> editorViewModelFactory,
        Func<EditorWindow> editorWindowFactory)
    {
        _editorViewModelFactory = editorViewModelFactory;
        _editorWindowFactory = editorWindowFactory;
    }

    public bool IsEditorOpen => _editorWindow != null;

    public event Action? EditorClosed;

    public void SetMainWindow(Window mainWindow)
    {
        _mainWindow = mainWindow;
    }

    public void OpenEditor()
    {
        // If already open, bring to front
        if (_editorWindow != null)
        {
            _editorWindow.Activate();
            return;
        }

        try
        {
            // Hide main window with fade-out animation (modal mode)
            if (_mainWindow != null)
            {
                var fadeOut = new System.Windows.Media.Animation.DoubleAnimation
                {
                    From = 1.0,
                    To = 0.0,
                    Duration = TimeSpan.FromMilliseconds(200)
                };
                fadeOut.Completed += (s, e) => _mainWindow.Hide();
                _mainWindow.BeginAnimation(Window.OpacityProperty, fadeOut);
            }

            // Create EditorWindow and EditorViewModel via DI
            _editorWindow = _editorWindowFactory();
            var editorViewModel = _editorViewModelFactory();

            // Set window reference for EditorViewModel
            editorViewModel.SetWindow(_editorWindow);
            _editorWindow.DataContext = editorViewModel;

            // Set window ownership (critical for Taskbar/Alt-Tab behavior)
            _editorWindow.Owner = _mainWindow;

            // Clean up on close and show main window
            _editorWindow.Closed += (s, e) => OnEditorWindowClosed();

            _editorWindow.Show();
        }
        catch (Exception ex)
        {
            // If editor fails to open, restore main window (safety measure)
            if (_mainWindow != null)
            {
                _mainWindow.Opacity = 1.0;
                _mainWindow.Show();
            }

            MessageBox.Show($"Editör açılırken hata oluştu: {ex.Message}",
                "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void CloseEditor()
    {
        _editorWindow?.Close();
    }

    private void OnEditorWindowClosed()
    {
        _editorWindow = null;

        // Show main window with fade-in animation (end modal mode)
        if (_mainWindow != null)
        {
            _mainWindow.Opacity = 0.0;
            _mainWindow.Show();
            _mainWindow.Activate();

            var fadeIn = new System.Windows.Media.Animation.DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            _mainWindow.BeginAnimation(Window.OpacityProperty, fadeIn);
        }

        EditorClosed?.Invoke();
    }
}
