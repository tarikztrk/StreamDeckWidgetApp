using WindowsInput;            // Kütüphane
using WindowsInput.Native;     // Tuş Kodları (VirtualKeyCode)
using StreamDeckWidgetApp.Core.Actions.Abstractions;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Core.Actions.Runners;

public class HotkeyAction : IActionRunner
{
    public string ActionType => "Hotkey";
    
    // Simülatör nesnesini başlatıyoruz
    private readonly InputSimulator _inputSimulator = new InputSimulator();

    public void Execute(DeckItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Command)) return;

        try
        {
            var command = item.Command.ToUpper().Trim();
            
            // Önce hazır komutları kontrol et
            switch (command)
            {
                case "COPY":
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_C);
                    return;
                    
                case "PASTE":
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                    return;
                    
                case "CUT":
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_X);
                    return;
                    
                case "UNDO":
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_Z);
                    return;
                    
                case "REDO":
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_Y);
                    return;
                    
                case "SAVE":
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_S);
                    return;
                    
                case "SELECT_ALL":
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A);
                    return;
                    
                case "FIND":
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_F);
                    return;
                    
                case "NEW":
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_N);
                    return;
                    
                case "OPEN":
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_O);
                    return;
                    
                case "PRINT":
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_P);
                    return;
                    
                case "CLOSE":
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_W);
                    return;
                    
                case "REFRESH":
                    _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.F5);
                    return;

                case "SCREENSHOT":
                    _inputSimulator.Keyboard.ModifiedKeyStroke(
                        new[] { VirtualKeyCode.LWIN, VirtualKeyCode.SHIFT }, 
                        VirtualKeyCode.VK_S);
                    return;
                    
                case "TASK_MANAGER":
                    _inputSimulator.Keyboard.ModifiedKeyStroke(
                        new[] { VirtualKeyCode.CONTROL, VirtualKeyCode.SHIFT }, 
                        VirtualKeyCode.ESCAPE);
                    return;
                    
                case "ALT_TAB":
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.TAB);
                    return;
                    
                case "WIN_D": // Masaüstünü göster
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.VK_D);
                    return;
                    
                case "WIN_E": // Dosya Gezgini
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.VK_E);
                    return;
                    
                case "WIN_L": // Kilitle
                    _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.VK_L);
                    return;
                    
                case "VOL_UP":
                    _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_UP);
                    return;
                    
                case "VOL_DOWN":
                    _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_DOWN);
                    return;
                    
                case "MUTE":
                    _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_MUTE);
                    return;
                
                case "MEDIA_PLAY":
                    _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.MEDIA_PLAY_PAUSE);
                    return;
            }
            
            // Özel tuş kombinasyonu desteği: "CTRL+SHIFT+N" formatı
            if (command.Contains("+"))
            {
                ExecuteCustomHotkey(command);
                return;
            }
            
            // Tek tuş (F1-F12, ESC, ENTER vb.)
            if (TryGetSingleKey(command, out var singleKey))
            {
                _inputSimulator.Keyboard.KeyPress(singleKey);
                return;
            }
            
            System.Diagnostics.Debug.WriteLine($"Tanınmayan Hotkey Komutu: {item.Command}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Hotkey Hatası: {ex.Message}");
        }
    }
    
    /// <summary>
    /// "CTRL+SHIFT+N" gibi özel kombinasyonları çalıştır
    /// </summary>
    private void ExecuteCustomHotkey(string command)
    {
        var parts = command.Split('+');
        var modifiers = new List<VirtualKeyCode>();
        VirtualKeyCode? mainKey = null;
        
        foreach (var part in parts)
        {
            var key = part.Trim().ToUpper();
            
            // Modifier tuşları
            if (key == "CTRL" || key == "CONTROL")
            {
                modifiers.Add(VirtualKeyCode.CONTROL);
            }
            else if (key == "ALT")
            {
                modifiers.Add(VirtualKeyCode.MENU);
            }
            else if (key == "SHIFT")
            {
                modifiers.Add(VirtualKeyCode.SHIFT);
            }
            else if (key == "WIN" || key == "WINDOWS")
            {
                modifiers.Add(VirtualKeyCode.LWIN);
            }
            else
            {
                // Ana tuş
                if (TryGetSingleKey(key, out var k))
                {
                    mainKey = k;
                }
            }
        }
        
        if (mainKey.HasValue)
        {
            if (modifiers.Count > 0)
            {
                _inputSimulator.Keyboard.ModifiedKeyStroke(modifiers.ToArray(), mainKey.Value);
            }
            else
            {
                _inputSimulator.Keyboard.KeyPress(mainKey.Value);
            }
        }
    }
    
    /// <summary>
    /// Tek tuş string'ini VirtualKeyCode'a çevir
    /// </summary>
    private bool TryGetSingleKey(string key, out VirtualKeyCode keyCode)
    {
        keyCode = VirtualKeyCode.NONAME;
        
        // Function tuşları
        if (key.StartsWith("F") && key.Length <= 3)
        {
            if (int.TryParse(key.Substring(1), out int fNum) && fNum >= 1 && fNum <= 24)
            {
                keyCode = (VirtualKeyCode)(0x6F + fNum); // F1 = 0x70
                return true;
            }
        }
        
        // Özel tuşlar
        var specialKeys = new Dictionary<string, VirtualKeyCode>(StringComparer.OrdinalIgnoreCase)
        {
            ["ESC"] = VirtualKeyCode.ESCAPE,
            ["ESCAPE"] = VirtualKeyCode.ESCAPE,
            ["ENTER"] = VirtualKeyCode.RETURN,
            ["RETURN"] = VirtualKeyCode.RETURN,
            ["TAB"] = VirtualKeyCode.TAB,
            ["SPACE"] = VirtualKeyCode.SPACE,
            ["BACKSPACE"] = VirtualKeyCode.BACK,
            ["DELETE"] = VirtualKeyCode.DELETE,
            ["DEL"] = VirtualKeyCode.DELETE,
            ["INSERT"] = VirtualKeyCode.INSERT,
            ["INS"] = VirtualKeyCode.INSERT,
            ["HOME"] = VirtualKeyCode.HOME,
            ["END"] = VirtualKeyCode.END,
            ["PAGEUP"] = VirtualKeyCode.PRIOR,
            ["PAGEDOWN"] = VirtualKeyCode.NEXT,
            ["UP"] = VirtualKeyCode.UP,
            ["DOWN"] = VirtualKeyCode.DOWN,
            ["LEFT"] = VirtualKeyCode.LEFT,
            ["RIGHT"] = VirtualKeyCode.RIGHT,
            ["PRINT"] = VirtualKeyCode.SNAPSHOT,
            ["PRINTSCREEN"] = VirtualKeyCode.SNAPSHOT,
            ["PAUSE"] = VirtualKeyCode.PAUSE,
            ["NUMLOCK"] = VirtualKeyCode.NUMLOCK,
            ["CAPSLOCK"] = VirtualKeyCode.CAPITAL,
            ["SCROLLLOCK"] = VirtualKeyCode.SCROLL,
        };
        
        if (specialKeys.TryGetValue(key, out keyCode))
            return true;
        
        // Tek harf (A-Z)
        if (key.Length == 1 && char.IsLetter(key[0]))
        {
            keyCode = (VirtualKeyCode)(0x41 + (char.ToUpper(key[0]) - 'A'));
            return true;
        }
        
        // Rakam (0-9)
        if (key.Length == 1 && char.IsDigit(key[0]))
        {
            keyCode = (VirtualKeyCode)(0x30 + (key[0] - '0'));
            return true;
        }
        
        return false;
    }
}
