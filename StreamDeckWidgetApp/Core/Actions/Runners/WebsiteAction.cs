using System.Diagnostics;
using StreamDeckWidgetApp.Core.Actions.Abstractions;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Core.Actions.Runners;

public class WebsiteAction : IActionRunner
{
    public string ActionType => "Website";

    public void Execute(DeckItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Command)) return;

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = item.Command,
                UseShellExecute = true
            });
        }
        catch { /* Loglama eklenebilir */ }
    }
}
