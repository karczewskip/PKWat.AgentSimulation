namespace PKWat.AgentSimulation.ExamplesVisualizer;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Drawing;
using PKWat.AgentSimulation.ExamplesVisualizer.Simulations;
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
                services.AddSingleton<ColorsGenerator>();
                services.AddBuilders();
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

public static class RegisterExtensions
{
    public static void AddBuilders(this IServiceCollection services)
    {
        Type[] registeringGenericTypes = [
            typeof(IExampleSimulationBuilder),
            typeof(IVisualizationDrawer)];

        services.AddTransient<SimulationsBuilder>();

        foreach (var type in Assembly.GetCallingAssembly().GetTypes().Where(type => !type.IsAbstract && !type.IsInterface))
        {
            var interfaces = type.GetInterfaces();
            if (interfaces.Any(i => registeringGenericTypes.Contains(i)))
            {
                services.AddTransient(type);
            }
        }
    }
}


