namespace PKWat.AgentSimulation.Core.Builder;

using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Crash;
using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.Core.Event;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Snapshots;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Core.Time;
using System.Reflection;

public interface ISimulationBuilderContext
{
    ISimulationBuilderContext AddAgent<AGENT>() where AGENT : ISimulationAgent;
    ISimulationBuilderContext AddAgents<AGENT>(int number) where AGENT : ISimulationAgent;
    ISimulationBuilderContext AddCallback(Func<ISimulationContext, Task> callback);
    ISimulationBuilderContext AddCallback(Action<ISimulationContext> callback);
    ISimulationBuilderContext SetSimulationStep(TimeSpan simulationStep);
    ISimulationBuilderContext UseCalendar();
    ISimulationBuilderContext SetWaitingTimeBetweenSteps(TimeSpan waitingTimeBetweenSteps);
    ISimulationBuilderContext SetRandomSeed(int seed);
    ISimulationBuilderContext AddInitializationStage<U>() where U : ISimulationStage;
    ISimulationBuilderContext AddInitializationStage<U>(Action<U> initialization) where U : ISimulationStage;
    ISimulationBuilderContext AddStage<U>() where U : ISimulationStage;
    ISimulationBuilderContext AddStage<U>(Action<U> initialization) where U : ISimulationStage;
    ISimulationBuilderContext AddInitializationEvent<U>() where U : ISimulationEvent;
    ISimulationBuilderContext AddInitializationEvent<U>(Action<U> initialization) where U : ISimulationEvent;
    ISimulationBuilderContext AddCrashCondition(Func<ISimulationContext, SimulationCrashResult> crashCondition);
    ISimulationBuilderContext WithSnapshots();

    ISimulation Build();
}

internal class SimulationBuilderContext(
    IServiceProvider serviceProvider, 
    Type environmentType) : ISimulationBuilderContext
{
    private List<Func<ISimulationAgent>> _agentsToGenerate = new();
    private List<Func<ISimulationStage>> _initializationStagesToGenerate = new();
    private List<Func<ISimulationEvent>> _initializationEventsToGenerate = new();
    private List<Func<ISimulationStage>> _stagesToGenerate = new();
    private List<Func<ISimulationContext, Task>> _callbacks = new();
    private List<Func<ISimulationContext, SimulationCrashResult>> _crashConditions = new();
    private ISimulationTimeMover _simulationTimeMover = new IntervalBasedSimulationTimeMover(TimeSpan.Zero);
    private TimeSpan _waitingTimeBetweenSteps = TimeSpan.Zero;
    private int? _randomSeed;
    private bool _doSnapshot = false;

    public ISimulationBuilderContext AddAgent<U>() where U : ISimulationAgent
    {
        return AddAgents<U>(1);
    }

    public ISimulationBuilderContext AddAgents<U>(int number) where U : ISimulationAgent
    {
        _agentsToGenerate.AddRange(Enumerable
            .Range(0, number)
            .Select(x => new Func<ISimulationAgent>(
                () => serviceProvider.GetRequiredService<U>())));

        return this;
    }

    public ISimulationBuilderContext AddCallback(Func<ISimulationContext, Task> callback)
    {
        _callbacks.Add(callback);

        return this;
    }

    public ISimulationBuilderContext AddCallback(Action<ISimulationContext> callback)
    {
        _callbacks.Add(async c => callback(c));

        return this;
    }

    public ISimulationBuilderContext SetSimulationStep(TimeSpan simulationStep)
    {
        _simulationTimeMover = new IntervalBasedSimulationTimeMover(simulationStep);

        return this;
    }

    public ISimulationBuilderContext UseCalendar()
    {
        _simulationTimeMover = new CalendarSimulationTimeMover(serviceProvider.GetRequiredService<ISimulationCalendar>());

        return this;
    }

    public ISimulationBuilderContext SetWaitingTimeBetweenSteps(TimeSpan waitingTimeBetweenSteps)
    {
        _waitingTimeBetweenSteps = waitingTimeBetweenSteps;

        return this;
    }

    public ISimulationBuilderContext SetRandomSeed(int seed)
    {
        _randomSeed = seed;

        return this;
    }

    public ISimulationBuilderContext WithSnapshots()
    {
        _doSnapshot = true;
        return this;
    }

    public ISimulation Build()
    {
        serviceProvider.GetRequiredService<RandomNumbersGeneratorFactory>().Initialize(_randomSeed);
        var simulationEnvironment = (ISimulationEnvironment)serviceProvider.GetRequiredService(environmentType);
        var agents = _agentsToGenerate.Select(x => x()).ToArray();
        var initializationStages = _initializationStagesToGenerate.Select(x => x()).ToArray();
        var stages = _stagesToGenerate.Select(x => x()).ToArray();
        var initializationEvents = _initializationEventsToGenerate.Select(x => x()).ToArray();
        var eventStore = serviceProvider.GetRequiredService<SimulationEventStore>();

        return new Simulation(
            new SimulationContext(
                serviceProvider,
                simulationEnvironment,
                agents,
                _simulationTimeMover,
                _waitingTimeBetweenSteps),
            CreateSimulationSnapshotStore(),
            _callbacks,
            initializationStages,
            stages,
            initializationEvents,
            eventStore,
            _crashConditions);
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

    public ISimulationBuilderContext AddInitializationStage<U>() where U : ISimulationStage
        => AddInitializationStage<U>(_ => { });

    public ISimulationBuilderContext AddInitializationStage<U>(Action<U> initialization) where U : ISimulationStage
    {
        _initializationStagesToGenerate.Add(() =>
        {
            var stage = serviceProvider.GetRequiredService<U>();
            initialization(stage);

            return stage;
        });

        return this;
    }

    public ISimulationBuilderContext AddStage<U>() where U : ISimulationStage
        => AddStage<U>(_ => { });

    public ISimulationBuilderContext AddStage<U>(Action<U> initialization) where U : ISimulationStage
    {
        _stagesToGenerate.Add(() =>
        {
            var stage = serviceProvider.GetRequiredService<U>();
            initialization(stage);

            return stage;
        });

        return this;
    }

    public ISimulationBuilderContext AddInitializationEvent<U>() where U : ISimulationEvent
        => AddInitializationEvent<U>(_ => { });

    public ISimulationBuilderContext AddInitializationEvent<U>(Action<U> initialization) where U : ISimulationEvent
        {
            _initializationEventsToGenerate.Add(() =>
            {
                var simulationEvent = serviceProvider.GetRequiredService<U>();
                initialization(simulationEvent);
                return simulationEvent;
            });
            return this;
    }

    public ISimulationBuilderContext AddCrashCondition(Func<ISimulationContext, SimulationCrashResult> crashCondition)
    {
        _crashConditions.Add(crashCondition);
        return this;
    }
}
