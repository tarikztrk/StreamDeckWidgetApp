using WindowsInput;            // Kütüphane
using WindowsInput.Native;     // Tuþ Kodlarý (VirtualKeyCode)
using StreamDeckWidgetApp.Core.Actions.Abstractions;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Core.Actions.Runners;

public class HotkeyAction : IActionRunner
{
    public string ActionType => "Hotkey";
    
    // Simülatör nesnesini baþlatýyoruz
    private readonly InputSimulator _inputSimulator = new InputSimulator();

    public void Execute(DeckItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Command)) return;

        try
        {
            // Komutlarý daha anlaþýlýr (Human Readable) yapmak için basit bir haritalama yapýyoruz.
            // JSON'da "COPY" yazacaðýz, kod bunu "CTRL + C" olarak yorumlayacak.
            
            switch (item.Command.ToUpper())
            {
                case "COPY": // Kopyala
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_C);
                    break;
                    
                case "PASTE": // Yapýþtýr
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                    break;
                    
                case "CUT": // Kes
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_X);
                    break;
                    
                case "UNDO": // Geri Al (Ctrl+Z)
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_Z);
                    break;

                case "SCREENSHOT": // Win + Shift + S
                    _inputSimulator.Keyboard.ModifiedKeyStroke(
                        new[] { VirtualKeyCode.LWIN, VirtualKeyCode.SHIFT }, 
                        VirtualKeyCode.VK_S);
                    break;
                    
                case "VOL_UP": // Ses Aç
                    _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_UP);
                    break;
                    
                case "VOL_DOWN": // Ses Kýs
                    _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_DOWN);
                    break;
                    
                case "MUTE": // Sessize Al
                    _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_MUTE);
                    break;
                
                case "MEDIA_PLAY": // Oynat/Durdur
                    _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.MEDIA_PLAY_PAUSE);
                    break;

                // Eðer özel bir tuþ gelirse (Örn: "F5")
                default:
                    // Ýleride buraya daha geliþmiþ bir okuyucu yazacaðýz.
                    System.Diagnostics.Debug.WriteLine($"Tanýnmayan Komut: {item.Command}");
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Hotkey Hatasý: {ex.Message}");
        }
    }
}
