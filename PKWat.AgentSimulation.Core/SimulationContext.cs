namespace PKWat.AgentSimulation.Core;

using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.Core.PerformanceInfo;
using PKWat.AgentSimulation.Core.Time;
using System.Collections.Generic;

public interface ISimulationContext<ENVIRONMENT> : ISimulationTimeProvider, ISimulationPerformanceInfoProvider where ENVIRONMENT : ISimulationEnvironment
{
    ENVIRONMENT SimulationEnvironment { get; }

    AGENT AddAgent<AGENT>() where AGENT : ISimulationAgent;
    void RemoveAgent(AgentId agentId);
    IEnumerable<AGENT> GetAgents<AGENT>() where AGENT : ISimulationAgent;
    AGENT GetRequiredAgent<AGENT>() where AGENT : ISimulationAgent;
    AGENT GetRequiredAgent<AGENT>(AgentId agentId) where AGENT : ISimulationAgent;
}

internal class SimulationContext<ENVIRONMENT> : ISimulationContext<ENVIRONMENT> where ENVIRONMENT : ISimulationEnvironment
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _simulationStep;

    private SimulationTime _simulationTime;
    private SimulationPerformanceInfo _performanceInfo;

    public SimulationContext(
        IServiceProvider serviceProvider,
        ENVIRONMENT simulationEnvironment,
        ISimulationAgent[] agents,
        TimeSpan simulationStep,
        TimeSpan waitingTimeBetweenSteps)
    {
        _serviceProvider = serviceProvider;
        _simulationStep = simulationStep;

        SimulationEnvironment = simulationEnvironment;
        Agents = agents.ToDictionary(x => x.Id);
        WaitingTimeBetweenSteps = waitingTimeBetweenSteps;
    }

    public ENVIRONMENT SimulationEnvironment { get; }
    public IReadOnlySimulationTime SimulationTime => _simulationTime;
    public ISimulationCyclePerformanceInfo PerformanceInfo => _performanceInfo;

    public Dictionary<AgentId, ISimulationAgent> Agents { get; }
    public TimeSpan WaitingTimeBetweenSteps { get; }

    public IEnumerable<T> GetAgents<T>() where T : ISimulationAgent 
        => Agents.Values.OfType<T>();

    public AGENT GetRequiredAgent<AGENT>() where AGENT : ISimulationAgent
        => Agents.Values.OfType<AGENT>().Single();

    public AGENT GetRequiredAgent<AGENT>(AgentId agentId) where AGENT : ISimulationAgent
        => (AGENT)Agents[agentId];

    internal void StartCycleZero()
    {
        _performanceInfo = _serviceProvider.GetRequiredService<SimulationPerformanceInfo>();
        _performanceInfo.Clear();
        _simulationTime = new SimulationTime(TimeSpan.Zero, _simulationStep);
    }

    internal void StartNewCycle()
    {
        _simulationTime = _simulationTime.AddStep();
        _performanceInfo.StartNewCycle();
    }

    public AGENT AddAgent<AGENT>() where AGENT : ISimulationAgent
    {
        var agent = _serviceProvider.GetRequiredService<AGENT>();
        Agents.Add(agent.Id, agent);

        return agent;
    }

    public void RemoveAgent(AgentId agentId)
    {
        Agents.Remove(agentId);
    }
}