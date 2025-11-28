using WindowsInput;
using WindowsInput.Native;
using StreamDeckWidgetApp.Core.Actions.Abstractions;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Core.Actions.Runners;

/// <summary>
/// Medya kontrol komutları için Action Runner
/// Desteklenen komutlar: PLAY_PAUSE, NEXT, PREV, STOP
/// </summary>
public class MediaControlAction : IActionRunner
{
    public string ActionType => "MediaControl";
    
    private readonly InputSimulator _inputSimulator = new InputSimulator();

    public void Execute(DeckItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Command)) return;

        try
        {
            switch (item.Command.ToUpper())
            {
                case "PLAY_PAUSE":
                case "MEDIA_PLAY_PAUSE":
                    _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.MEDIA_PLAY_PAUSE);
                    break;
                    
                case "NEXT":
                case "NEXT_TRACK":
                case "MEDIA_NEXT":
                    _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.MEDIA_NEXT_TRACK);
                    break;
                    
                case "PREV":
                case "PREV_TRACK":
                case "MEDIA_PREV":
                    _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.MEDIA_PREV_TRACK);
                    break;
                    
                case "STOP":
                case "MEDIA_STOP":
                    _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.MEDIA_STOP);
                    break;
                    
                default:
                    System.Diagnostics.Debug.WriteLine($"Tanınmayan MediaControl komutu: {item.Command}");
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MediaControl Hatası: {ex.Message}");
        }
    }
}
