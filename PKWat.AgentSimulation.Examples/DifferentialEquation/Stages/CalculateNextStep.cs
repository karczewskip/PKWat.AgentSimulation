namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;

public class CalculateNextStep : ISimulationStage
{
    public Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<DifferentialEquationEnvironment>();
        var agents = context.GetAgents<DESolverAgent>();

        foreach (var agent in agents)
        {
            if (!agent.HasReachedEnd(environment.EndX))
            {
                agent.CalculateNextStep(environment.StepSize, environment.DerivativeFunction);
            }
        }

        return Task.CompletedTask;
    }
}
