namespace PKWat.AgentSimulation.Core.Stage;

using PKWat.AgentSimulation.Core.Environment;

public interface ISimulationStage
{
}

public interface ISimulationStage<ENVIRONMENT> : ISimulationStage where ENVIRONMENT : ISimulationEnvironment
{
    Task Execute(ISimulationContext<ENVIRONMENT> context);
}
