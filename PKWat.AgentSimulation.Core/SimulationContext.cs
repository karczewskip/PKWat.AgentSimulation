namespace PKWat.AgentSimulation.Core;

using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Crash;
using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.Core.PerformanceInfo;
using PKWat.AgentSimulation.Core.Time;
using System.Collections.Generic;

public interface ISimulationContext : ISimulationTimeProvider
{
    ENVIRONMENT GetSimulationEnvironment<ENVIRONMENT>() where ENVIRONMENT : ISimulationEnvironment;
    AGENT AddAgent<AGENT>() where AGENT : ISimulationAgent;
    void RemoveAgent(AgentId agentId);
    IEnumerable<AGENT> GetAgents<AGENT>() where AGENT : ISimulationAgent;
    AGENT GetRequiredAgent<AGENT>() where AGENT : ISimulationAgent;
    AGENT GetRequiredAgent<AGENT>(AgentId agentId) where AGENT : ISimulationAgent;
}

internal class SimulationContext : ISimulationContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _simulationStep;

    private RunningSimulationState _runningState = RunningSimulationState.CreateNotRunningState();
    private SimulationTime _simulationTime;
    private SimulationPerformanceInfo _performanceInfo;

    public SimulationContext(
        IServiceProvider serviceProvider,
        ISimulationEnvironment simulationEnvironment,
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

    public ISimulationEnvironment SimulationEnvironment { get; }
    public IReadOnlySimulationTime Time => _simulationTime;
    public ISimulationCyclePerformanceInfo PerformanceInfo => _performanceInfo;

    public bool IsRunning => _runningState.IsRunning;
    public CancellationToken CancellationToken => _runningState.CancellationToken;
    public SimulationCrashResult CrashResult => _runningState.CrashResult;

    internal void StartSimulation()
    {
        _runningState = RunningSimulationState.CreateRunningState();
    }

    public Dictionary<AgentId, ISimulationAgent> Agents { get; }
    public TimeSpan WaitingTimeBetweenSteps { get; }

    public ENVIRONMENT GetSimulationEnvironment<ENVIRONMENT>() where ENVIRONMENT : ISimulationEnvironment
        => (ENVIRONMENT)SimulationEnvironment;

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
        _simulationTime = SimulationTime.CreateZero();
    }

    internal void StartNewCycle()
    {
        _simulationTime = _simulationTime.AddStep(_simulationStep);
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

    internal async Task OnCycleFinishAsync()
    {
        _performanceInfo.NotifySubscribers();

        await Task.Delay(WaitingTimeBetweenSteps);
    }

    internal void Crash(SimulationCrashResult crashResult)
    {
        _runningState.Crash(crashResult);
    }

    internal void StopSimulation()
    {
        _runningState.Stop();
    }
}