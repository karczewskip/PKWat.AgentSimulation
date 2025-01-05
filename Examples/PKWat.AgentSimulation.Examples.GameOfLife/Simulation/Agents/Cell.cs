namespace PKWat.AgentSimulation.Examples.GameOfLife.Simulation.Agents;

using PKWat.AgentSimulation.Core;
using PKWat.AgentSimulation.Core.PerformanceInfo;
using PKWat.AgentSimulation.Core.Time;

public record CellState(bool IsAlive);

internal class Cell(IRandomNumbersGenerator randomNumbersGenerator, ISimulationCyclePerformanceInfo simulationCyclePerformanceInfo) : SimulationAgent<LifeMatrixEnvironment, CellState>
{
    protected override CellState GetInitialState(LifeMatrixEnvironment environment)
    {
        return new CellState(randomNumbersGenerator.GetNextBool());
    }

    protected override CellState GetNextState(LifeMatrixEnvironment environment, IReadOnlySimulationTime simulationTime)
    {
        using var step = simulationCyclePerformanceInfo.AddStep("Cell");
        var aliveNeighbours = environment.GetAliveNeighboursCount(Id);
        if (State.IsAlive)
        {
            var isStillAlive = aliveNeighbours == 2 || aliveNeighbours == 3;
            return new CellState(isStillAlive);
        }
        else
        {
            var isBorn = aliveNeighbours == 3;
            return new CellState(isBorn);
        }
    }
}