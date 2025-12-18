using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using PKWat.AgentSimulation.Math.Algorithms.TSP;

// Define a custom configuration to ensure all exporters are active
public class TspBenchmarkConfig : ManualConfig
{
    public TspBenchmarkConfig()
    {
        // Add default columns and loggers
        AddColumnProvider(DefaultConfig.Instance.GetColumnProviders().ToArray());
        AddLogger(ConsoleLogger.Default);

        // Exporters generate files in BenchmarkDotNet.Artifacts
        AddExporter(HtmlExporter.Default);   // Great for viewing in a browser
        AddExporter(CsvExporter.Default);    // Best for manual charting in Excel
        AddExporter(RPlotExporter.Default);  // Requires R installed to generate PNGs
    }
}

[Config(typeof(TspBenchmarkConfig))]
// MemoryDiagnoser enables memory allocation tracking
[MemoryDiagnoser]
public class TspBenchmark
{
    private List<TspPoint> _points = new();
    private readonly CancellationTokenSource _cts = new();

    // Concrete instances of your algorithms
    private readonly ITspAlgorithm _bruteForceSolver = new BruteForceAlgorithm();
    private readonly ITspAlgorithm _heldKarpSolver = new HeldKarpAlgorithm();
    private readonly ITspAlgorithm _mstPrimSolver = new MstPrimAlgorithm();

    // Parameter to test different data sizes
    [ParamsSource(nameof(PointCounts))]
    public int NumberOfPoints { get; set; }

    public IEnumerable<int> PointCounts => Enumerable.Range(start: 4, count: 3);

    // This runs once before the benchmarks for a specific Params set.
    // It is NOT included in the time/memory measurement of [Benchmark] methods.
    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42); // Seeded for reproducibility
        _points = Enumerable.Range(0, NumberOfPoints)
            .Select(id => TspPoint.Create(id, random.NextDouble() * 1000, random.NextDouble() * 1000))
            .ToList();
    }

    [Benchmark(Baseline = true)]
    public TspSolution? HeldKarpBenchmark()
    {
        return _heldKarpSolver.Solve(_points, _cts.Token);
    }

    [Benchmark]
    public TspSolution? MstPrimBenchmark()
    {
        return _mstPrimSolver.Solve(_points, _cts.Token);
    }

    [Benchmark]
    public TspSolution? BruteForceBenchmark()
    {
        // Warning: Brute force will hang for large N!
        if (NumberOfPoints > 12) return null;
        return _bruteForceSolver.Solve(_points, _cts.Token);
    }
}