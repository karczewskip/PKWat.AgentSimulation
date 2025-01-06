namespace PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Agents; 

using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Time;
using System;

public record PregnancyStatus(double Progress = 0)
{
    public bool InLabour => Progress >= 1;

    public PregnancyStatus UpdateProgress(double addingProgress)
    {
        var newProgress = Progress + addingProgress;

        return new PregnancyStatus(
            Progress: newProgress > 1 ? 1 : newProgress);
    }

    public static PregnancyStatus StartPregnancy()
    {
        return new PregnancyStatus();
    }
}

public record PreyState(MovingDirection MovingDirection, PregnancyStatus Pregnancy)
{
    public static PreyState NewBorn() => new PreyState(MovingDirection.None, PregnancyStatus.StartPregnancy());
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
        return State with
        {
            MovingDirection = newDirection
        };
    }

    internal void ResetAfterLabour()
    {
        SetState(
            State with
            {
                Pregnancy = PregnancyStatus.StartPregnancy()
            });
    }

    internal PregnancyStatus UpdatePregnancy(double pregnancyUpdate)
    {
        var newPregnancy = State.Pregnancy.UpdateProgress(pregnancyUpdate);
        SetState(
            State with
            {
                Pregnancy = newPregnancy
            });

        return newPregnancy;
    }
}
