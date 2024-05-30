namespace PKWat.AgentSimulation.Core;

using Microsoft.Extensions.DependencyInjection;

public interface ISimulationBuilderContext<ENVIRONMENT>
{
    ISimulationBuilderContext<ENVIRONMENT> AddAgent<AGENT, AGENTSTATE>(Func<IRandomNumbersGenerator, AGENTSTATE> generateInitialState, ISensor<ENVIRONMENT>[] sensors) where AGENT : IStateContainingAgent<ENVIRONMENT, AGENTSTATE>;
    ISimulationBuilderContext<ENVIRONMENT> AddAgents<AGENT, AGENTSTATE>(int number, Func<IRandomNumbersGenerator, AGENTSTATE> generateInitialState, ISensor<ENVIRONMENT>[] sensors) where AGENT : IStateContainingAgent<ENVIRONMENT, AGENTSTATE>;
    ISimulationBuilderContext<ENVIRONMENT> AddEnvironmentUpdates(Func<ISimulationContext<ENVIRONMENT>, Task> update);
    ISimulationBuilderContext<ENVIRONMENT> AddCallback(Func<ISimulationContext<ENVIRONMENT>, Task> callback);
    ISimulationBuilderContext<ENVIRONMENT> SetSimulationStep(TimeSpan simulationStep);
    ISimulationBuilderContext<ENVIRONMENT> SetWaitingTimeBetweenSteps(TimeSpan waitingTimeBetweenSteps);
    ISimulationBuilderContext<ENVIRONMENT> SetRandomSeed(int seed);
    ISimulationBuilderContext<ENVIRONMENT> AddEvent<U>() where U : ISimulationEvent<ENVIRONMENT>;

    ISimulation Build();
}

internal class SimulationBuilderContext<T>(T simulationEnvironment, IServiceProvider serviceProvider) : ISimulationBuilderContext<T>
{
    private IRandomNumbersGenerator _randomNumbersGenerator => serviceProvider.GetRequiredService<IRandomNumbersGenerator>();

    private List<AgentWithSensors<T>> _agents = new();
    private List<Func<AgentWithSensors<T>>> _agentsToGenerate = new();
    private List<Func<ISimulationEvent<T>>> _eventsToGenerate = new();
    private List<Func<ISimulationContext<T>, Task>> _environmentUpdates = new();
    private List<Func<ISimulationContext<T>, Task>> _callbacks = new();
    private List<ISimulationEvent<T>> _events = new();
    private TimeSpan _simulationStep = TimeSpan.FromSeconds(1);
    private TimeSpan _waitingTimeBetweenSteps = TimeSpan.Zero;
    private int? _randomSeed;

    public ISimulationBuilderContext<T> AddAgent<U, AGENTSTATE>(Func<IRandomNumbersGenerator, AGENTSTATE> generateInitialState, ISensor<T>[] sensors) where U : IStateContainingAgent<T, AGENTSTATE>
    {
        return AddAgents<U, AGENTSTATE>(1, generateInitialState, sensors);
    }

    public ISimulationBuilderContext<T> AddAgents<U, AGENTSTATE>(int number, Func<IRandomNumbersGenerator, AGENTSTATE> generateInitialState, ISensor<T>[] sensors) where U : IStateContainingAgent<T, AGENTSTATE>
    {
        _agentsToGenerate.AddRange(Enumerable
            .Range(0, number)
            .Select(x => new Func<AgentWithSensors<T>>(
                () =>
                {
                    var newAgent = serviceProvider.GetRequiredService<U>();
                    newAgent.Initialize(generateInitialState(_randomNumbersGenerator));
                    return new AgentWithSensors<T>(newAgent, sensors);
                })));

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
    {
        _eventsToGenerate.Add(() => serviceProvider.GetRequiredService<U>());

        return this;
    }

    public ISimulation Build()
    {
        serviceProvider.GetRequiredService<RandomNumbersGeneratorFactory>().Initialize(_randomSeed);
        _agents.AddRange(_agentsToGenerate.Select(x => x()));
        _events.AddRange(_eventsToGenerate.Select(x => x()));

        return new Simulation<T>(new SimulationContext<T>(serviceProvider, simulationEnvironment ,_agents, _simulationStep, _waitingTimeBetweenSteps), _environmentUpdates, _callbacks, _events);
    }
}
