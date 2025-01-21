namespace PKWat.AgentSimulation.ExamplesVisualizer.Simulations.PreyVsPredator.Agents;

using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.RandomNumbers;

public class PregnancyStatus
{
    public double Progress { get; private set; }

    public bool InLabour => Progress >= 1;

    private PregnancyStatus(double initialValue)
    {
        Progress = initialValue;
    }

    public void UpdateProgress(double addingProgress)
    {
        var newProgress = Progress + addingProgress;

        if (newProgress > 1)
        {
            newProgress = 1;
        }

        Progress = newProgress;
    }

    public void Reset()
    {
        Progress = 0;
    }

    public static PregnancyStatus StartPregnancy()
    {
        return new PregnancyStatus(initialValue: 0);
    }
}

public class Prey(IRandomNumbersGenerator randomNumbersGenerator) :
    SimpleSimulationAgent
{
    private PregnancyStatus _pregnancy = PregnancyStatus.StartPregnancy();

    public bool IsInLabour => _pregnancy.InLabour;

    internal void ResetAfterLabour()
    {
        _pregnancy.Reset();
    }

    internal void UpdatePregnancy(double pregnancyUpdate)
    {
        _pregnancy.UpdateProgress(pregnancyUpdate);
    }
}
