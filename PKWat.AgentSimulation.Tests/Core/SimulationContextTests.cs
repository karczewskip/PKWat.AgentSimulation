namespace PKWat.AgentSimulation.Tests.Core;

using Microsoft.Extensions.DependencyInjection;
using Moq;
using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Environment;
using System;

[TestFixture]
public class SimulationContextTests : IDisposable
{
    private IServiceProvider _serviceProviderMock;
    private ISimulationEnvironment _simulationEnvironmentMock;
    private ISimulationAgent _simulationAgentMock;
    private TimeSpan _simulationStep;
    private TimeSpan _waitingTimeBetweenSteps;

    [SetUp]
    public void SetUp()
    {
        var simulationAgentMock = new Mock<ISimulationAgent>();
        simulationAgentMock.SetupGet(a => a.Id).Returns(AgentId.GenerateNew());
        _simulationAgentMock = simulationAgentMock.Object;

        _simulationEnvironmentMock = new Mock<ISimulationEnvironment>().Object;
        _simulationStep = TimeSpan.FromSeconds(1);
        _waitingTimeBetweenSteps = TimeSpan.FromMilliseconds(100);

        var services = new ServiceCollection();
        services.AddSingleton(_simulationAgentMock);
        _serviceProviderMock = services.BuildServiceProvider();
    }

    [TearDown]
    public void TearDown()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (_serviceProviderMock is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    [Test]
    public void AddAgent_ShouldAddAgentToDictionary()
    {
        // Arrange
        var context = new SimulationContext(
            _serviceProviderMock,
            _simulationEnvironmentMock,
            [],
            _simulationStep,
            _waitingTimeBetweenSteps);

        // Act
        var agent = context.AddAgent<ISimulationAgent>();

        // Assert
        Assert.That(context.Agents.Keys.Contains(agent.Id));
    }

    [Test]
    public void RemoveAgent_ShouldRemoveAgentFromDictionary()
    {
        // Arrange
        var agents = new ISimulationAgent[] { _simulationAgentMock };
        var context = new SimulationContext(
            _serviceProviderMock,
            _simulationEnvironmentMock,
            agents,
            _simulationStep,
            _waitingTimeBetweenSteps);

        // Act
        context.RemoveAgent(_simulationAgentMock.Id);

        // Assert
        Assert.That(context.Agents.ContainsKey(_simulationAgentMock.Id) == false);
    }

    [Test]
    public void GetRequiredAgent_ShouldReturnAgent()
    {
        // Arrange
        var agents = new ISimulationAgent[] { _simulationAgentMock };
        var context = new SimulationContext(
            _serviceProviderMock,
            _simulationEnvironmentMock,
            agents,
            _simulationStep,
            _waitingTimeBetweenSteps);

        // Act
        var agent = context.GetRequiredAgent<ISimulationAgent>();

        // Assert
        Assert.That(_simulationAgentMock == agent);
    }

    [Test]
    public void GetRequiredAgentById_ShouldReturnAgent()
    {
        // Arrange
        var agents = new ISimulationAgent[] { _simulationAgentMock };
        var context = new SimulationContext(
            _serviceProviderMock,
            _simulationEnvironmentMock,
            agents,
            _simulationStep,
            _waitingTimeBetweenSteps);

        // Act
        var agent = context.GetRequiredAgent<ISimulationAgent>(_simulationAgentMock.Id);

        // Assert
        Assert.That(_simulationAgentMock == agent);
    }

    [Test]
    public void GetSimulationEnvironment_ShouldReturnEnvironment()
    {
        // Arrange
        var agents = new ISimulationAgent[] { _simulationAgentMock };
        var context = new SimulationContext(
            _serviceProviderMock,
            _simulationEnvironmentMock,
            agents,
            _simulationStep,
            _waitingTimeBetweenSteps);

        // Act
        var environment = context.GetSimulationEnvironment<ISimulationEnvironment>();

        // Assert
        Assert.That(_simulationEnvironmentMock == environment);
    }
}
