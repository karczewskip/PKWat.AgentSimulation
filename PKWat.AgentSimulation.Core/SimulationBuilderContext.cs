namespace PKWat.AgentSimulation.Core;

using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Snapshots;
using System.Reflection;

public interface ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> where ENVIRONMENT : ISimulationEnvironment<ENVIRONMENT_STATE>
{
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddAgents<AGENT>(int number) where AGENT : ISimulationAgent<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddEnvironmentUpdates(Func<ISimulationContext<ENVIRONMENT>, Task> update);
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddEnvironmentInitialization(Func<ISimulationContext<ENVIRONMENT>, Task> initialization);
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddCallback(Func<ISimulationContext<ENVIRONMENT>, Task> callback);
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddCallback(Action<ISimulationContext<ENVIRONMENT>> callback);
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> SetSimulationStep(TimeSpan simulationStep);
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> SetWaitingTimeBetweenSteps(TimeSpan waitingTimeBetweenSteps);
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> SetRandomSeed(int seed);
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddEvent<U>() where U : ISimulationEvent<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddEventWithInitialization<U>(Action<U> initialization) where U : ISimulationEvent<ENVIRONMENT>;
    ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> WithSnapshots();

    ISimulation Build();
}

internal class SimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> : ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> where ENVIRONMENT : ISimulationEnvironment<ENVIRONMENT_STATE>
{
    private readonly IServiceProvider _serviceProvider;

    private Func<ENVIRONMENT> _environmentCreate;
    private List<Func<ISimulationAgent<ENVIRONMENT>>> _agentsToGenerate = new();
    private List<Func<ISimulationEvent<ENVIRONMENT>>> _eventsToGenerate = new();
    private List<Func<ISimulationContext<ENVIRONMENT>, Task>> _environmentUpdates = new();
    private Func<ISimulationContext<ENVIRONMENT>, Task> _environmentInitilization = async c => { };
    private List<Func<ISimulationContext<ENVIRONMENT>, Task>> _callbacks = new();
    private TimeSpan _simulationStep = TimeSpan.FromSeconds(1);
    private TimeSpan _waitingTimeBetweenSteps = TimeSpan.Zero;
    private int? _randomSeed;
    private bool _doSnapshot = false;

    public SimulationBuilderContext(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> LoadState(ENVIRONMENT_STATE environmentState)
    {
        _environmentCreate = () =>
        {
            var environment = _serviceProvider.GetRequiredService<ENVIRONMENT>();
            environment.LoadState(environmentState);

            return environment;
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

    public ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddEnvironmentUpdates(Func<ISimulationContext<ENVIRONMENT>, Task> update)
    {
        _environmentUpdates.Add(update);

        return this;
    }

    public ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddEnvironmentInitialization(Func<ISimulationContext<ENVIRONMENT>, Task> initialization)
    {
        _environmentInitilization = initialization;

        return this;
    }

    public ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddCallback(Func<ISimulationContext<ENVIRONMENT>, Task> callback)
    {
        _callbacks.Add(callback);

        return this;
    }

    public ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> AddCallback(Action<ISimulationContext<ENVIRONMENT>> callback)
    {
        _callbacks.Add(async c => callback(c));

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

    public ISimulationBuilderContext<ENVIRONMENT, ENVIRONMENT_STATE> WithSnapshots()
    {
        _doSnapshot = true;
        return this;
    }

    public ISimulation Build()
    {
        _serviceProvider.GetRequiredService<RandomNumbersGeneratorFactory>().Initialize(_randomSeed);
        var simulationEnvironment = _environmentCreate();
        var agents = _agentsToGenerate.Select(x => x()).ToArray();
        var events = _eventsToGenerate.Select(x => x()).ToArray();

        return new Simulation<ENVIRONMENT, ENVIRONMENT_STATE>(
            new SimulationContext<ENVIRONMENT>(
                _serviceProvider,
                simulationEnvironment,
                agents,
                _simulationStep,
                _waitingTimeBetweenSteps),
            CreateSimulationSnapshotStore(),
            _environmentUpdates,
            _environmentInitilization,
            _callbacks,
            events);
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
}
