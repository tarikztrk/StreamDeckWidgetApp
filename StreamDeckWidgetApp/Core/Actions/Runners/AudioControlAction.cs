using WindowsInput;
using WindowsInput.Native;
using StreamDeckWidgetApp.Core.Actions.Abstractions;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Core.Actions.Runners;

/// <summary>
/// Ses kontrolü için Action Runner
/// Desteklenen komutlar: MUTE, VOL_UP, VOL_DOWN, VOL_SET_XX (XX = 0-100)
/// </summary>
public class AudioControlAction : IActionRunner
{
    public string ActionType => "AudioControl";
    
    private readonly InputSimulator _inputSimulator = new InputSimulator();

    public void Execute(DeckItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Command)) return;

        try
        {
            var command = item.Command.ToUpper();
            
            switch (command)
            {
                case "MUTE":
                case "TOGGLE_MUTE":
                    _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_MUTE);
                    break;
                    
                case "VOL_UP":
                case "VOLUME_UP":
                    // 5 kademe artır
                    for (int i = 0; i < 5; i++)
                    {
                        _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_UP);
                        System.Threading.Thread.Sleep(10);
                    }
                    break;
                    
                case "VOL_DOWN":
                case "VOLUME_DOWN":
                    // 5 kademe azalt
                    for (int i = 0; i < 5; i++)
                    {
                        _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_DOWN);
                        System.Threading.Thread.Sleep(10);
                    }
                    break;
                    
                default:
                    // VOL_SET_50 gibi komutlar için
                    if (command.StartsWith("VOL_SET_"))
                    {
                        // Windows'ta doğrudan ses seviyesi ayarlamak için NAudio veya CoreAudio gerekir
                        // Bu basit implementasyonda sadece log yazdırıyoruz
                        System.Diagnostics.Debug.WriteLine($"VOL_SET komutu henüz desteklenmiyor: {command}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Tanınmayan AudioControl komutu: {item.Command}");
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AudioControl Hatası: {ex.Message}");
        }
    }
}
