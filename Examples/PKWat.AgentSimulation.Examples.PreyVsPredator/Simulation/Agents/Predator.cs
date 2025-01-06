namespace PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Agents;

using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Time;

public record HealthStatus(double Health = 1, bool Died = false)
{
    public HealthStatus DecreaseHealth(double substractingHealth)
    {
        var newHealth = Health - substractingHealth;

        return new HealthStatus(
            Health: newHealth, 
            Died: newHealth <= 0);
    }

    public HealthStatus Recover()
    {
        return new HealthStatus(Health: 1, Died: false);
    }
}

public record PredatorState(MovingDirection MovingDirection, HealthStatus Health)
{
    public static PredatorState NewBorn() => new PredatorState(MovingDirection.None, new HealthStatus());
}

internal class Predator(IRandomNumbersGenerator randomNumbersGenerator) :
    SimulationAgent<PreyVsPredatorEnvironment, PredatorState>
{
    private readonly MovingDirection[] possibleDirections = [MovingDirection.Up, MovingDirection.Down, MovingDirection.Left, MovingDirection.Right];

    protected override PredatorState GetInitialState(PreyVsPredatorEnvironment environment)
    {
        return PredatorState.NewBorn();
    }

    protected override PredatorState GetNextState(PreyVsPredatorEnvironment environment, IReadOnlySimulationTime simulationTime)
    {
        var newDirection = possibleDirections[randomNumbersGenerator.Next(possibleDirections.Length)];
        var newHealth = State.Health.DecreaseHealth(0.1);
        return State with
        {
            MovingDirection = newDirection,
            Health = newHealth
        };
    }
}
