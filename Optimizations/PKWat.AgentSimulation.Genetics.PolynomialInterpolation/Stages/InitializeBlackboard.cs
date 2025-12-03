using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

namespace PKWat.AgentSimulation.Genetics.PolynomialInterpolation.Stages;

public class InitializeBlackboard : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<CalculationsBlackboard>();
        var from = -5.0;
        var to = 4.0;
        var freq = 1_000;
        environment.SetExpectedValues(
            ExpectedValues.Build(
                Enumerable.Range(0, freq + 1).Select(x => from + (to - from) * x / freq),
                TargetFunction));
    }

    private double TargetFunction(double x)
    {
        switch (x)
        {
            case < -5.0:
                return 0;
            case <= -3.0:
                return (x+5)*(x+3);
            case <= -1.0:
                return x;
            case <= 4.0:
                return (x + 1) * (x - 4) * x;
            default:
                return 0;
        }
    }
}
