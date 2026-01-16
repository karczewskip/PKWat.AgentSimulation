namespace PKWat.AgentSimulation.Examples.TspProblems.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using System.Threading.Tasks;

public class InitializeTspSpace : ISimulationStage
{
    private double _width = 100.0;
    private double _height = 100.0;

    public InitializeTspSpace SetSize(double width, double height)
    {
        _width = width;
        _height = height;
        return this;
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspEnvironment>();
        environment.SetSearchSpaceSize(_width, _height);
        await Task.CompletedTask;
    }
}
