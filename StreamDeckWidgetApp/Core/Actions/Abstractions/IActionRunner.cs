using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Core.Actions.Abstractions;

public interface IActionRunner
{
    // Hangi ActionType'ý yönettiðini belirtir (Örn: "Execute", "Website")
    string ActionType { get; } 
    
    // Ýþlemi yapar
    void Execute(DeckItem item);
}
