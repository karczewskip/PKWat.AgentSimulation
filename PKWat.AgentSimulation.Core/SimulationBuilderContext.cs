namespace PKWat.AgentSimulation.Core;

using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Core.Snapshots;
using System.Reflection;

public interface ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> where ENVIRONMENT : ISimulationEnvironment<ENVIRONMENT_STATE>
{
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddAgents<AGENT>(int number) where AGENT : ISimulationAgent<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddCallback(Func<ISimulationContext<ENVIRONMENT>, Task> callback);
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> SetSimulationStep(TimeSpan simulationStep);
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> SetWaitingTimeBetweenSteps(TimeSpan waitingTimeBetweenSteps);
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> SetRandomSeed(int seed);
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddEvent<U>() where U : ISimulationEvent<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddEventWithInitialization<U>(Action<U> initialization) where U : ISimulationEvent<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddCrashCondition(Func<ISimulationContext<ENVIRONMENT>, SimulationCrashResult> crashCondition);

    ISimulation Build();
}

internal class SimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> : ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> where ENVIRONMENT : ISimulationEnvironment<ENVIRONMENT_STATE> where ENVIRONMENT_STATE : ISnapshotCreator
{
    private readonly IServiceProvider _serviceProvider;

    private Func<(ENVIRONMENT, ENVIRONMENT_STATE)> _environmentCreate;
    private List<Func<ISimulationAgent<ENVIRONMENT>>> _agentsToGenerate = new();
    private List<Func<ISimulationEvent<ENVIRONMENT>>> _eventsToGenerate = new();
    private List<Func<ISimulationContext<ENVIRONMENT>, Task>> _callbacks = new();
    private List<Func<ISimulationContext<ENVIRONMENT>, SimulationCrashResult>> _crashConditions = new();
    private TimeSpan _simulationStep = TimeSpan.FromSeconds(1);
    private TimeSpan _waitingTimeBetweenSteps = TimeSpan.Zero;
    private int? _randomSeed;

    public SimulationBuilderContext(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> LoadState(ENVIRONMENT_STATE environmentState)
    {
        _environmentCreate = () => 
        {
            var environment = _serviceProvider.GetRequiredService<ENVIRONMENT>();

            return (environment, environmentState);
        };

        return this;
    }

    public ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddAgent<U>() where U : ISimulationAgent<ENVIRONMENT>
    {
        return AddAgents<U>(1);
    }

    public ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddAgents<U>(int number) where U : ISimulationAgent<ENVIRONMENT>
    {
        _agentsToGenerate.AddRange(Enumerable
            .Range(0, number)
            .Select(x => new Func<ISimulationAgent<ENVIRONMENT>>(
                () => _serviceProvider.GetRequiredService<U>())));

        return this;
    }

    public ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddCallback(Func<ISimulationContext<ENVIRONMENT>, Task> callback)
    {
        _callbacks.Add(callback);

        return this;
    }

    public ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> SetSimulationStep(TimeSpan simulationStep)
    {
        _simulationStep = simulationStep;

        return this;
    }

    public ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> SetWaitingTimeBetweenSteps(TimeSpan waitingTimeBetweenSteps)
    {
        _waitingTimeBetweenSteps = waitingTimeBetweenSteps;

        return this;
    }

    public ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> SetRandomSeed(int seed)
    {
        _randomSeed = seed;

        return this;
    }

    public ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddEvent<U>() where U : ISimulationEvent<ENVIRONMENT>
        => AddEventWithInitialization<U>(_ => { });

    public ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddEventWithInitialization<U>(Action<U> initialization) where U : ISimulationEvent<ENVIRONMENT>
    {
        _eventsToGenerate.Add(() => 
            {
                var newEvent = _serviceProvider.GetRequiredService<U>();
                initialization(newEvent);

                return newEvent;
            });

        return this;
    }

    public ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddCrashCondition(Func<ISimulationContext<ENVIRONMENT>, SimulationCrashResult> crashCondition)
    {
        _crashConditions.Add(crashCondition);

        return this;
    }

    public ISimulation Build()
    {
        _serviceProvider.GetRequiredService<RandomNumbersGeneratorFactory>().Initialize(_randomSeed);
        var (simulationEnvironment, simulationEnvironmentState) = _environmentCreate();
        var agents = _agentsToGenerate.Select(x => x()).ToArray();
        var events = _eventsToGenerate.Select(x => x()).ToArray();

        var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var snapshotDirectory = Path.Combine(binDirectory, "snapshots");

        var snapshotStore = new SimulationSnapshotStore(new SimulationSnapshotConfiguration(snapshotDirectory));

        return new Simulation<ENVIRONMENT, ENVIRONMENT_STATE>(
            new SimulationContext<ENVIRONMENT, ENVIRONMENT_STATE>(
                _serviceProvider,
                simulationEnvironment,
                simulationEnvironmentState,
                agents, 
                _simulationStep, 
                _waitingTimeBetweenSteps), 
            snapshotStore,
            _callbacks,
            events,
            _crashConditions);
    }
}
