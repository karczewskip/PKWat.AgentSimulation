namespace PKWat.AgentSimulation.Examples.TspProblems.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.SimMath.Algorithms.TSP;
using System.Threading.Tasks;

public class InitializeTspPoints(IRandomNumbersGenerator randomNumbersGenerator) : ISimulationStage
{
    private int _pointCount = 8;

    public InitializeTspPoints SetPointCount(int count)
    {
        _pointCount = count;
        return this;
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspEnvironment>();

        // Generate random points on the Euclidean surface
        for (int i = 0; i < _pointCount; i++)
        {
            double x = randomNumbersGenerator.NextDouble(10, environment.SearchSpaceWidth - 10);
            double y = randomNumbersGenerator.NextDouble(10, environment.SearchSpaceHeight - 10);
            
            var point = TspPoint.Create(i, x, y);
            environment.AddPoint(point);
        }

        // Build distance matrix for all agents to use
        environment.BuildDistanceMatrix();

        await Task.CompletedTask;
    }
}
