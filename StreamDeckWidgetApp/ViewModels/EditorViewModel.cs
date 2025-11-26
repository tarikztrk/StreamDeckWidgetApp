using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using StreamDeckWidgetApp.Core;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.ViewModels;

/// <summary>
/// ViewModel for EditorWindow - shares data with MainViewModel
/// </summary>
public class EditorViewModel : ObservableObject
{
    private readonly MainViewModel _mainViewModel;
    private Window? _editorWindow;

    // --- Commands ---
    public ICommand CloseCommand { get; }
    public ICommand SaveCommand { get; }

    // --- Proxied Properties from MainViewModel ---
    public DeckItem? SelectedDeckItem
    {
        get => _mainViewModel.SelectedDeckItem;
        set => _mainViewModel.SelectedDeckItem = value;
    }

    public int Rows
    {
        get => _mainViewModel.Rows;
        set => _mainViewModel.Rows = value;
    }

    public int Columns
    {
        get => _mainViewModel.Columns;
        set => _mainViewModel.Columns = value;
    }

    public int SelectedButtonSize
    {
        get => _mainViewModel.SelectedButtonSize;
        set => _mainViewModel.SelectedButtonSize = value;
    }

    public Dictionary<string, int> ButtonSizeOptions => _mainViewModel.ButtonSizeOptions;
    public List<string> ActionTypes => _mainViewModel.ActionTypes;
    public ObservableCollection<DeckItem> DeckItems => _mainViewModel.DeckItems;

    public EditorViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;

        // Close command - notify MainViewModel to clean up
        CloseCommand = new RelayCommand(_ => CloseEditor());

        // Save command - delegate to MainViewModel
        SaveCommand = new RelayCommand(_ => _mainViewModel.SaveChanges());

        // Subscribe to MainViewModel property changes to update UI
        _mainViewModel.PropertyChanged += (s, e) =>
        {
            // Relay property change notifications
            if (e.PropertyName == nameof(MainViewModel.SelectedDeckItem))
                OnPropertyChanged(nameof(SelectedDeckItem));
            else if (e.PropertyName == nameof(MainViewModel.Rows))
                OnPropertyChanged(nameof(Rows));
            else if (e.PropertyName == nameof(MainViewModel.Columns))
                OnPropertyChanged(nameof(Columns));
            else if (e.PropertyName == nameof(MainViewModel.SelectedButtonSize))
                OnPropertyChanged(nameof(SelectedButtonSize));
        };
    }

    /// <summary>
    /// Set the window reference (called by MainViewModel.OpenEditor)
    /// </summary>
    public void SetWindow(Window window)
    {
        _editorWindow = window;
    }

    /// <summary>
    /// Close the editor window
    /// </summary>
    private void CloseEditor()
    {
        _editorWindow?.Close();
        _mainViewModel.OnEditorClosed();
    }
}
