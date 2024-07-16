namespace PKWat.AgentSimulation.Core;

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Collections.Generic;

public record SimulationTime(TimeSpan Time, TimeSpan Step)
{
    public SimulationTime AddStep() => this with { Time = Time + Step };
}

public interface ISimulationContext<ENVIRONMENT>
{
    ENVIRONMENT SimulationEnvironment { get; }
    SimulationTime SimulationTime { get; }

    AGENT AddAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;
    IEnumerable<AGENT> GetAgents<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;
    AGENT GetRequiredAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>;
    AGENT GetRequiredAgent<AGENT>(AgentId agentId) where AGENT : ISimulationAgent<ENVIRONMENT>;

    void SendMessage(IAddressedAgentMessage addressedAgentMessage);
    IEnumerable<IAgentMessage> GetMessages(IRecognizableAgent receiver);
}

internal class SimulationContext<ENVIRONMENT> : ISimulationContext<ENVIRONMENT>
{
    private readonly ConcurrentBag<IAddressedAgentMessage> _newMessages = new();
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

        _messagesToDeliver.Clear();
        foreach (var addressedAgentMessage in _newMessages)
        {
            var receiver = addressedAgentMessage.Receiver;
            var message = addressedAgentMessage.Message;

            if (!_messagesToDeliver.ContainsKey(receiver))
            {
                _messagesToDeliver[receiver] = new List<IAgentMessage>();
            }

            _messagesToDeliver[receiver].Add(message);
        }

        _newMessages.Clear();
    }

    public AGENT AddAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>
    {
        var agent = _serviceProvider.GetRequiredService<AGENT>();
        agent.Initialize(this);
        Agents.Add(agent.Id, agent);

        return agent;
    }

    public void SendMessage(IAddressedAgentMessage addressedAgentMessage)
    {
        _newMessages.Add(addressedAgentMessage);
    }

    public IEnumerable<IAgentMessage> GetMessages(IRecognizableAgent receiver)
    {
        if (_messagesToDeliver.TryGetValue(receiver, out var messages))
        {
            return messages;
        }

        return [];
    }
}