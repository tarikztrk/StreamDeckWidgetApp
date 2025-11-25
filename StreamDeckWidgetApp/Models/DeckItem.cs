using StreamDeckWidgetApp.Core; // ObservableObject burada

namespace StreamDeckWidgetApp.Models;

// ObservableObject'ten miras alýyoruz ki deðiþiklikleri UI'a haber versin
public class DeckItem : ObservableObject
{
    private string _id = Guid.NewGuid().ToString();
    private string _title = "Yeni Buton";
    private string _actionType = "Execute";
    private string _command = "";
    private string _color = "#333333";
    private int _row;
    private int _column;

    public string Id
    {
        get => _id;
        set { _id = value; OnPropertyChanged(); }
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
}
