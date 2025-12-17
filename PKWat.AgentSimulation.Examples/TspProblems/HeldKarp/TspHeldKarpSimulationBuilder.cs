namespace PKWat.AgentSimulation.Examples.TspProblems.HeldKarp;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Core.Crash;
using PKWat.AgentSimulation.Examples.TspProblems.HeldKarp.Stages;
using PKWat.AgentSimulation.Examples.TspProblems.Stages;

public class TspHeldKarpSimulationBuilder
{
    private readonly ISimulationBuilder _simulationBuilder;

    public TspHeldKarpSimulationBuilder(ISimulationBuilder simulationBuilder)
    {
        _simulationBuilder = simulationBuilder;
    }

    public ISimulation Build(int pointCount = 8, long maxIterations = 50000)
    {
        var simulation = _simulationBuilder
            .CreateNewSimulation<TspEnvironment>()
            .AddInitializationStage<InitializeTspSpace>(s => s.SetSize(100.0, 100.0))
            .AddInitializationStage<InitializeTspPoints>(s => s.SetPointCount(pointCount))
            .AddInitializationStage<InitializeHeldKarpAgent>()
            .AddStage<ProcessNextHeldKarpState>()
            .AddCrashCondition(c => 
            {
                var agent = c.GetAgents<Agents.HeldKarpAgent>().FirstOrDefault();
                
                if (agent?.IsComplete == true)
                {
                    return SimulationCrashResult.Crash($"Held-Karp Complete! Best distance: {agent.BestSolution?.TotalDistance:F2}");
                }
                
                if (c.Time.StepNo >= maxIterations)
                {
                    return SimulationCrashResult.Crash($"Max iterations reached. Best distance: {agent?.BestSolution?.TotalDistance:F2}");
                }
                
                return SimulationCrashResult.NoCrash;
            })
            .SetRandomSeed(42)
            .Build();

        return simulation;
    }
}
