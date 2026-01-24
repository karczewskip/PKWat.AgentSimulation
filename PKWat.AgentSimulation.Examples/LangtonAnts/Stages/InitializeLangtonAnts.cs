namespace PKWat.AgentSimulation.Examples.LangtonAnts.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.SimMath.Colors;
using System.Threading.Tasks;

public class InitializeLangtonAnts(ColorsGenerator colorsGenerator, IRandomNumbersGenerator randomNumbersGenerator) : ISimulationStage
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
        var allColors = colorsGenerator.Generate(_pairCount * 2);
        var environment = context.GetSimulationEnvironment<LangtonAntsEnvironment>();
        environment.Initialize(_width, _height, allColors);

        for(int i = 0; i < _pairCount; i++)
        {
            var color1 = allColors[i * 2];
            var color2 = allColors[i * 2 + 1];
            var pair = new AntPair(i, color1, color2);
            var ant = context.AddAgent<Ant>();
            var xA = randomNumbersGenerator.Next(environment.Width);
            var yA = randomNumbersGenerator.Next(environment.Height);
            var directionA = (AntDirection)randomNumbersGenerator.Next(4);
            ant.Initialize(xA, yA, directionA, AntType.A, pair);

            var antB = context.AddAgent<Ant>();
            var xB = randomNumbersGenerator.Next(environment.Width);
            var yB = randomNumbersGenerator.Next(environment.Height);
            var directionB = (AntDirection)randomNumbersGenerator.Next(4);
            antB.Initialize(xB, yB, directionB, AntType.B, pair);
        }

    }
}
