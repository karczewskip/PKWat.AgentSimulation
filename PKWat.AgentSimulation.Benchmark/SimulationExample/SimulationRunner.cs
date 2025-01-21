namespace PKWat.AgentSimulation.Benchmark.SimulationExample;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Builder;
using PKWat.AgentSimulation.Core.Crash;
using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.Core.Stage;
using System.Linq;
using System.Threading.Tasks;

internal class SimulationRunner(ISimulationBuilder simulationBuilder)
{
    public async Task RunSimulation(int numberOfAgents)
    {
        var simulationContext = simulationBuilder.CreateNewSimulation<ExampleEnvironment>()
            .AddAgents<AgentWithCounter>(numberOfAgents)
            .AddStage<UpdateCountersStage>()
            .AddCrashCondition(c => c.GetAgents<AgentWithCounter>().Any(x => x.Counter > 1000) ? SimulationCrashResult.Crash("Counter exceeded") : SimulationCrashResult.NoCrash)
            .Build();

        await simulationContext.StartAsync();
    }

    public class ExampleEnvironment : DefaultSimulationEnvironment
    {
    }

    public class AgentWithCounter : SimpleSimulationAgent
    {
        public int Counter { get; set; }
    }

    public class UpdateCountersStage : ISimulationStage
    {
        public async Task Execute(ISimulationContext context)
        {
            var agents = context.GetAgents<AgentWithCounter>().ToArray();
            foreach (var agent in agents)
            {
                agent.Counter++;
            }
        }
    }

}
