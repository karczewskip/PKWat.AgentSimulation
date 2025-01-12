namespace PKWat.AgentSimulation.Core.Stage;

public interface ISimulationStage
{
    Task Execute(ISimulationContext context);
}
