namespace PKWat.AgentSimulation.Examples.TspProblems.Agents;

using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Math.Algorithms.TSP;

public class HeldKarpAgent : SimpleSimulationAgent
{
    public TspSolution? BestSolution { get; private set; }
    public int CurrentSize { get; private set; }
    public int CurrentMask { get; private set; }
    public int CurrentLastNode { get; private set; }
    public Dictionary<(int, int), double>? DpTable { get; private set; }
    public Dictionary<(int, int), int>? ParentTable { get; private set; }
    public bool IsComplete { get; private set; }
    public int TotalPoints { get; private set; }

    public void Initialize(int pointCount)
    {
        BestSolution = TspSolution.Empty();
        CurrentSize = 1;
        CurrentMask = 0;
        CurrentLastNode = 0;
        DpTable = new Dictionary<(int, int), double>();
        ParentTable = new Dictionary<(int, int), int>();
        IsComplete = false;
        TotalPoints = pointCount;

        // Initialize base case: starting from node 0
        int baseMask = 1 << 0;
        DpTable[(baseMask, 0)] = 0;
    }

    public void ProcessState(int mask, int lastNode, double distance, int parent)
    {
        var key = (mask, lastNode);
        
        if (!DpTable.ContainsKey(key) || distance < DpTable[key])
        {
            DpTable[key] = distance;
            ParentTable[key] = parent;
        }

        CurrentMask = mask;
        CurrentLastNode = lastNode;
    }

    public void UpdateCurrentSize(int size)
    {
        CurrentSize = size;
    }

    public void SetBestSolution(TspSolution solution)
    {
        BestSolution = solution;
    }

    public void MarkComplete()
    {
        IsComplete = true;
    }
}
