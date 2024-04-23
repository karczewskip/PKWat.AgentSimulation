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

    public void AddAgent<AGENT>() where AGENT : ISimulationAgent<ENVIRONMENT>
    {
        var agent = _serviceProvider.GetRequiredService<AGENT>();
        agent.Initialize(this);
        Agents.Add(agent);
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