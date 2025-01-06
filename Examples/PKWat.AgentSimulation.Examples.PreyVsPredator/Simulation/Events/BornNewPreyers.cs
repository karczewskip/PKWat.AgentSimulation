namespace PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Events;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Event;
using PKWat.AgentSimulation.Core.PerformanceInfo;
using PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Agents;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

internal class BornNewPreyers(ISimulationCyclePerformanceInfo simulationCyclePerformanceInfo) : ISimulationEvent<PreyVsPredatorEnvironment>
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
            if(context.SimulationEnvironment.AnotherPrey(prey.Id))
            {
                continue;
            }

            var newPregnancy = prey.UpdatePregnancy(pregnancyUpdate);
            if (newPregnancy.InLabour)
            {
                var newBornPrey = context.AddAgent<Prey>();
                newBornPreyersWithParents.Add((newBornPrey.Id, prey.Id));

                prey.ResetAfterLabour();
            }
        }

        context.SimulationEnvironment.PlaceNewBornPreys(newBornPreyersWithParents);
    }

    public async Task<bool> ShouldBeExecuted(ISimulationContext<PreyVsPredatorEnvironment> context)
    {
        return true;
    }
}
