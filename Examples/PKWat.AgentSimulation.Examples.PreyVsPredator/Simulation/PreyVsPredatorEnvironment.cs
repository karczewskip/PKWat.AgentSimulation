using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Agents;

namespace PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation;

public record PreyVsPredatorEnvironmentState(
    int Width,
    int Height,
    Dictionary<AgentId, (int X, int Y)> AgentCoordinates,
    Dictionary<(int X, int Y), HashSet<AgentId>> Preys,
    Dictionary<(int X, int Y), HashSet<AgentId>> Predators)
{
    public static PreyVsPredatorEnvironmentState New(int width, int height)
    {
        return new PreyVsPredatorEnvironmentState(
            Width: width,
            Height: height,
            AgentCoordinates: new Dictionary<AgentId, (int X, int Y)>(),
            Preys: new Dictionary<(int X, int Y), HashSet<AgentId>>(),
            Predators: new Dictionary<(int X, int Y), HashSet<AgentId>>());
    }
}

public class PreyVsPredatorEnvironment(IRandomNumbersGenerator randomNumbersGenerator) : DefaultSimulationEnvironment<PreyVsPredatorEnvironmentState>
{
    public void PlaceInitialPreys(IEnumerable<AgentId> agentIds)
    {
        var width = GetState().Width;
        var height = GetState().Height;

        foreach (var agentId in agentIds)
        {
            var x = randomNumbersGenerator.Next(width);
            var y = randomNumbersGenerator.Next(height);
            GetState().AgentCoordinates[agentId] = (x, y);
            GetState().Preys.TryAdd((x, y), new HashSet<AgentId>());
            GetState().Preys[(x, y)].Add(agentId);
        }
    }

    public void PlaceInitialPredators(IEnumerable<AgentId> agentIds)
    {
        var width = GetState().Width;
        var height = GetState().Height;
        foreach (var agentId in agentIds)
        {
            var x = randomNumbersGenerator.Next(width);
            var y = randomNumbersGenerator.Next(height);
            GetState().AgentCoordinates[agentId] = (x, y);
            GetState().Predators.TryAdd((x, y), new HashSet<AgentId>());
            GetState().Predators[(x, y)].Add(agentId);
        }
    }

    public void MovePreys(IEnumerable<(AgentId Id, MovingDirection Direction)> preysDirections)
    {
        var state = GetState();

        foreach (var id in preysDirections)
        {
            var (x, y) = state.AgentCoordinates[id.Id];
            var (newX, newY) = GetNewCoordinates(x, y, id.Direction);
            if(x == newX && y == newY)
            {
                continue;
            }

            state.Preys[(x, y)].Remove(id.Id);
            state.AgentCoordinates[id.Id] = (newX, newY);
            state.Preys.TryAdd((newX, newY), new HashSet<AgentId>());
            state.Preys[(newX, newY)].Add(id.Id);
        }

        foreach(var preysCoordinates in state.Preys.Where(x => x.Value.Any() == false).Select(x => x.Key))
        {
            state.Preys.Remove(preysCoordinates);
        }
    }

    private (int X, int Y) GetNewCoordinates(int x, int y, MovingDirection direction)
    {
        var width = GetState().Width;
        var height = GetState().Height;
        var newX = x;
        var newY = y;
        switch (direction)
        {
            case MovingDirection.Up:
                newY = (y + 1) >= height ? y : y + 1;
                break;
            case MovingDirection.Down:
                newY = (y - 1) < 0 ? 0 : y - 1;
                break;
            case MovingDirection.Left:
                newX = (x - 1) < 0 ? 0 : x - 1;
                break;
            case MovingDirection.Right:
                newX = (x + 1) >= width ? x : x + 1;
                break;
        }
        return (newX, newY);
    }

    public int GetWidth()
    {
        return GetState().Width;
    }

    public int GetHeight()
    {
        return GetState().Height;
    }

    internal bool IsPredatorAt(int i, int j)
    {
        return GetState().Predators.ContainsKey((i, j));
    }

    internal bool IsPreyAt(int i, int j)
    {
        return GetState().Preys.ContainsKey((i, j));
    }
}
