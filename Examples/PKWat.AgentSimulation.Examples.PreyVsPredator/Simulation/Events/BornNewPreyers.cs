namespace PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Events;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Event;
using PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Agents;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

internal class BornNewPreyers : ISimulationEvent<PreyVsPredatorEnvironment>
{
    public async Task Execute(ISimulationContext<PreyVsPredatorEnvironment> context)
    {
        var newBornPreyersWithParents = new List<(AgentId NewBorn, AgentId Parent)>();
        var preysInLabor = context.GetAgents<Prey>().Where(x => x.State.Pregnancy.InLabour).ToArray();

        foreach (var prey in preysInLabor)
        {
            var newBornPrey = context.AddAgent<Prey>();
            newBornPreyersWithParents.Add((newBornPrey.Id, prey.Id));

            prey.ResetAfterLabour();
        }

        context.SimulationEnvironment.PlaceNewBornPreys(newBornPreyersWithParents);
    }

    public async Task<bool> ShouldBeExecuted(ISimulationContext<PreyVsPredatorEnvironment> context)
    {
        return true;
    }
}
