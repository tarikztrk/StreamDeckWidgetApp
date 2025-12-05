using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Services;
using StreamDeckWidgetApp.ViewModels;
using StreamDeckWidgetApp.Views;
using Wpf.Ui.Appearance;

namespace StreamDeckWidgetApp;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public App()
    {
        Services = ConfigureServices();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Servis Katmanı Kayıtları
        services.AddSingleton<IActionService, ActionService>();
        services.AddSingleton<IConfigService, JsonConfigService>();

        // ViewModel'leri Kayıt Et
        services.AddSingleton<MainViewModel>();
        services.AddTransient<EditorViewModel>();

        // View'ları Kayıt Et
        services.AddTransient<MainWindow>();
        services.AddTransient<EditorWindow>();

        // Func factories for MainViewModel
        services.AddSingleton<Func<EditorViewModel>>(sp => () => sp.GetRequiredService<EditorViewModel>());
        services.AddSingleton<Func<EditorWindow>>(sp => () => sp.GetRequiredService<EditorWindow>());

        // App singleton
        services.AddSingleton<Application>(sp => (App)Application.Current);

        return services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // MainWindow'u DI Container üzerinden çözümlüyoruz
        var mainWindow = Services.GetRequiredService<MainWindow>();

        // Wpf.Ui Tema Motorunu Başlat - Dark Tema Zorla
        SystemThemeWatcher.Watch(mainWindow);
        ApplicationThemeManager.Apply(ApplicationTheme.Dark);

        mainWindow.Show();
    }
}
