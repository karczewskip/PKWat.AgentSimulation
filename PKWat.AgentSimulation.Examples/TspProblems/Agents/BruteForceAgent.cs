namespace PKWat.AgentSimulation.Examples.TspProblems.Agents;

using PKWat.AgentSimulation.Core.Agent;

public class BruteForceAgent : SimpleSimulationAgent
{
    public TspSolution? BestSolution { get; private set; }
    public TspSolution? CurrentSolution { get; private set; }
    public List<int>? CurrentPermutation { get; private set; }
    public long PermutationsChecked { get; private set; }
    public bool IsComplete { get; private set; }

    public void Initialize(int pointCount)
    {
        BestSolution = TspSolution.Empty();
        CurrentPermutation = Enumerable.Range(0, pointCount).ToList();
        PermutationsChecked = 0;
        IsComplete = false;
    }

    public void SetCurrentSolution(TspSolution solution)
    {
        CurrentSolution = solution;
        PermutationsChecked++;

        if (solution.TotalDistance < BestSolution!.TotalDistance)
        {
            BestSolution = solution;
        }
    }

    public void UpdatePermutation(List<int> permutation)
    {
        CurrentPermutation = permutation;
    }

    public void MarkComplete()
    {
        IsComplete = true;
    }
}
