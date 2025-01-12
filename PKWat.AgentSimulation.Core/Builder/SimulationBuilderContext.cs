namespace PKWat.AgentSimulation.Core.Builder;

using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Snapshots;
using PKWat.AgentSimulation.Core.Stage;
using System.Reflection;

public interface ISimulationBuilderContext<ENVIRONMENT> where ENVIRONMENT : ISimulationEnvironment
{
    ISimulationBuilderContext<ENVIRONMENT> AddAgent<AGENT>() where AGENT : ISimulationAgent;
    ISimulationBuilderContext<ENVIRONMENT> AddAgents<AGENT>(int number) where AGENT : ISimulationAgent;
    ISimulationBuilderContext<ENVIRONMENT> AddCallback(Func<ISimulationContext<ENVIRONMENT>, Task> callback);
    ISimulationBuilderContext<ENVIRONMENT> AddCallback(Action<ISimulationContext<ENVIRONMENT>> callback);
    ISimulationBuilderContext<ENVIRONMENT> SetSimulationStep(TimeSpan simulationStep);
    ISimulationBuilderContext<ENVIRONMENT> SetWaitingTimeBetweenSteps(TimeSpan waitingTimeBetweenSteps);
    ISimulationBuilderContext<ENVIRONMENT> SetRandomSeed(int seed);
    ISimulationBuilderContext<ENVIRONMENT> AddInitializationStage<U>() where U : ISimulationStage<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT> AddInitializationStage<U>(Action<U> initialization) where U : ISimulationStage<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT> AddStage<U>() where U : ISimulationStage<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT> AddStage<U>(Action<U> initialization) where U : ISimulationStage<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT> WithSnapshots();

    ISimulation Build();
}

internal class SimulationBuilderContext<ENVIRONMENT> : ISimulationBuilderContext<ENVIRONMENT> where ENVIRONMENT : ISimulationEnvironment
{
    private readonly IServiceProvider _serviceProvider;

    private List<Func<ISimulationAgent>> _agentsToGenerate = new();
    private List<Func<ISimulationStage<ENVIRONMENT>>> _initializationStagesToGenerate = new();
    private List<Func<ISimulationStage<ENVIRONMENT>>> _stagesToGenerate = new();
    private List<Func<ISimulationContext<ENVIRONMENT>, Task>> _callbacks = new();
    private TimeSpan _simulationStep = TimeSpan.Zero;
    private TimeSpan _waitingTimeBetweenSteps = TimeSpan.Zero;
    private int? _randomSeed;
    private bool _doSnapshot = false;

    public SimulationBuilderContext(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISimulationBuilderContext<ENVIRONMENT> AddAgent<U>() where U : ISimulationAgent
    {
        return AddAgents<U>(1);
    }

    public ISimulationBuilderContext<ENVIRONMENT> AddAgents<U>(int number) where U : ISimulationAgent
    {
        _agentsToGenerate.AddRange(Enumerable
            .Range(0, number)
            .Select(x => new Func<ISimulationAgent>(
                () => _serviceProvider.GetRequiredService<U>())));

        return this;
    }

    public ISimulationBuilderContext<ENVIRONMENT> AddCallback(Func<ISimulationContext<ENVIRONMENT>, Task> callback)
    {
        _callbacks.Add(callback);

        return this;
    }

    public ISimulationBuilderContext<ENVIRONMENT> AddCallback(Action<ISimulationContext<ENVIRONMENT>> callback)
    {
        _callbacks.Add(async c => callback(c));

        return this;
    }

    public ISimulationBuilderContext<ENVIRONMENT> SetSimulationStep(TimeSpan simulationStep)
    {
        _simulationStep = simulationStep;

        return this;
    }

    public ISimulationBuilderContext<ENVIRONMENT> SetWaitingTimeBetweenSteps(TimeSpan waitingTimeBetweenSteps)
    {
        _waitingTimeBetweenSteps = waitingTimeBetweenSteps;

        return this;
    }

    public ISimulationBuilderContext<ENVIRONMENT> SetRandomSeed(int seed)
    {
        _randomSeed = seed;

        return this;
    }

    public ISimulationBuilderContext<ENVIRONMENT> WithSnapshots()
    {
        _doSnapshot = true;
        return this;
    }

    public ISimulation Build()
    {
        _serviceProvider.GetRequiredService<RandomNumbersGeneratorFactory>().Initialize(_randomSeed);
        var simulationEnvironment = _serviceProvider.GetRequiredService<ENVIRONMENT>();
        var agents = _agentsToGenerate.Select(x => x()).ToArray();
        var initializationStages = _initializationStagesToGenerate.Select(x => x()).ToArray();
        var stages = _stagesToGenerate.Select(x => x()).ToArray();

        return new Simulation<ENVIRONMENT>(
            new SimulationContext<ENVIRONMENT>(
                _serviceProvider,
                simulationEnvironment,
                agents,
                _simulationStep,
                _waitingTimeBetweenSteps),
            CreateSimulationSnapshotStore(),
            _callbacks,
            initializationStages,
            stages);
    }

    private ISimulationSnapshotStore CreateSimulationSnapshotStore()
    {
        if (_doSnapshot)
        {
            var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var snapshotDirectory = Path.Combine(binDirectory, "snapshots");
            return new SimulationSnapshotStore(new SimulationSnapshotConfiguration(snapshotDirectory));
        }

        return new NullSimulationSnapshotStore();
    }

    public ISimulationBuilderContext<ENVIRONMENT> AddInitializationStage<U>() where U : ISimulationStage<ENVIRONMENT>
        => AddInitializationStage<U>(_ => { });

    public ISimulationBuilderContext<ENVIRONMENT> AddInitializationStage<U>(Action<U> initialization) where U : ISimulationStage<ENVIRONMENT>
    {
        _initializationStagesToGenerate.Add(() =>
        {
            var stage = _serviceProvider.GetRequiredService<U>();
            initialization(stage);

            return stage;
        });

        return this;
    }

    public ISimulationBuilderContext<ENVIRONMENT> AddStage<U>() where U : ISimulationStage<ENVIRONMENT>
        => AddStage<U>(_ => { });

    public ISimulationBuilderContext<ENVIRONMENT> AddStage<U>(Action<U> initialization) where U : ISimulationStage<ENVIRONMENT>
    {
        _stagesToGenerate.Add(() =>
        {
            var stage = _serviceProvider.GetRequiredService<U>();
            initialization(stage);

            return stage;
        });

        return this;
    }
}
