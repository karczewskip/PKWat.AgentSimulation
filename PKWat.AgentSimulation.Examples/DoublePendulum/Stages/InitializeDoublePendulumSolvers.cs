namespace PKWat.AgentSimulation.Examples.DoublePendulum.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.DoublePendulum.Agents;

public class InitializeDoublePendulumSolvers : ISimulationStage
{
    public Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<DoublePendulumEnvironment>();
        var agents = context.GetAgents<DoublePendulumSolverAgent>();

        foreach (var agent in agents)
        {
            agent.Initialize(
                environment.InitialTheta1, 
                environment.InitialOmega1, 
                environment.InitialTheta2, 
                environment.InitialOmega2,
                environment.Length1,
                environment.Length2);
        }

        return Task.CompletedTask;
    }
}
