namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator.Agents;

using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.RandomNumbers;

public class HealthStatus
{
    public double Health { get; private set; }

    public bool Died => Health <= 0;

    private HealthStatus(double health)
    {
        Health = health;
    }

    public void DecreaseHealth(double substractingHealth)
    {
        var newHealth = Health - substractingHealth;

        Health = newHealth < 0 ? 0 : newHealth;
    }

    public void Recover()
    {
        Health = 1;
    }

    public static HealthStatus FullHealth() => new HealthStatus(1);
}

public class Predator(IRandomNumbersGenerator randomNumbersGenerator) :
    SimpleSimulationAgent
{
    private HealthStatus _healthStatus = HealthStatus.FullHealth();

    public bool IsDied => _healthStatus.Died;

    internal void DecreaseHealth(double starvationIncrement)
    {
        _healthStatus.DecreaseHealth(starvationIncrement);
    }

    internal void ResetAfterEaten()
    {
        _healthStatus.Recover();
    }
}
