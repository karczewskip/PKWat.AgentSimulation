namespace PKWat.AgentSimulation.Core;

using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Core.Snapshots;
using System.Reflection;

public interface ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> where ENVIRONMENT : ISimulationEnvironment<ENVIRONMENT_STATE>
{
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddAgents<AGENT>(int number) where AGENT : ISimulationAgent<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddEnvironmentUpdates(Func<ISimulationContext<ENVIRONMENT>, Task> update);
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddCallback(Func<ISimulationContext<ENVIRONMENT>, Task> callback);
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> SetSimulationStep(TimeSpan simulationStep);
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> SetWaitingTimeBetweenSteps(TimeSpan waitingTimeBetweenSteps);
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> SetRandomSeed(int seed);
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddEvent<U>() where U : ISimulationEvent<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddEventWithInitialization<U>(Action<U> initialization) where U : ISimulationEvent<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddCrashCondition(Func<ISimulationContext<ENVIRONMENT>, SimulationCrashResult> crashCondition);

    ISimulation Build();
}

internal class SimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> : ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> where ENVIRONMENT : ISimulationEnvironment<ENVIRONMENT_STATE>
{
    private readonly ENVIRONMENT _simulationEnvironment;
    private readonly IServiceProvider _serviceProvider;

    private List<ISimulationAgent<ENVIRONMENT>> _agents = new();
    private List<Func<ISimulationAgent<ENVIRONMENT>>> _agentsToGenerate = new();
    private List<Func<ISimulationEvent<ENVIRONMENT>>> _eventsToGenerate = new();
    private List<Func<ISimulationContext<ENVIRONMENT>, Task>> _environmentUpdates = new();
    private List<Func<ISimulationContext<ENVIRONMENT>, Task>> _callbacks = new();
    private List<ISimulationEvent<ENVIRONMENT>> _events = new();
    private List<Func<ISimulationContext<ENVIRONMENT>, SimulationCrashResult>> _crashConditions = new();
    private TimeSpan _simulationStep = TimeSpan.FromSeconds(1);
    private TimeSpan _waitingTimeBetweenSteps = TimeSpan.Zero;
    private int? _randomSeed;

    public SimulationBuilderContext(ENVIRONMENT simulationEnvironment, IServiceProvider serviceProvider)
    {
        _simulationEnvironment = simulationEnvironment;
        _serviceProvider = serviceProvider;
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

    public ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddEnvironmentUpdates(Func<ISimulationContext<ENVIRONMENT>, Task> update)
    {
        _environmentUpdates.Add(update);

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
        _agents.AddRange(_agentsToGenerate.Select(x => x()));
        _events.AddRange(_eventsToGenerate.Select(x => x()));

        var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var snapshotDirectory = Path.Combine(binDirectory, "snapshots");

        var snapshotStore = new SimulationSnapshotStore(new SimulationSnapshotConfiguration(snapshotDirectory));

        return new Simulation<ENVIRONMENT, ENVIRONMENT_STATE>(
            new SimulationContext<ENVIRONMENT>(
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
