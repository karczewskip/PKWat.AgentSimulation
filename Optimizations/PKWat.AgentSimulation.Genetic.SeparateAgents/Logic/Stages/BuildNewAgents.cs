using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Stage;

namespace PKWat.AgentSimulation.Genetic.SeparateAgents.Logic.Stages;

internal class BuildNewAgents : ISimulationStage
{
    public async Task Execute(ISimulationContext context)
    {
        context.RemoveAgents(context.GetAgents<PolynomialCheckAgent>().Select(x => x.Id));

        foreach(var i in Enumerable.Range(0, 1000))
        {
            var agent = context.AddAgent<PolynomialCheckAgent>();
            agent.SetParameters(PolynomialParameters.BuildFromCoefficients([1, 2, i]));
        }
    }
}
