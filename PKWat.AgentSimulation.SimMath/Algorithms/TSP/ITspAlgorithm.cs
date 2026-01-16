namespace PKWat.AgentSimulation.SimMath.Algorithms.TSP;

public interface ITspAlgorithm
{
    /// <summary>
    /// Solves the TSP problem for the given points.
    /// </summary>
    /// <param name="points">List of TSP points to visit</param>
    /// <param name="cancellationToken">Token to cancel the operation if time limit is exceeded</param>
    /// <returns>The best TSP solution found, or null if cancelled</returns>
    TspSolution? Solve(List<TspPoint> points, CancellationToken cancellationToken);
}
