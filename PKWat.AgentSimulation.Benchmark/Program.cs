using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Benchmark.SimulationExample;
using PKWat.AgentSimulation.Core;

public class AllowNonOptimized : ManualConfig
{
    public AllowNonOptimized()
    {
        // Zezwól na brak optymalizacji JIT
        AddValidator(JitOptimizationsValidator.DontFailOnError);

        // Dodaj domyślnych eksporterów
        AddExporter(DefaultExporters.Csv);
        AddExporter(DefaultExporters.Html);
        AddExporter(DefaultExporters.Markdown);

        // Dodaj domyślnych loggerów
        AddLogger(ConsoleLogger.Default);

        // Dodaj domyślnych dostawców kolumn
        AddColumnProvider(DefaultColumnProviders.Instance);
    }
}

public class Test
{
    private IServiceProvider serviceProvider;

    public Test()
    {
        var services = new ServiceCollection();
        services.AddAgentSimulation(typeof(SimulationRunner).Assembly);
        services.AddScoped<SimulationRunner>();
        serviceProvider = services.BuildServiceProvider();
    }

    [Benchmark]
    public async Task Test1000()
    {
        using var scope = serviceProvider.CreateScope();

        await scope.ServiceProvider.GetRequiredService<SimulationRunner>().RunSimulation(1000);
    }

    [Benchmark]
    public async Task Test2000()
    {
        using var scope = serviceProvider.CreateScope();

        await scope.ServiceProvider.GetRequiredService<SimulationRunner>().RunSimulation(2000);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var config = new AllowNonOptimized();
        var summary = BenchmarkRunner.Run<Test>(config);
    }
}
