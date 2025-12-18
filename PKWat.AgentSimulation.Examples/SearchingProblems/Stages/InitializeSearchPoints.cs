namespace PKWat.AgentSimulation.Examples.SearchingProblems.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;
using System.Threading.Tasks;

public class InitializeSearchPoints(IRandomNumbersGenerator randomNumbersGenerator) : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<SearchingEnvironment>();
        
        // Create initial random points on the Euclidean surface
        int numberOfInitialPoints = 50;
        
        for (int i = 0; i < numberOfInitialPoints; i++)
        {
            double x = randomNumbersGenerator.NextDouble(0, environment.SearchSpaceWidth);
            double y = randomNumbersGenerator.NextDouble(0, environment.SearchSpaceHeight);
            
            // Example objective function: maximize -(x-50)^2 - (y-50)^2 (peak at center)
            double value = CalculateObjectiveFunction(x, y);
            
            var point = SearchPoint.Create(x, y, value);
            environment.AddPoint(point);
        }

        await Task.CompletedTask;
    }

    private double CalculateObjectiveFunction(double x, double y)
    {
        // Simple quadratic function with maximum at (50, 50)
        // You can customize this function for different optimization landscapes
        double centerX = 50.0;
        double centerY = 50.0;
        return 100.0 - System.Math.Pow(x - centerX, 2) / 25.0 - System.Math.Pow(y - centerY, 2) / 25.0;
    }
}
