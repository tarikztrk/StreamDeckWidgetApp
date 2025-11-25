using System.Diagnostics;
using StreamDeckWidgetApp.Core.Actions.Abstractions;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Core.Actions.Runners;

public class ExecuteAppAction : IActionRunner
{
    public string ActionType => "Execute";

    public void Execute(DeckItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Command)) return;

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = item.Command,
                UseShellExecute = true,
                WorkingDirectory = System.IO.Path.GetDirectoryName(item.Command) // Çalýþma dizinini ayarla
            });
        }
        catch { /* Loglama eklenebilir */ }
    }
}
