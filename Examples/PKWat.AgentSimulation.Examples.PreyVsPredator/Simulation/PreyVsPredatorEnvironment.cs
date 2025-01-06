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

    public void PlaceNewBornPreys(IEnumerable<(AgentId NewBorn, AgentId Parent)> newBornPreys)
    {
        foreach (var newBornPrey in newBornPreys)
        {
            var parentCoordinates = GetState().AgentCoordinates[newBornPrey.Parent];
            var x = parentCoordinates.X;
            var y = parentCoordinates.Y;
            GetState().AgentCoordinates[newBornPrey.NewBorn] = (x, y);
            GetState().Preys[(x, y)].Add(newBornPrey.NewBorn);
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

    internal void PlaceNewBornPredators(IEnumerable<(AgentId NewBorn, AgentId Parent)> newBornPredators)
    {
        foreach (var newBornPredator in newBornPredators)
        {
            var parentCoordinates = GetState().AgentCoordinates[newBornPredator.Parent];
            var x = parentCoordinates.X;
            var y = parentCoordinates.Y;
            GetState().AgentCoordinates[newBornPredator.NewBorn] = (x, y);
            GetState().Predators[(x, y)].Add(newBornPredator.NewBorn);
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

    public void MovePredators(IEnumerable<(AgentId Id, MovingDirection Direction)> predatorsDirections)
    {
        var state = GetState();

        foreach (var id in predatorsDirections)
        {
            var (x, y) = state.AgentCoordinates[id.Id];
            var (newX, newY) = GetNewCoordinates(x, y, id.Direction);
            if (x == newX && y == newY)
            {
                continue;
            }

            state.Predators[(x, y)].Remove(id.Id);
            state.AgentCoordinates[id.Id] = (newX, newY);
            state.Predators.TryAdd((newX, newY), new HashSet<AgentId>());
            state.Predators[(newX, newY)].Add(id.Id);
        }

        foreach (var predatorsCoordinates in state.Predators.Where(x => x.Value.Any() == false).Select(x => x.Key))
        {
            state.Predators.Remove(predatorsCoordinates);
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

    internal bool AnotherPrey(AgentId id)
    {
        var state = GetState();
        var coordinates = state.AgentCoordinates[id];
        return state.Preys[(coordinates.X, coordinates.Y)].Count > 1;
    }

    internal AgentId? EatPreyByPredator(AgentId id)
    {
        var state = GetState();
        var coordinates = state.AgentCoordinates[id];

        if(state.Preys.ContainsKey((coordinates.X, coordinates.Y)) == false)
        {
            return null;
        }

        var preys = state.Preys[(coordinates.X, coordinates.Y)];
        var eatenPrey = preys.First();
        preys.Remove(eatenPrey);
        state.AgentCoordinates.Remove(eatenPrey);

        if(preys.Any() == false)
        {
            state.Preys.Remove((coordinates.X, coordinates.Y));
        }

        return eatenPrey;
    }
}
