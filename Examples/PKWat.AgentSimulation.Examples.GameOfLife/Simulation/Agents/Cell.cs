namespace PKWat.AgentSimulation.Examples.GameOfLife.Simulation.Agents; 

using PKWat.AgentSimulation.Core;

public record CellState(bool IsAlive);

internal class Cell : SimulationAgent<LifeMatrixEnvironment, CellState>
{
    private readonly IRandomNumbersGenerator _randomNumbersGenerator;

    public Cell(IRandomNumbersGenerator randomNumbersGenerator)
    {
        _randomNumbersGenerator = randomNumbersGenerator;
    }

    protected override CellState GetInitialState(LifeMatrixEnvironment environment)
    {
        return new CellState(_randomNumbersGenerator.GetNextBool());
    }

    protected override CellState GetNextState(LifeMatrixEnvironment environment, SimulationTime simulationTime)
    {
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