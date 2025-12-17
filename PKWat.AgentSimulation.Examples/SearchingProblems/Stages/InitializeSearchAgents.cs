namespace PKWat.AgentSimulation.Examples.SearchingProblems.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.SearchingProblems.Agents;
using System.Threading.Tasks;

public class InitializeSearchAgents(IRandomNumbersGenerator randomNumbersGenerator) : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<SearchingEnvironment>();
        
        // Create search agents at random starting positions
        int numberOfAgents = 5;
        
        for (int i = 0; i < numberOfAgents; i++)
        {
            var agent = context.AddAgent<SearchAgent>();
            
            double startX = randomNumbersGenerator.NextDouble(0, environment.SearchSpaceWidth);
            double startY = randomNumbersGenerator.NextDouble(0, environment.SearchSpaceHeight);
            double startValue = CalculateObjectiveFunction(startX, startY);
            
            agent.SetPosition(startX, startY, startValue);
            
            var startPoint = SearchPoint.Create(startX, startY, startValue);
            environment.AddPoint(startPoint);
        }

        await Task.CompletedTask;
    }

    private double CalculateObjectiveFunction(double x, double y)
    {
        double centerX = 50.0;
        double centerY = 50.0;
        return 100.0 - Math.Pow(x - centerX, 2) / 25.0 - Math.Pow(y - centerY, 2) / 25.0;
    }
}
