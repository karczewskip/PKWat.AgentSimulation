namespace PKWat.AgentSimulation.Examples.TspProblems.Agents;

using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Math.Algorithms.TSP;

public class MstAgent : SimpleSimulationAgent
{
    public TspSolution? BestSolution { get; private set; }
    public List<int>? MstRoute { get; private set; }
    public List<int>? CurrentRoute { get; private set; }
    public HashSet<int>? VisitedNodes { get; private set; }
    public List<(int from, int to)>? MstEdges { get; private set; }
    public HashSet<int>? NodesInMst { get; private set; }
    public bool IsMstBuilt { get; private set; }
    public bool IsDfsStarted { get; private set; }
    public bool IsComplete { get; private set; }
    public int CurrentStep { get; private set; }
    public double[,]? Distances { get; private set; }

    public void Initialize(int pointCount, double[,] distanceMatrix)
    {
        BestSolution = TspSolution.Empty();
        MstRoute = new List<int>();
        CurrentRoute = new List<int>();
        VisitedNodes = new HashSet<int>();
        MstEdges = new List<(int from, int to)>();
        NodesInMst = new HashSet<int>();
        IsMstBuilt = false;
        IsDfsStarted = false;
        IsComplete = false;
        CurrentStep = 0;
        Distances = distanceMatrix;
    }

    public void AddMstEdge(int from, int to)
    {
        MstEdges!.Add((from, to));
        NodesInMst!.Add(from);
        NodesInMst!.Add(to);
    }

    public void CompleteMstBuilding(List<int> dfsRoute)
    {
        MstRoute = new List<int>(dfsRoute);
        IsMstBuilt = true;
    }

    public void StartDfs()
    {
        IsDfsStarted = true;
    }

    public void AddNodeToRoute(int node)
    {
        CurrentRoute!.Add(node);
        VisitedNodes!.Add(node);
        CurrentStep++;
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
