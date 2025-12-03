using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Genetics.PolynomialAproximation;

namespace PKWat.AgentSimulation.Genetics.PolynomialAproximation.Stages;

public class CalculateForAllAgentsByGPU : ISimulationStage
{
    // Consider injecting this as a singleton to avoid re-initializing CUDA context every time
    private readonly GpuPolynomialEvaluator _gpuEvaluator = new GpuPolynomialEvaluator();

    public async Task Execute(ISimulationContext context)
    {
        var blackboard = context.GetSimulationEnvironment<CalculationsBlackboard>();
        var agents = context.GetAgents<PolynomialCheckAgent>().ToArray();

        blackboard.AgentErrors.Clear();

        // 1. Prepare Data for GPU
        // Extract plain arrays from objects
        double[] xData = blackboard.ExpectedValues.X;
        double[] yData = blackboard.ExpectedValues.Y;

        // Convert List of Agents to Array of Coefficients (double[][])
        double[][] allCoefficients = agents
            .Select(a => a.Parameters!.Coefficients)
            .ToArray();

        // 2. Run on GPU
        var gpuResults = _gpuEvaluator.CalculateErrors(xData, yData, allCoefficients);

        // 3. Map results back to Agents
        for (int i = 0; i < agents.Length; i++)
        {
            var result = gpuResults[i];

            // Create your domain ErrorResult object from the GPU raw data
            // Assuming ErrorResult constructor takes (sum, mean)
            var domainError = new ErrorResult(result.SumError, result.MeanError);

            blackboard.AgentErrors[agents[i].Id] = domainError;
        }

        blackboard.CheckBestResult();
    }
}
