using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Abstractions;

public interface IActionService
{
    void ExecuteItem(DeckItem item);
}
