namespace PKWat.AgentSimulation.Core;

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Collections.Generic;

public interface ISimulationContext<ENVIRONMENT>
{
    ENVIRONMENT SimulationEnvironment { get; }
    TimeSpan SimulationStep { get; }
    TimeSpan SimulationTime { get; }

    void AddAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;
    IEnumerable<AGENT> GetAgents<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;
    AGENT GetRequiredAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;

    void SendMessage<RECEIVER>(RECEIVER receiver, IAgentMessage message) where RECEIVER : IRecognizableAgent;
    IEnumerable<IAgentMessage> GetMessages<RECEIVER>(RECEIVER receiver) where RECEIVER : IRecognizableAgent;

}

internal class SimulationContext<ENVIRONMENT> : ISimulationContext<ENVIRONMENT>
{
    private readonly ConcurrentBag<(IRecognizableAgent, IAgentMessage)> _newMessages = new();
    private readonly Dictionary<IRecognizableAgent, List<IAgentMessage>> _messagesToDeliver = new();

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

    public AGENT GetRequiredAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>
        => Agents.OfType<AGENT>().Single();

    internal void Update()
    {
        SimulationTime += SimulationStep;

        _messagesToDeliver.Clear();
        foreach (var (receiver, message) in _newMessages)
        {
            if (!_messagesToDeliver.ContainsKey(receiver))
            {
                _messagesToDeliver[receiver] = new List<IAgentMessage>();
            }

            _messagesToDeliver[receiver].Add(message);
        }

        _newMessages.Clear();
    }

    public void AddAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>
    {
        var agent = _serviceProvider.GetRequiredService<AGENT>();
        Agents.Add(agent);
    }

    public void SendMessage<RECEIVER>(RECEIVER receiver, IAgentMessage message)
        where RECEIVER : IRecognizableAgent
    {
        _newMessages.Add((receiver, message));
    }

    public IEnumerable<IAgentMessage> GetMessages<RECEIVER>(RECEIVER receiver) where RECEIVER : IRecognizableAgent
    {
        if (_messagesToDeliver.TryGetValue(receiver, out var messages))
        {
            return messages;
        }

        return [];
    }
}