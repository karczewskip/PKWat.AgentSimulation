namespace PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.PerformanceInfo;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Agents;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

internal class BornNewPreyers(ISimulationCyclePerformanceInfo simulationCyclePerformanceInfo) : ISimulationStage<PreyVsPredatorEnvironment>
{
    private double pregnancyUpdate = 0.001;

    public void ChangePregnancyUpdate(double newPregnancyUpdate)
    {
        pregnancyUpdate = newPregnancyUpdate;
    }

    public async Task Execute(ISimulationContext<PreyVsPredatorEnvironment> context)
    {
        using var _ = simulationCyclePerformanceInfo.AddStep("Born new preyers");
        var newBornPreyersWithParents = new List<(AgentId NewBorn, AgentId Parent)>();
        var allPreys = context.GetAgents<Prey>().ToArray();

        foreach (var prey in allPreys)
        {
            if (context.SimulationEnvironment.AnotherPrey(prey.Id))
            {
                continue;
            }

            prey.UpdatePregnancy(pregnancyUpdate);
            if (prey.IsInLabour)
            {
                var newBornPrey = context.AddAgent<Prey>();
                newBornPreyersWithParents.Add((newBornPrey.Id, prey.Id));

                prey.ResetAfterLabour();
            }
        }

        context.SimulationEnvironment.PlaceNewBornPreys(newBornPreyersWithParents);
    }
}
