namespace PKWat.AgentSimulation.Core;

using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Core.Time;
using System.Collections.Generic;

public interface ISimulationContext<ENVIRONMENT> : ISimulationTimeProvider where ENVIRONMENT : ISimulationEnvironment
{
    ENVIRONMENT SimulationEnvironment { get; }

    AGENT AddAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;
    IEnumerable<AGENT> GetAgents<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;
    AGENT GetRequiredAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;
    AGENT GetRequiredAgent<AGENT>(AgentId agentId) where AGENT : ISimulationAgent<ENVIRONMENT>;
}

internal class SimulationContext<ENVIRONMENT> : ISimulationContext<ENVIRONMENT> where ENVIRONMENT : ISimulationEnvironment
{
    private readonly IServiceProvider _serviceProvider;
    private SimulationTime _simulationTime;

    public SimulationContext(
        IServiceProvider serviceProvider,
        ENVIRONMENT simulationEnvironment,
        ISimulationAgent<ENVIRONMENT>[] agents,
        TimeSpan simulationStep,
        TimeSpan waitingTimeBetweenSteps)
    {
        _serviceProvider = serviceProvider;
        _simulationTime = new SimulationTime(TimeSpan.Zero, simulationStep);

        SimulationEnvironment = simulationEnvironment;
        Agents = agents.ToDictionary(x => x.Id);
        WaitingTimeBetweenSteps = waitingTimeBetweenSteps;
    }

    public ENVIRONMENT SimulationEnvironment { get; }
    public IReadOnlySimulationTime SimulationTime => _simulationTime;

    public Dictionary<AgentId, ISimulationAgent<ENVIRONMENT>> Agents { get; }
    public TimeSpan WaitingTimeBetweenSteps { get; }

    public IEnumerable<T> GetAgents<T>() where T : ISimulationAgent<ENVIRONMENT> 
        => Agents.Values.OfType<T>();

    public AGENT GetRequiredAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>
        => Agents.Values.OfType<AGENT>().Single();

    public AGENT GetRequiredAgent<AGENT>(AgentId agentId) where AGENT : ISimulationAgent<ENVIRONMENT>
        => (AGENT)Agents[agentId];

    internal void Update()
    {
        _simulationTime = _simulationTime.AddStep();
    }

    public AGENT AddAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>
    {
        var agent = _serviceProvider.GetRequiredService<AGENT>();
        agent.Initialize(SimulationEnvironment);
        Agents.Add(agent.Id, agent);

        return agent;
    }
}