using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Core.Actions;
using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Services;

public class ActionService : IActionService
{
    private readonly ActionFactory _actionFactory;

    public ActionService()
    {
        _actionFactory = new ActionFactory();
    }

    public void ExecuteItem(DeckItem item)
    {
        // 1. Fabrikadan bu ActionType'ý (örn: "Website") bilen iþçiyi al
        var runner = _actionFactory.GetRunner(item.ActionType);

        // 2. Ýþçi varsa çalýþtýr
        if (runner != null)
        {
            runner.Execute(item);
        }
        else
        {
            // Tanýmsýz aksiyon tipi (Loglanabilir)
            System.Diagnostics.Debug.WriteLine($"Tanýnmayan ActionType: {item.ActionType}");
        }
    }
}
