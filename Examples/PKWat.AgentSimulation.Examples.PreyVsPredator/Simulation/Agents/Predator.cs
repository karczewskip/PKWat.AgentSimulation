namespace PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Agents;

using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Time;
using System;

public record HealthStatus(double Health = 1)
{
    public bool Died => Health <= 0;

    public HealthStatus DecreaseHealth(double substractingHealth)
    {
        var newHealth = Health - substractingHealth;

        return new HealthStatus(
            Health: newHealth);
    }

    public HealthStatus Recover()
    {
        return new HealthStatus(Health: 1);
    }
}

public record PredatorState(MovingDirection MovingDirection, HealthStatus Health)
{
    public static PredatorState NewBorn() => new PredatorState(MovingDirection.None, new HealthStatus());
}

internal class Predator(IRandomNumbersGenerator randomNumbersGenerator) :
    SimulationAgent<PreyVsPredatorEnvironment, PredatorState>
{
    protected override PredatorState GetInitialState(PreyVsPredatorEnvironment environment)
    {
        return PredatorState.NewBorn();
    }

    protected override PredatorState GetNextState(PreyVsPredatorEnvironment environment, IReadOnlySimulationTime simulationTime)
    {
        return State;
    }

    internal HealthStatus DecreaseHealth(double starvationIncrement)
    {
        var newHealth = State.Health.DecreaseHealth(starvationIncrement);
        SetState(
            State with
            {
                Health = newHealth
            });

        return newHealth;
    }

    internal void ResetAfterEaten()
    {
        SetState(
            State with
            {
                Health = State.Health.Recover()
            });
    }
}
