namespace PKWat.AgentSimulation.Examples.LangtonAnts.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using System.Threading.Tasks;

public class InitializeLangtonAnts : ISimulationStage
{
    private int _width = 100;
    private int _height = 100;
    private int _pairCount = 3;

    public void UseSize(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void UsePairCount(int pairCount)
    {
        _pairCount = pairCount;
    }

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<LangtonAntsEnvironment>();
        environment.Initialize(_width, _height, _pairCount);
    }
}
