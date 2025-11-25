using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Services;
using StreamDeckWidgetApp.ViewModels;

namespace StreamDeckWidgetApp;

public partial class App : Application
{
    public new static App Current => (App)Application.Current;
    public IServiceProvider Services { get; }

    public App()
    {
        Services = ConfigureServices();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Servisleri Kayýt Et
        services.AddSingleton<IActionService, ActionService>();
        services.AddSingleton<IConfigService, JsonConfigService>(); // JSON Konfigürasyon Servisi

        // ViewModel'leri Kayýt Et
        services.AddTransient<MainViewModel>();

        // View'larý Kayýt Et
        services.AddTransient<MainWindow>();

        return services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // MainWindow'u DI Container üzerinden çözümlüyoruz
        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
