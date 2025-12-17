using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.Cuda;
using PKWat.AgentSimulation.Core.PerformanceInfo;

public class GpuPolynomialEvaluator : IDisposable
{
    private readonly Context _context;
    private readonly Accelerator _accelerator;
    private readonly ISimulationCyclePerformanceInfo _simulationCyclePerformanceInfo;

    public GpuPolynomialEvaluator(ISimulationCyclePerformanceInfo simulationCyclePerformanceInfo)
    {
        // Initialize ILGPU context and select CUDA device
        _context = Context.Create(builder => builder.Cuda());
        _accelerator = _context.GetCudaDevice(0).CreateAccelerator(_context);
        _simulationCyclePerformanceInfo = simulationCyclePerformanceInfo;
    }

    // struct to match your logic: sum and mean
    public struct GpuErrorResult
    {
        public double SumError;
        public double MeanError;
    }

    /// <summary>
    /// The Kernel runs on the GPU.
    /// Each thread represents ONE Agent evaluating its polynomial against ALL expected values.
    /// </summary>
    /// <param name="index">The unique ID of the thread (Agent Index)</param>
    /// <param name="degree">Polynomial degree (l)</param>
    /// <param name="numPoints">Number of expected values (n)</param>
    /// <param name="xValues">Array of expected X values</param>
    /// <param name="yValues">Array of expected Y values</param>
    /// <param name="allCoefficients">Flattened array of all agents' coefficients</param>
    /// <param name="results">Output array where results are stored</param>
    static void EvaluationKernel(
        Index1D index,
        int degree,
        int numPoints,
        ArrayView<double> xValues,
        ArrayView<double> yValues,
        ArrayView<double> allCoefficients,
        ArrayView<GpuErrorResult> results)
    {
        int agentIndex = index;
        double absoluteErrorSum = 0;

        // Calculate the starting position of coefficients for this specific agent
        // Since every agent has (degree + 1) coefficients, we skip: agentIndex * (degree + 1)
        int coeffOffset = agentIndex * (degree + 1);

        // Loop through all Expected Values
        for (int i = 0; i < numPoints; i++)
        {
            double x = xValues[i];
            double expectedY = yValues[i];

            // Horner's method / Polynomial evaluation
            // Logic copied exactly from your PolynomialCheckAgent
            double predictedY = 0;
            for (int d = 0; d <= degree; d++)
            {
                predictedY *= x;
                predictedY += allCoefficients[coeffOffset + d];
            }

            double diff = predictedY - expectedY;

            // Math.Abs implementation for GPU
            if (diff < 0) diff = -diff;

            absoluteErrorSum += diff;
        }

        // Write result to the output array
        results[agentIndex] = new GpuErrorResult
        {
            SumError = absoluteErrorSum,
            MeanError = absoluteErrorSum / numPoints
        };
    }

    /// <summary>
    /// Host method to prepare data and launch the GPU kernel.
    /// </summary>
    /// <param name="expectedX">N expected X values</param>
    /// <param name="expectedY">N expected Y values</param>
    /// <param name="agentsCoefficients">List of K coefficient arrays (each length l+1)</param>
    /// <returns>Array of ErrorResults for each agent</returns>
    public GpuErrorResult[] CalculateErrors(double[] expectedX, double[] expectedY, double[][] agentsCoefficients)
    {
        var loadDataToGpuStep = _simulationCyclePerformanceInfo.AddManualStep("Load data to GPU");

        int numAgents = agentsCoefficients.Length;
        int numPoints = expectedX.Length;
        // Assuming all agents have the same polynomial degree based on the first agent
        int coeffsPerAgent = agentsCoefficients[0].Length;
        int degree = coeffsPerAgent - 1;

        // 1. Flatten the coefficients (CPU side)
        // We convert double[][] to a single double[] because GPU memory is linear.
        double[] flatCoefficients = new double[numAgents * coeffsPerAgent];

        // Using Parallel.For on CPU to flatten quickly before sending to GPU
        System.Threading.Tasks.Parallel.For(0, numAgents, i =>
        {
            for (int j = 0; j < coeffsPerAgent; j++)
            {
                flatCoefficients[i * coeffsPerAgent + j] = agentsCoefficients[i][j];
            }
        });

        // 2. Allocate memory on GPU
        using var deviceX = _accelerator.Allocate1D(expectedX);
        using var deviceY = _accelerator.Allocate1D(expectedY);
        using var deviceCoeffs = _accelerator.Allocate1D(flatCoefficients);
        using var deviceResults = _accelerator.Allocate1D<GpuErrorResult>(numAgents);

        loadDataToGpuStep.Stop();

        var calculationStep = _simulationCyclePerformanceInfo.AddManualStep("GPU Calculation");

        // 3. Load Kernel
        var kernel = _accelerator.LoadAutoGroupedStreamKernel<
            Index1D, int, int, ArrayView<double>, ArrayView<double>, ArrayView<double>, ArrayView<GpuErrorResult>
            >(EvaluationKernel);

        // 4. Execute Kernel
        // We launch 'numAgents' threads. Each thread handles one full polynomial evaluation.
        kernel(numAgents, degree, numPoints, deviceX.View, deviceY.View, deviceCoeffs.View, deviceResults.View);

        // 5. Synchronize and get results
        _accelerator.Synchronize();

        calculationStep.Stop();

        return deviceResults.GetAsArray1D();
    }

    public void Dispose()
    {
        _accelerator.Dispose();
        _context.Dispose();
    }
}