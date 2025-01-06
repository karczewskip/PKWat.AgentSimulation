namespace PKWat.AgentSimulation.Examples.PreyVsPredator;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation;
using System.Reflection;
using System.Windows;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IHost? AppHost { get; private set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContsct, services) =>
            {
                services.AddSingleton<MainWindow>();
                services.AddScoped<PreyVsPredatorSimulationBuilder>();
                services.AddScoped<PreyVsPredatorDrawer>();
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

