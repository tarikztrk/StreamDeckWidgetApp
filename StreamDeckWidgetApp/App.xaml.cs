using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StreamDeckWidgetApp.Abstractions;
using StreamDeckWidgetApp.Services;
using StreamDeckWidgetApp.ViewModels;
using StreamDeckWidgetApp.Views;
using Wpf.Ui;

namespace StreamDeckWidgetApp;

public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
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
            }).Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        // MainWindow'u host üzerinden çözümlüyoruz
        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();

        base.OnExit(e);
    }
}
