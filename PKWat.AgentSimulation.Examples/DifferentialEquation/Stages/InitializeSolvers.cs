namespace PKWat.AgentSimulation.Examples.DifferentialEquation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.DifferentialEquation.Agents;

public class InitializeSolvers : ISimulationStage
{
    public Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<DifferentialEquationEnvironment>();

        var analyticalAgent = context.GetAgents<AnalyticalSolverAgent>().First();
        analyticalAgent.Initialize(environment.StartX, environment.InitialY);
        analyticalAgent.SetAnalyticalSolution(environment.AnalyticalSolution);

        var eulerAgent = context.GetAgents<EulerMethodAgent>().First();
        eulerAgent.Initialize(environment.StartX, environment.InitialY);

        var rkAgent = context.GetAgents<RungeKuttaMethodAgent>().First();
        rkAgent.Initialize(environment.StartX, environment.InitialY);

        return Task.CompletedTask;
    }
}
