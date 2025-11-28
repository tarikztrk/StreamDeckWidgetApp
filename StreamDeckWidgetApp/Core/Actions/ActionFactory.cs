using StreamDeckWidgetApp.Core.Actions.Abstractions;
using StreamDeckWidgetApp.Core.Actions.Runners;

namespace StreamDeckWidgetApp.Core.Actions;

public class ActionFactory
{
    private readonly Dictionary<string, IActionRunner> _runners;

    public ActionFactory()
    {
        // Mevcut tüm aksiyonları listeye ekliyoruz
        _runners = new Dictionary<string, IActionRunner>(StringComparer.OrdinalIgnoreCase);

        Register(new ExecuteAppAction());
        Register(new WebsiteAction());
        Register(new HotkeyAction());
        Register(new MediaControlAction());
        Register(new AudioControlAction());
        Register(new TextTypeAction());
    }

    private void Register(IActionRunner runner)
    {
        _runners[runner.ActionType] = runner;
    }

    public IActionRunner? GetRunner(string actionType)
    {
        if (_runners.TryGetValue(actionType, out var runner))
        {
            return runner;
        }
        return null; // Tanımsız aksiyon
    }
    
    /// <summary>
    /// Tüm kayıtlı action tiplerini döndürür
    /// </summary>
    public IEnumerable<string> GetAllActionTypes()
    {
        return _runners.Keys;
    }
}
