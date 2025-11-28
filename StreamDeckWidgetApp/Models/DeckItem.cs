using StreamDeckWidgetApp.Core; // ObservableObject burada

namespace StreamDeckWidgetApp.Models;

// ObservableObject'ten miras al�yoruz ki de�i�iklikleri UI'a haber versin
public class DeckItem : ObservableObject
{
    private string _id = Guid.NewGuid().ToString();
    private string _title = "Yeni Buton";
    private string _actionType = "Execute";
    private string _command = "";
    private string _color = "#333333";
    private int _row;
    private int _column;
    private string? _iconPath;
    private string? _icon; // Segoe MDL2 veya Emoji karakter
    private bool _isSelected;
    private string _behaviorType = "Push"; // "Push" veya "Toggle"
    private bool _isActive; // Toggle butonlar için runtime state (JSON'a kaydedilmez)

    public string Id
    {
        get => _id;
        set { _id = value; OnPropertyChanged(); }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; OnPropertyChanged(); }
    }
    
    public string BehaviorType
    {
        get => _behaviorType;
        set { _behaviorType = value; OnPropertyChanged(); }
    }
    
    public bool IsActive
    {
        get => _isActive;
        set { _isActive = value; OnPropertyChanged(); }
    }

    public string Title
    {
        get => _title;
        set 
        { 
            _title = value; 
            OnPropertyChanged(); 
        }
    }

    public string ActionType
    {
        get => _actionType;
        set { _actionType = value; OnPropertyChanged(); }
    }

    public string Command
    {
        get => _command;
        set { _command = value; OnPropertyChanged(); }
    }

    public string Color
    {
        get => _color;
        set { _color = value; OnPropertyChanged(); }
    }

    public int Row
    {
        get => _row;
        set { _row = value; OnPropertyChanged(); }
    }

    public int Column
    {
        get => _column;
        set { _column = value; OnPropertyChanged(); }
    }

    public string? IconPath
    {
        get => _iconPath;
        set 
        { 
            if (_iconPath != value) // Gereksiz tetiklemeyi önle
            {
                _iconPath = value;
                OnPropertyChanged(); // UI'a bildir
            }
        }
    }
    
    /// <summary>
    /// Segoe MDL2 Assets veya Emoji karakter ikonu
    /// IconPath boşken bu gösterilir
    /// </summary>
    public string? Icon
    {
        get => _icon;
        set 
        { 
            if (_icon != value)
            {
                _icon = value;
                OnPropertyChanged();
            }
        }
    }
}
