namespace PKWat.AgentSimulation.Examples.Pendulum.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.Pendulum.Agents;

public class InitializePendulumSolvers : ISimulationStage
{
    public Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<PendulumEnvironment>();
        var agents = context.GetAgents<PendulumSolverAgent>();

        foreach (var agent in agents)
        {
            agent.Initialize(environment.InitialTheta, environment.InitialOmega);
        }

        return Task.CompletedTask;
    }
}
