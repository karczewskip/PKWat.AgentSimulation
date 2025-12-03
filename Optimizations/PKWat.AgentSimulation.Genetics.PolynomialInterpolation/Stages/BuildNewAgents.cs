using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Genetics.PolynomialInterpolation;

namespace PKWat.AgentSimulation.Genetics.PolynomialInterpolation.Stages;

public class BuildNewAgents(IRandomNumbersGenerator randomNumbersGenerator) : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        var numberOfAgentsToGenerate = 1000;
        var numberOfCoefficients = 6;

        if (context.Time.StepNo == 1)
        {
            AddRandomGeneration(context, numberOfAgentsToGenerate, numberOfCoefficients);
            return;
        }

        var numberOfChecksWithoutImprovement = context.GetSimulationEnvironment<CalculationsBlackboard>().NumberOfChecksWithoutImprovement;
        if (numberOfChecksWithoutImprovement % 50 == 0)
        {
            GenerateNewWithElite(context, 1, numberOfAgentsToGenerate, numberOfCoefficients);
            return;
        }

        var numberOfNewRandomAgents = (int)(numberOfAgentsToGenerate * 0.1);
        GenerateUsingTournament(context, numberOfAgentsToGenerate - numberOfNewRandomAgents, numberOfCoefficients);
        AddRandomGeneration(context, numberOfNewRandomAgents, numberOfCoefficients);
    }

    private void AddRandomGeneration(ISimulationContext context, int numberOfAgentsToGenerate, int numberOfCoefficients)
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

        var bestParameters = GetBestParamters(context, bestPopulationSize);
        context.RemoveAgents(context.GetAgents<PolynomialCheckAgent>().Select(x => x.Id));
        for (int i = 0; i < bestPopulationSize; i++)
        {
            var agent = context.AddAgent<PolynomialCheckAgent>();
            agent.SetParameters(bestParameters[i]);
        }

        AddRandomGeneration(context, numberOfAgentsToGenerate - bestPopulationSize, numberOfCoefficients);
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

    private void GenerateUsingTournament(ISimulationContext context, int numberOfAgentsToGenerate, int numberOfCoefficients)
    {
        var blackboard = context.GetSimulationEnvironment<CalculationsBlackboard>();
        var allAgentsWithErrors = blackboard.AgentErrors.ToList();
        var generatedParamters = new List<PolynomialParameters>();

        for (int i = 0; i < numberOfAgentsToGenerate - 1; i++)
        {
            var parent1 = TournamentSelection(context, allAgentsWithErrors, 5); // 5 fighters in tournament
            var parent2 = TournamentSelection(context, allAgentsWithErrors, 5);

            var newParameters = GaussianFromParents([parent1, parent2]);

            generatedParamters.Add(MutateProportional(newParameters));
        }

        var bestId = allAgentsWithErrors.OrderBy(x => x.Value.AbsoluteError).First().Key;
        var bestParams = context.GetRequiredAgent<PolynomialCheckAgent>(bestId).Parameters!;

        context.RemoveAgents(context.GetAgents<PolynomialCheckAgent>().Select(x => x.Id));

        var champion = context.AddAgent<PolynomialCheckAgent>();
        champion.SetParameters(bestParams);

        foreach(var generatedParamtersForSingleAgent in generatedParamters)
        {
            var agent = context.AddAgent<PolynomialCheckAgent>();
            agent.SetParameters(generatedParamtersForSingleAgent);
        }
    }

    private PolynomialParameters TournamentSelection(
        ISimulationContext context,
        List<KeyValuePair<AgentId, ErrorResult>> allAgents,
        int tournamentSize)
    {
        PolynomialParameters bestCandidate = null!;
        double bestError = double.MaxValue;

        for (int i = 0; i < tournamentSize; i++)
        {
            var randomCandidate = allAgents[randomNumbersGenerator.Next(allAgents.Count)];
            if (randomCandidate.Value.AbsoluteError < bestError)
            {
                bestError = randomCandidate.Value.AbsoluteError;
                bestCandidate = context.GetRequiredAgent<PolynomialCheckAgent>(randomCandidate.Key).Parameters!;
            }
        }
        return bestCandidate;
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

    private PolynomialParameters GaussianFromParents(PolynomialParameters[] parents)
    {
        var numberOfCoefficients = parents[0].Coefficients.Length;
        var newCoefficients = new double[numberOfCoefficients];

        for (int i = 0; i < numberOfCoefficients; i++)
        {
            var mean = parents.Select(x => x.Coefficients[i]).Average();

            double variance = parents.Select(x => Math.Pow(x.Coefficients[i] - mean, 2)).Average();
            double stddev = Math.Sqrt(variance);

            double minStdDev = 0.05;
            if (stddev < minStdDev) stddev = minStdDev;

            newCoefficients[i] = randomNumbersGenerator.GetNextGaussian(mean, stddev);
        }
        return PolynomialParameters.BuildFromCoefficients(newCoefficients);
    }

    private PolynomialParameters MutateParameters(PolynomialParameters parameters, bool isPanicMode)
    {
        double mutationRate = 0.05;
        double mutationStrength = 0.1;

        if (isPanicMode)
        {
            mutationRate = 0.2;
            mutationStrength = 2.0;
        }

        var numberOfCoefficients = parameters.Coefficients.Length;
        var newCoefficients = new double[numberOfCoefficients];

        for (int i = 0; i < numberOfCoefficients; i++)
        {
            var coefficient = parameters.Coefficients[i];

            if (randomNumbersGenerator.NextDouble(0, 1) < mutationRate)
            {
                coefficient += randomNumbersGenerator.GetNextGaussian(0, mutationStrength);
            }
            newCoefficients[i] = coefficient;
        }
        return PolynomialParameters.BuildFromCoefficients(newCoefficients);
    }

    private PolynomialParameters MutateProportional(PolynomialParameters parameters)
    {
        double mutationRate = 0.1;
        double mutationScale = 0.5;

        var numberOfCoefficients = parameters.Coefficients.Length;
        var newCoefficients = new double[numberOfCoefficients];

        for (int i = 0; i < numberOfCoefficients; i++)
        {
            var coefficient = parameters.Coefficients[i];

            if (randomNumbersGenerator.NextDouble(0, 1) < mutationRate)
            {
                double percentChange = 1.0 + randomNumbersGenerator.NextDouble(-mutationScale, mutationScale);
                coefficient *= percentChange;

                coefficient += randomNumbersGenerator.NextDouble(-0.01, 0.01);
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
