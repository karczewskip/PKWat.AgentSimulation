namespace PKWat.AgentSimulation.Examples.TspProblems.HeldKarp.Stages;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;
using PKWat.AgentSimulation.Examples.TspProblems.Agents;
using System.Threading.Tasks;

public class ProcessNextHeldKarpState : ISimulationStage
{
    private int _currentSize = 1;
    private List<(int mask, int lastNode)> _currentStates = new();
    private int _currentStateIndex = 0;

    public async Task Execute(ISimulationContext context)
    {
        var environment = context.GetSimulationEnvironment<TspEnvironment>();
        var agent = context.GetAgents<HeldKarpAgent>().First();

        if (agent.IsComplete || environment.DistanceMatrix == null)
            return;

        int n = environment.Points.Count;

        // If we're starting a new size level
        if (_currentStateIndex >= _currentStates.Count)
        {
            _currentSize++;
            agent.UpdateCurrentSize(_currentSize);

            if (_currentSize > n)
            {
                // Reconstruct the best solution
                ReconstructSolution(agent, environment, n);
                agent.MarkComplete();
                return;
            }

            // Generate all states for current size
            _currentStates = GenerateStatesForSize(_currentSize, n);
            _currentStateIndex = 0;
        }

        // Process one state
        if (_currentStateIndex < _currentStates.Count)
        {
            var (mask, lastNode) = _currentStates[_currentStateIndex];
            ProcessState(agent, environment, mask, lastNode, n);
            _currentStateIndex++;
        }

        await Task.CompletedTask;
    }

    private List<(int, int)> GenerateStatesForSize(int size, int n)
    {
        var states = new List<(int, int)>();
        
        // Generate all subsets of size 'size' that include node 0
        GenerateSubsets(0, 0, size, n, 1 << 0, states);
        
        return states;
    }

    private void GenerateSubsets(int current, int count, int targetSize, int n, int mask, List<(int, int)> states)
    {
        if (count == targetSize - 1)
        {
            // Add all possible last nodes for this subset
            for (int i = 1; i < n; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    states.Add((mask, i));
                }
            }
            return;
        }

        for (int i = current + 1; i < n; i++)
        {
            GenerateSubsets(i, count + 1, targetSize, n, mask | (1 << i), states);
        }
    }

    private void ProcessState(HeldKarpAgent agent, TspEnvironment environment, int mask, int lastNode, int n)
    {
        var dpTable = agent.DpTable!;
        var parentTable = agent.ParentTable!;
        var distances = environment.DistanceMatrix!;

        double minDist = double.MaxValue;
        int bestParent = -1;

        // Try all possible previous nodes
        for (int k = 0; k < n; k++)
        {
            if (k == lastNode || (mask & (1 << k)) == 0)
                continue;

            int prevMask = mask & ~(1 << lastNode);
            var prevKey = (prevMask, k);

            if (dpTable.ContainsKey(prevKey))
            {
                double dist = dpTable[prevKey] + distances[k, lastNode];
                if (dist < minDist)
                {
                    minDist = dist;
                    bestParent = k;
                }
            }
        }

        if (bestParent != -1)
        {
            agent.ProcessState(mask, lastNode, minDist, bestParent);
        }
    }

    private void ReconstructSolution(HeldKarpAgent agent, TspEnvironment environment, int n)
    {
        var dpTable = agent.DpTable!;
        var parentTable = agent.ParentTable!;
        var distances = environment.DistanceMatrix!;

        // Find the minimum cost to return to start
        int fullMask = (1 << n) - 1;
        double minCost = double.MaxValue;
        int lastNode = -1;

        for (int i = 1; i < n; i++)
        {
            var key = (fullMask, i);
            if (dpTable.ContainsKey(key))
            {
                double cost = dpTable[key] + distances[i, 0];
                if (cost < minCost)
                {
                    minCost = cost;
                    lastNode = i;
                }
            }
        }

        if (lastNode == -1)
            return;

        // Reconstruct the path
        var route = new List<int>();
        int currentMask = fullMask;
        int currentNode = lastNode;

        while (currentMask != (1 << 0))
        {
            route.Add(currentNode);
            var key = (currentMask, currentNode);
            int parent = parentTable[key];
            currentMask &= ~(1 << currentNode);
            currentNode = parent;
        }

        route.Add(0); // Start node
        route.Reverse();

        var solution = TspSolution.Create(route, minCost);
        agent.SetBestSolution(solution);
        environment.UpdateBestSolution(solution);
    }
}
