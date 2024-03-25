namespace PKWat.AgentSimulation.Examples.ButterflyEffect;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Drawing;
using System.Reflection;
using System.Windows;

public partial class App : Application
{
    public static IHost? AppHost { get; private set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<MainWindow>();
                services.AddSingleton<BouncingBallStateInitializer>();
                services.AddSingleton<ColorsGenerator>();
                services.AddSingleton<PictureRenderer>();
                services.AddAgentSimulation(Assembly.GetAssembly(typeof(App)));
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
        startupForm.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();

        base.OnExit(e);
    }
}
