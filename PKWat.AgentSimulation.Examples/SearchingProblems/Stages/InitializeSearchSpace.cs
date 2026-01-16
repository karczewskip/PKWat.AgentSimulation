namespace PKWat.AgentSimulation.Examples.SearchingProblems.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using System.Threading.Tasks;

public class InitializeSearchSpace : ISimulationStage
{
    private double _width = 100.0;
    private double _height = 100.0;

    public void SetSize(double width, double height)
    {
        _width = width;
        _height = height;
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<SearchingEnvironment>();
        environment.SetSearchSpaceSize(_width, _height);
        
        await Task.CompletedTask;
    }
}
