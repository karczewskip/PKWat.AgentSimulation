namespace PKWat.AgentSimulation.Core;

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

public interface ISimulationContext<ENVIRONMENT>
{
    ENVIRONMENT SimulationEnvironment { get; }
    TimeSpan SimulationStep { get; }
    TimeSpan SimulationTime { get; }

    IEnumerable<AGENT> GetAgents<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;

    void AddAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;
}

internal class SimulationContext<ENVIRONMENT>: ISimulationContext<ENVIRONMENT>
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
        Agents = agents;
        SimulationStep = simulationStep;
        SimulationTime = TimeSpan.Zero;
        WaitingTimeBetweenSteps = waitingTimeBetweenSteps;
    }

    public ENVIRONMENT SimulationEnvironment { get; }
    public TimeSpan SimulationStep { get; }
    public TimeSpan SimulationTime { get; private set; }

    public List<ISimulationAgent<ENVIRONMENT>> Agents { get; }
    public TimeSpan WaitingTimeBetweenSteps { get; }

    public IEnumerable<T> GetAgents<T>() where T : ISimulationAgent<ENVIRONMENT> 
        => Agents.OfType<T>();

    internal void UpdateSimulationTime()
    {
        SimulationTime += SimulationStep;
    }

    public void AddAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>
    {
        var agent = _serviceProvider.GetRequiredService<AGENT>();
        Agents.Add(agent);
    }
}