namespace PKWat.AgentSimulation.Examples.SearchingProblems.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.SearchingProblems.Agents;
using System.Threading.Tasks;

public class ExploreNextPoint(IRandomNumbersGenerator randomNumbersGenerator) : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<SearchingEnvironment>();
        var agents = context.GetAgents<SearchAgent>();

        foreach (var agent in agents)
        {
            // Generate a neighbor point to explore
            double stepSize = 5.0; // Size of exploration step
            double newX = agent.CurrentX + randomNumbersGenerator.NextDouble(-stepSize, stepSize);
            double newY = agent.CurrentY + randomNumbersGenerator.NextDouble(-stepSize, stepSize);

            // Keep within bounds
            newX = System.Math.Clamp(newX, 0, environment.SearchSpaceWidth);
            newY = System.Math.Clamp(newY, 0, environment.SearchSpaceHeight);

            // Evaluate the new point
            double newValue = CalculateObjectiveFunction(newX, newY);
            var newPoint = SearchPoint.Create(newX, newY, newValue);

            // Accept if better (greedy hill climbing) or with probability (simulated annealing)
            if (newValue > agent.CurrentValue || AcceptWorseWithProbability(agent.CurrentValue, newValue, context.Time.StepNo))
            {
                agent.SetPosition(newX, newY, newValue);
                environment.AddPoint(newPoint);
                environment.UpdateBestPoint(newPoint);
            }
        }

        await Task.CompletedTask;
    }

    private double CalculateObjectiveFunction(double x, double y)
    {
        // Same objective function as initialization
        double centerX = 50.0;
        double centerY = 50.0;
        return 100.0 - System.Math.Pow(x - centerX, 2) / 25.0 - System.Math.Pow(y - centerY, 2) / 25.0;
    }

    private bool AcceptWorseWithProbability(double currentValue, double newValue, long iteration)
    {
        // Simulated annealing: accept worse solutions with decreasing probability
        double temperature = System.Math.Max(1.0, 100.0 - iteration);
        double acceptanceProbability = System.Math.Exp((newValue - currentValue) / temperature);
        return randomNumbersGenerator.NextDouble() < acceptanceProbability;
    }
}
