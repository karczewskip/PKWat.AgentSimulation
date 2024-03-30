namespace PKWat.AgentSimulation.Core;

using Microsoft.Extensions.DependencyInjection;

public interface ISimulationBuilderContext<T>
{
    ISimulationBuilderContext<T> AddAgents<U>(int number) where U : ISimulationAgent<T>;
    ISimulationBuilderContext<T> AddEnvironmentUpdates(Func<ISimulationContext<T>, Task> update);
    ISimulationBuilderContext<T> AddCallback(Func<ISimulationContext<T>, Task> callback);
    ISimulationBuilderContext<T> SetSimulationStep(TimeSpan simulationStep);
    ISimulationBuilderContext<T> SetWaitingTimeBetweenSteps(TimeSpan waitingTimeBetweenSteps);
    ISimulationBuilderContext<T> SetRandomSeed(int seed);

    ISimulation Build();
}

internal class SimulationBuilderContext<T> : ISimulationBuilderContext<T>
{
    private readonly T _simulationEnvironment;
    private readonly IServiceProvider _serviceProvider;

    private List<ISimulationAgent<T>> _agents = new();
    private List<Func<ISimulationAgent<T>>> _agentsToGenerate = new();
    private List<Func<ISimulationContext<T>, Task>> _environmentUpdates = new();
    private List<Func<ISimulationContext<T>, Task>> _callbacks = new();
    private TimeSpan _simulationStep = TimeSpan.FromSeconds(1);
    private TimeSpan _waitingTimeBetweenSteps = TimeSpan.Zero;
    private int? _randomSeed;

    public SimulationBuilderContext(T simulationEnvironment, IServiceProvider serviceProvider)
    {
        _simulationEnvironment = simulationEnvironment;
        _serviceProvider = serviceProvider;
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

    public ISimulation Build()
    {
        _serviceProvider.GetRequiredService<RandomNumbersGeneratorFactory>().Initialize(_randomSeed);
        _agents.AddRange(_agentsToGenerate.Select(x => x()));

        return new Simulation<T>(new SimulationContext<T>(_simulationEnvironment ,_agents, _simulationStep, _waitingTimeBetweenSteps), _environmentUpdates, _callbacks);
    }
}
