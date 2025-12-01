using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;

namespace PKWat.AgentSimulation.Genetic.SeparateAgents.Logic.Stages;

internal class BuildNewAgents(IRandomNumbersGenerator randomNumbersGenerator) : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var numberOfAgentsToGenerate = 1000;
        var numberOfCoefficients = 7;

        if (context.Time.StepNo == 1)
        {
            GenerateFirstGeneration(context, numberOfAgentsToGenerate, numberOfCoefficients);
            return;
        }

        GenerateNewGeneration(context, numberOfAgentsToGenerate, numberOfCoefficients);
    }

    private void GenerateFirstGeneration(ISimulationContext context, int numberOfAgentsToGenerate, int numberOfCoefficients)
    {
        var minCoefficient = -10.0;
        var maxCoefficient = 10.0;

        for (int i = 0; i < numberOfAgentsToGenerate; i++)
        {
            var agent = context.AddAgent<PolynomialCheckAgent>();
            agent.SetParameters(PolynomialParameters.BuildFromCoefficients(Enumerable.Range(0, numberOfCoefficients).Select(_ => randomNumbersGenerator.NextDouble(minCoefficient, maxCoefficient)).ToArray()));
        }
    }

    private void GenerateNewGeneration(ISimulationContext context, int numberOfAgentsToGenerate, int numberOfCoefficients)
    {
        var bestParameters = GetBestParamters(context, 20);
        context.RemoveAgents(context.GetAgents<PolynomialCheckAgent>().Select(x => x.Id));
        var mutationRate = 0.4;
        var mutationAmount = 1.0;
        for (int i = 0; i < numberOfAgentsToGenerate; i++)
        {
            var parentParameters = bestParameters[randomNumbersGenerator.Next(bestParameters.Length)];
            var newCoefficients = new double[numberOfCoefficients];
            for (int j = 0; j < numberOfCoefficients; j++)
            {
                var coefficient = parentParameters.Coefficients[j];
                if (randomNumbersGenerator.NextDouble(0, 1) < mutationRate)
                {
                    coefficient += randomNumbersGenerator.NextDouble(-mutationAmount, mutationAmount);
                }
                newCoefficients[j] = coefficient;
            }
            var agent = context.AddAgent<PolynomialCheckAgent>();
            agent.SetParameters(PolynomialParameters.BuildFromCoefficients(newCoefficients));
        }
    }

    private PolynomialParameters[] GetBestParamters(ISimulationContext context, int count)
    {
        var blckboard = context.GetSimulationEnvironment<CalculationsBlackboard>();
        return blckboard.AgentErrors
            .OrderBy(x => x.Value.AbsoluteError)
            .Take(count)
            .Select(x => context.GetRequiredAgent<PolynomialCheckAgent>(x.Key).Parameters!)
            .ToArray();
    }
}
