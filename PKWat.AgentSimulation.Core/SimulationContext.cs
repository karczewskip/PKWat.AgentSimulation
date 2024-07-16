namespace PKWat.AgentSimulation.Core;

using Microsoft.Extensions.DependencyInjection;
using PKWat.AgentSimulation.Core.Snapshots;
using System.Collections.Generic;

public record SimulationTime(TimeSpan Time, TimeSpan Step, long StepNo = 0)
{
    public SimulationTime AddStep() => this with { Time = Time + Step, StepNo = StepNo + 1 };
}

public interface ISimulationContext<ENVIRONMENT> where ENVIRONMENT : ISnapshotCreator
{
    ENVIRONMENT SimulationEnvironment { get; }
    SimulationTime SimulationTime { get; }

    AGENT AddAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;
    IEnumerable<AGENT> GetAgents<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;
    AGENT GetRequiredAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;
    AGENT GetRequiredAgent<AGENT>(AgentId agentId) where AGENT : ISimulationAgent<ENVIRONMENT>;
}

internal class SimulationContext<ENVIRONMENT> : ISimulationContext<ENVIRONMENT> where ENVIRONMENT : ISnapshotCreator
{
    private readonly IServiceProvider _serviceProvider;

    public SimulationContext(
        IServiceProvider serviceProvider,
        ENVIRONMENT simulationEnvironment,
        List<ISimulationAgent<ENVIRONMENT>> agents,
        TimeSpan simulationStep,
        TimeSpan waitingTimeBetweenSteps)
    {
        _serviceProvider = serviceProvider;

        SimulationEnvironment = simulationEnvironment;
        Agents = agents.ToDictionary(x => x.Id);
        SimulationTime = new SimulationTime(TimeSpan.Zero, simulationStep);
        WaitingTimeBetweenSteps = waitingTimeBetweenSteps;
    }

    public ENVIRONMENT SimulationEnvironment { get; }
    public SimulationTime SimulationTime { get; private set; }

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
        SimulationTime = SimulationTime.AddStep();
    }

    public AGENT AddAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>
    {
        var agent = _serviceProvider.GetRequiredService<AGENT>();
        agent.Initialize(this);
        Agents.Add(agent.Id, agent);

        return agent;
    }
}