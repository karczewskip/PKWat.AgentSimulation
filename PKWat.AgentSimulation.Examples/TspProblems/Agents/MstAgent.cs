namespace PKWat.AgentSimulation.Examples.TspProblems.Agents;

using PKWat.AgentSimulation.Core.Agent;

public class MstAgent : SimpleSimulationAgent
{
    public TspSolution? BestSolution { get; private set; }
    public List<int>? MstRoute { get; private set; }
    public List<int>? CurrentRoute { get; private set; }
    public HashSet<int>? VisitedNodes { get; private set; }
    public bool IsMstBuilt { get; private set; }
    public bool IsComplete { get; private set; }
    public int CurrentStep { get; private set; }
    public double[,]? Distances { get; private set; }

    public void Initialize(int pointCount, double[,] distanceMatrix)
    {
        BestSolution = TspSolution.Empty();
        MstRoute = new List<int>();
        CurrentRoute = new List<int>();
        VisitedNodes = new HashSet<int>();
        IsMstBuilt = false;
        IsComplete = false;
        CurrentStep = 0;
        Distances = distanceMatrix;
    }

    public void BuildMst(List<int> mstRoute)
    {
        MstRoute = new List<int>(mstRoute);
        IsMstBuilt = true;
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
