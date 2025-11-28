using WindowsInput;
using StreamDeckWidgetApp.Core.Actions.Abstractions;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Core.Actions.Runners;

/// <summary>
/// Metin yazma için Action Runner
/// Command alanındaki metni otomatik yazar (klavye simülasyonu)
/// </summary>
public class TextTypeAction : IActionRunner
{
    public string ActionType => "TextType";
    
    private readonly InputSimulator _inputSimulator = new InputSimulator();

    public void Execute(DeckItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Command)) return;

        try
        {
            // Metni yazabilmek için kısa bir gecikme
            System.Threading.Thread.Sleep(100);
            
            // Metni yaz
            _inputSimulator.Keyboard.TextEntry(item.Command);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"TextType Hatası: {ex.Message}");
        }
    }
}
