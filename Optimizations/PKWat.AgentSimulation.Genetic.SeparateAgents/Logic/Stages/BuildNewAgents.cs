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

        GenerateNewGenerationUsingNumberOfParents(context, 20, 2, numberOfAgentsToGenerate, numberOfCoefficients);
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

    private void GenerateNewGenerationUsingNumberOfParents(ISimulationContext context, int bestPopulationSize, int numberOfParents, int numberOfAgentsToGenerate, int numberOfCoefficients)
    {
        var bestParameters = GetBestParamters(context, bestPopulationSize);
        context.RemoveAgents(context.GetAgents<PolynomialCheckAgent>().Select(x => x.Id));
        var mutationRate = 0.1;
        var mutationAmount = 1.0;
        for (int i = 0; i < numberOfAgentsToGenerate - bestPopulationSize; i++)
        {
            var allParentParameters = new PolynomialParameters[numberOfParents];
            for (int p = 0; p < numberOfParents; p++)
            {
                allParentParameters[p] = bestParameters[randomNumbersGenerator.Next(bestParameters.Length)];
            }

            var newCoefficients = new double[numberOfCoefficients];
            for (int j = 0; j < numberOfCoefficients; j++)
            {
                var parentIndex = randomNumbersGenerator.Next(numberOfParents);
                var coefficient = allParentParameters[parentIndex].Coefficients[j];
                if (randomNumbersGenerator.NextDouble(0, 1) < mutationRate)
                {
                    coefficient += randomNumbersGenerator.NextDouble(-mutationAmount, mutationAmount);
                }
                newCoefficients[j] = coefficient;
            }

            var agent = context.AddAgent<PolynomialCheckAgent>();
            agent.SetParameters(PolynomialParameters.BuildFromCoefficients(newCoefficients));
        }

        for (int i = 0; i < bestPopulationSize; i++)
        {
            var agent = context.AddAgent<PolynomialCheckAgent>();
            agent.SetParameters(bestParameters[i]);
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
