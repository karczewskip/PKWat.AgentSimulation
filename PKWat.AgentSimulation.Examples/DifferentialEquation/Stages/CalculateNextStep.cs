namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;

public class CalculateNextStep : ISimulationStage
{
    public Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<DifferentialEquationEnvironment>();
        
        var deSolverAgents = context.GetAgents<DESolverAgent>();
        foreach (var agent in deSolverAgents)
        {
            if (!agent.HasReachedEnd(environment.EndX))
            {
                agent.CalculateNextStep(environment.StepSize);
            }
        }

        var analyticalAgents = context.GetAgents<AnalyticalSolverAgent>();
        foreach (var agent in analyticalAgents)
        {
            if (!agent.HasReachedEnd(environment.EndX))
            {
                agent.CalculateNextStep(environment.StepSize);
            }
        }

        return Task.CompletedTask;
    }
}
