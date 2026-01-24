using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

namespace PKWat.AgentSimulation.Examples.LangtonAnts.Stages;

public class CalculateAntNewPosition: ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<LangtonAntsEnvironment>();
        var ants = context.GetAgents<Ant>();

        foreach (var ant in ants)
        {
            var (antX, antY) = ant.GetCurrentPosition();
            var currentColor = environment.GetColorAt(antX, antY);
            var (color, newX, newY, newDirection) = ant.CalculateNewPositionAndColor(currentColor);
            var (wrappedX, wrappedY) = environment.WrapCoordinates(newX, newY);
            if (environment.WasFreeInPreviousEpoch(wrappedX, wrappedY))
            {
                ant.SetTargetPosition(wrappedX, wrappedY, newDirection);
                environment.SetColorAt(antX, antY, color);
            }
        }
    }
}
