using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;

namespace PKWat.AgentSimulation.Genetic.SeparateAgents.Logic.Stages;

internal class BuildNewAgents(IRandomNumbersGenerator randomNumbersGenerator) : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        context.RemoveAgents(context.GetAgents<PolynomialCheckAgent>().Select(x => x.Id));

        var minCoefficient = -10;
        var maxCoefficient = 10;
        var numberOfCoefficients = 10;

        foreach (var i in Enumerable.Range(0, 1000))
        {
            var agent = context.AddAgent<PolynomialCheckAgent>();
            agent.SetParameters(PolynomialParameters.BuildFromCoefficients(Enumerable.Range(0, numberOfCoefficients).Select(_ => randomNumbersGenerator.NextDouble(minCoefficient, maxCoefficient)).ToArray()));
        }
    }
}
