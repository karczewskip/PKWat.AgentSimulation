namespace PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Agents; 

using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Time;

public record PregnancyStatus(double Progress = 0, bool InLabour = false)
{
    public PregnancyStatus UpdateProgress(double addingProgress)
    {
        var newProgress = Progress + addingProgress;
        var InLabour = newProgress >= 1;

        return new PregnancyStatus(
            Progress: newProgress > 1 ? 1 : newProgress, 
            InLabour: InLabour);
    }
}

public record PreyState(MovingDirection MovingDirection, PregnancyStatus Pregnancy)
{
    public static PreyState NewBorn() => new PreyState(MovingDirection.None, new PregnancyStatus());
}

internal class Prey(IRandomNumbersGenerator randomNumbersGenerator) :
    SimulationAgent<PreyVsPredatorEnvironment, PreyState>
{
    private readonly MovingDirection[] possibleDirections = [MovingDirection.Up, MovingDirection.Down, MovingDirection.Left, MovingDirection.Right];

    protected override PreyState GetInitialState(PreyVsPredatorEnvironment environment)
    {
        return PreyState.NewBorn();
    }

    protected override PreyState GetNextState(PreyVsPredatorEnvironment environment, IReadOnlySimulationTime simulationTime)
    {
        var newDirection = possibleDirections[randomNumbersGenerator.Next(possibleDirections.Length)];
        var newPregnancy = State.Pregnancy.UpdateProgress(0.1);
        return State with
        {
            MovingDirection = newDirection,
            Pregnancy = newPregnancy
        };
    }
}
