using StreamDeckWidgetApp.Models;

namespace StreamDeckWidgetApp.Abstractions;

public interface IConfigService
{
    Profile LoadProfile();
    void SaveProfile(Profile profile);
}
