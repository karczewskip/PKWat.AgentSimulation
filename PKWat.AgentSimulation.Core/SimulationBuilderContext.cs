namespace PKWat.AgentSimulation.Core;

using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Core.Snapshots;
using System.Reflection;

public interface ISimulationBuilderContext<ENVIRONMENT> where ENVIRONMENT : ISimulationEnvironment
{
    ISimulationBuilderContext<ENVIRONMENT> AddAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT> AddAgents<AGENT>(int number) where AGENT : ISimulationAgent<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT> AddEnvironmentUpdates(Func<ISimulationContext<ENVIRONMENT>, Task> update);
    ISimulationBuilderContext<ENVIRONMENT> AddCallback(Func<ISimulationContext<ENVIRONMENT>, Task> callback);
    ISimulationBuilderContext<ENVIRONMENT> SetSimulationStep(TimeSpan simulationStep);
    ISimulationBuilderContext<ENVIRONMENT> SetWaitingTimeBetweenSteps(TimeSpan waitingTimeBetweenSteps);
    ISimulationBuilderContext<ENVIRONMENT> SetRandomSeed(int seed);
    ISimulationBuilderContext<ENVIRONMENT> AddEvent<U>() where U : ISimulationEvent<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT> AddEventWithInitialization<U>(Action<U> initialization) where U : ISimulationEvent<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT> AddCrashCondition(Func<ISimulationContext<ENVIRONMENT>, SimulationCrashResult> crashCondition);

    ISimulation Build();
}

internal class SimulationBuilderContext<T> : ISimulationBuilderContext<T> where T : ISimulationEnvironment
{
    private readonly T _simulationEnvironment;
    private readonly IServiceProvider _serviceProvider;

    private List<ISimulationAgent<T>> _agents = new();
    private List<Func<ISimulationAgent<T>>> _agentsToGenerate = new();
    private List<Func<ISimulationEvent<T>>> _eventsToGenerate = new();
    private List<Func<ISimulationContext<T>, Task>> _environmentUpdates = new();
    private List<Func<ISimulationContext<T>, Task>> _callbacks = new();
    private List<ISimulationEvent<T>> _events = new();
    private List<Func<ISimulationContext<T>, SimulationCrashResult>> _crashConditions = new();
    private TimeSpan _simulationStep = TimeSpan.FromSeconds(1);
    private TimeSpan _waitingTimeBetweenSteps = TimeSpan.Zero;
    private int? _randomSeed;

    public SimulationBuilderContext(T simulationEnvironment, IServiceProvider serviceProvider)
    {
        _simulationEnvironment = simulationEnvironment;
        _serviceProvider = serviceProvider;
    }

    public ISimulationBuilderContext<T> AddAgent<U>() where U : ISimulationAgent<T>
    {
        return AddAgents<U>(1);
    }

    public ISimulationBuilderContext<T> AddAgents<U>(int number) where U : ISimulationAgent<T>
    {
        _agentsToGenerate.AddRange(Enumerable
            .Range(0, number)
            .Select(x => new Func<ISimulationAgent<T>>(
                () => _serviceProvider.GetRequiredService<U>())));

        return this;
    }

    public ISimulationBuilderContext<T> AddEnvironmentUpdates(Func<ISimulationContext<T>, Task> update)
    {
        _environmentUpdates.Add(update);

        return this;
    }

    public ISimulationBuilderContext<T> AddCallback(Func<ISimulationContext<T>, Task> callback)
    {
        _callbacks.Add(callback);

        return this;
    }

    public ISimulationBuilderContext<T> SetSimulationStep(TimeSpan simulationStep)
    {
        _simulationStep = simulationStep;

        return this;
    }

    public ISimulationBuilderContext<T> SetWaitingTimeBetweenSteps(TimeSpan waitingTimeBetweenSteps)
    {
        _waitingTimeBetweenSteps = waitingTimeBetweenSteps;

        return this;
    }

    public ISimulationBuilderContext<T> SetRandomSeed(int seed)
    {
        _randomSeed = seed;

        return this;
    }

    public ISimulationBuilderContext<T> AddEvent<U>() where U : ISimulationEvent<T>
        => AddEventWithInitialization<U>(_ => { });

    public ISimulationBuilderContext<T> AddEventWithInitialization<U>(Action<U> initialization) where U : ISimulationEvent<T>
    {
        var newEvent = _serviceProvider.GetRequiredService<U>();
        initialization(newEvent);

        _eventsToGenerate.Add(() => newEvent);
        return this;
    }

    public ISimulationBuilderContext<T> AddCrashCondition(Func<ISimulationContext<T>, SimulationCrashResult> crashCondition)
    {
        _crashConditions.Add(crashCondition);

        return this;
    }

    public ISimulation Build()
    {
        _serviceProvider.GetRequiredService<RandomNumbersGeneratorFactory>().Initialize(_randomSeed);
        _agents.AddRange(_agentsToGenerate.Select(x => x()));
        _events.AddRange(_eventsToGenerate.Select(x => x()));

        var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var snapshotDirectory = Path.Combine(binDirectory, "snapshots");

        var snapshotStore = new SimulationSnapshotStore(new SimulationSnapshotConfiguration(snapshotDirectory));

        return new Simulation<T>(
            new SimulationContext<T>(
                _serviceProvider, 
                _simulationEnvironment ,
                _agents, 
                _simulationStep, 
                _waitingTimeBetweenSteps), 
            snapshotStore, 
            _environmentUpdates, 
            _callbacks, 
            _events,
            _crashConditions);
    }
}
