using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;

namespace PKWat.AgentSimulation.Genetic.SeparateAgents.Logic.Stages;

internal class BuildNewAgents(IRandomNumbersGenerator randomNumbersGenerator) : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var numberOfAgentsToGenerate = 1000;
        var numberOfCoefficients = 10;

        if (context.Time.StepNo == 1)
        {
            GenerateFirstGeneration(context, numberOfAgentsToGenerate, numberOfCoefficients);
            return;
        }

        var numberOfChecksWithoutImprovement = context.GetSimulationEnvironment<CalculationsBlackboard>().NumberOfChecksWithoutImprovement;
        if(numberOfChecksWithoutImprovement % 3 == 0)
        {
            GenerateNewWithElite(context, 1, numberOfAgentsToGenerate, numberOfCoefficients);
            return;
        }

        Func<PolynomialParameters[], PolynomialParameters> calculatingFunc = numberOfChecksWithoutImprovement % 2 == 0
            ? GausianFromParents
            : RandomFromParents;

        var numberOfParents = ((numberOfChecksWithoutImprovement / 10) % 2) + 2;
        var bestPopulationSize = Math.Min(20 + numberOfChecksWithoutImprovement, numberOfAgentsToGenerate / 20);

        GenerateNewGenerationUsingNumberOfParents(context, bestPopulationSize, numberOfParents, numberOfAgentsToGenerate, numberOfCoefficients, parents => MutateParameters(calculatingFunc(parents), numberOfChecksWithoutImprovement+1));
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

    private void GenerateNewWithElite(ISimulationContext context, int bestPopulationSize, int numberOfAgentsToGenerate, int numberOfCoefficients)
    {
        var minCoefficient = -10.0;
        var maxCoefficient = 10.0;

        var bestParameters = GetBestParamters(context, bestPopulationSize);
        context.RemoveAgents(context.GetAgents<PolynomialCheckAgent>().Select(x => x.Id));
        for (int i = 0; i < numberOfAgentsToGenerate - bestPopulationSize; i++)
        {
            var agent = context.AddAgent<PolynomialCheckAgent>();
            agent.SetParameters(PolynomialParameters.BuildFromCoefficients(Enumerable.Range(0, numberOfCoefficients).Select(_ => randomNumbersGenerator.NextDouble(minCoefficient, maxCoefficient)).ToArray()));
        }
        for (int i = 0; i < bestPopulationSize; i++)
        {
            var agent = context.AddAgent<PolynomialCheckAgent>();
            agent.SetParameters(bestParameters[i]);
        }
    }

    private void GenerateNewGenerationUsingNumberOfParents(ISimulationContext context, int bestPopulationSize, int numberOfParents, int numberOfAgentsToGenerate, int numberOfCoefficients, Func<PolynomialParameters[], PolynomialParameters> calculateFromParents)
    {
        var bestParameters = GetBestParamters(context, bestPopulationSize);
        context.RemoveAgents(context.GetAgents<PolynomialCheckAgent>().Select(x => x.Id));
        for (int i = 0; i < numberOfAgentsToGenerate - bestPopulationSize; i++)
        {
            var allParentParameters = new PolynomialParameters[numberOfParents];
            for (int p = 0; p < numberOfParents; p++)
            {
                allParentParameters[p] = bestParameters[randomNumbersGenerator.Next(bestParameters.Length)];
            }
            
            var newParameters = calculateFromParents(allParentParameters);

            var agent = context.AddAgent<PolynomialCheckAgent>();
            agent.SetParameters(newParameters);
        }

        for (int i = 0; i < bestPopulationSize; i++)
        {
            var agent = context.AddAgent<PolynomialCheckAgent>();
            agent.SetParameters(bestParameters[i]);
        }
    }

    private PolynomialParameters RandomFromParents(PolynomialParameters[] parents)
    {
        if (parents.Length == 1)
        {
            return parents[0];
        }

        var numberOfCoefficients = parents[0].Coefficients.Length;
        var newCoefficients = new double[numberOfCoefficients];

        for (int i = 0; i < numberOfCoefficients; i++)
        {
            var parentIndex = randomNumbersGenerator.Next(parents.Length);
            newCoefficients[i] = parents[parentIndex].Coefficients[i];
        }
        return PolynomialParameters.BuildFromCoefficients(newCoefficients);
    }

    private PolynomialParameters AverageFromParents(PolynomialParameters[] parents)
    {
        var numberOfCoefficients = parents[0].Coefficients.Length;
        var newCoefficients = new double[numberOfCoefficients];
        for (int i = 0; i < numberOfCoefficients; i++)
        {
            newCoefficients[i] = parents.Select(x => x.Coefficients[i]).Average();
        }
        return PolynomialParameters.BuildFromCoefficients(newCoefficients);
    }

    private PolynomialParameters GausianFromParents(PolynomialParameters[] parents)
    {
        var numberOfCoefficients = parents[0].Coefficients.Length;
        var newCoefficients = new double[numberOfCoefficients];
        for (int i = 0; i < numberOfCoefficients; i++)
        {
            var mean = parents.Select(x => x.Coefficients[i]).Average();
            var stddev = Math.Sqrt(parents.Select(x => Math.Pow(x.Coefficients[i] - mean, 2)).Average());
            newCoefficients[i] = randomNumbersGenerator.GetNextGaussian(mean, stddev);
        }
        return PolynomialParameters.BuildFromCoefficients(newCoefficients);
    }

    private PolynomialParameters MutateParameters(PolynomialParameters parameters, double multiplier)
    {
        var mutationRate = 0.1;
        var mutationAmount = 1.0 * multiplier;
        var numberOfCoefficients = parameters.Coefficients.Length;
        var newCoefficients = new double[numberOfCoefficients];
        for (int i = 0; i < numberOfCoefficients; i++)
        {
            var coefficient = parameters.Coefficients[i];
            if (randomNumbersGenerator.NextDouble(0, 1) < mutationRate)
            {
                coefficient += randomNumbersGenerator.NextDouble(-mutationAmount, mutationAmount);
            }
            newCoefficients[i] = coefficient;
        }
        return PolynomialParameters.BuildFromCoefficients(newCoefficients);
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
