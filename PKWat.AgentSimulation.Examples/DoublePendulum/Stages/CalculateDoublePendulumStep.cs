namespace PKWat.AgentSimulation.Examples.DoublePendulum.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.DoublePendulum.Agents;

public class CalculateDoublePendulumStep : ISimulationStage
{
    public Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<DoublePendulumEnvironment>();
        var agents = context.GetAgents<DoublePendulumSolverAgent>();

        foreach (var agent in agents)
        {
            if (!agent.HasReachedEnd(environment.TotalTime))
            {
                agent.CalculateNextStep(
                    environment.TimeStep, 
                    environment.Gravity, 
                    environment.Length1, 
                    environment.Length2, 
                    environment.Mass1, 
                    environment.Mass2);
            }
        }

        return Task.CompletedTask;
    }
}
