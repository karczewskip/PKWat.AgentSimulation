namespace PKWat.AgentSimulation.Examples.Pendulum.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.Pendulum.Agents;

public class CalculatePendulumStep : ISimulationStage
{
    public Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<PendulumEnvironment>();
        var agents = context.GetAgents<PendulumSolverAgent>();

        foreach (var agent in agents)
        {
            if (!agent.HasReachedEnd(environment.TotalTime))
            {
                agent.CalculateNextStep(environment.TimeStep, environment.Gravity, environment.Length);
            }
        }

        return Task.CompletedTask;
    }
}
