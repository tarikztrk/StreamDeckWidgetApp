using System.Diagnostics;
using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Services;

public class ActionService : IActionService
{
    public void ExecuteItem(DeckItem item)
    {
        // Ýleride buraya Switch-Case veya Strategy Pattern ile farklý aksiyon tipleri eklenebilir.
        // Þimdilik sadece "Process.Start" mantýðý kuralým.
        
        if (string.IsNullOrWhiteSpace(item.Command)) return;

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = item.Command,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            // Ýleride buraya bir Logger servisi enjekte edip loglayacaðýz.
            Debug.WriteLine($"Hata: {ex.Message}");
        }
    }
}
