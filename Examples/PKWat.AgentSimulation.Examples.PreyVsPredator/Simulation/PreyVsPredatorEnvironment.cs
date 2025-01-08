using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Environment;
using PKWat.AgentSimulation.Core.RandomNumbers;
using PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation.Agents;

namespace PKWat.AgentSimulation.Examples.PreyVsPredator.Simulation;

public class PreyVsPredatorEnvironment(IRandomNumbersGenerator randomNumbersGenerator) : DefaultSimulationEnvironment
{
    private int width;
    private int height;

    private Dictionary<AgentId, (int X, int Y)> AgentCoordinates = new();
    private Dictionary<(int X, int Y), HashSet<AgentId>> Preys = new();
    private Dictionary<(int X, int Y), HashSet<AgentId>> Predators = new();

    public void SetSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public void PlaceInitialPreys(IEnumerable<AgentId> agentIds)
    {
        foreach (var agentId in agentIds)
        {
            var x = randomNumbersGenerator.Next(width);
            var y = randomNumbersGenerator.Next(height);
            AgentCoordinates[agentId] = (x, y);
            Preys.TryAdd((x, y), new HashSet<AgentId>());
            Preys[(x, y)].Add(agentId);
        }
    }

    public void PlaceNewBornPreys(IEnumerable<(AgentId NewBorn, AgentId Parent)> newBornPreys)
    {
        foreach (var newBornPrey in newBornPreys)
        {
            var parentCoordinates = AgentCoordinates[newBornPrey.Parent];
            var x = parentCoordinates.X;
            var y = parentCoordinates.Y;
            AgentCoordinates[newBornPrey.NewBorn] = (x, y);
            Preys[(x, y)].Add(newBornPrey.NewBorn);
        }
    }

    public void PlaceInitialPredators(IEnumerable<AgentId> agentIds)
    {
        foreach (var agentId in agentIds)
        {
            var x = randomNumbersGenerator.Next(width);
            var y = randomNumbersGenerator.Next(height);
            AgentCoordinates[agentId] = (x, y);
            Predators.TryAdd((x, y), new HashSet<AgentId>());
            Predators[(x, y)].Add(agentId);
        }
    }

    internal void PlaceNewBornPredators(IEnumerable<(AgentId NewBorn, AgentId Parent)> newBornPredators)
    {
        foreach (var newBornPredator in newBornPredators)
        {
            var parentCoordinates = AgentCoordinates[newBornPredator.Parent];
            var x = parentCoordinates.X;
            var y = parentCoordinates.Y;
            AgentCoordinates[newBornPredator.NewBorn] = (x, y);
            Predators[(x, y)].Add(newBornPredator.NewBorn);
        }
    }

    public void MovePreys(IEnumerable<(AgentId Id, MovingDirection Direction)> preysDirections)
    {
        foreach (var id in preysDirections)
        {
            var (x, y) = AgentCoordinates[id.Id];
            var (newX, newY) = GetNewCoordinates(x, y, id.Direction);
            if(x == newX && y == newY)
            {
                continue;
            }

            Preys[(x, y)].Remove(id.Id);
            AgentCoordinates[id.Id] = (newX, newY);
            Preys.TryAdd((newX, newY), new HashSet<AgentId>());
            Preys[(newX, newY)].Add(id.Id);
        }

        foreach(var preysCoordinates in Preys.Where(x => x.Value.Any() == false).Select(x => x.Key))
        {
            Preys.Remove(preysCoordinates);
        }
    }

    public void MovePredators(IEnumerable<(AgentId Id, MovingDirection Direction)> predatorsDirections)
    {
        foreach (var id in predatorsDirections)
        {
            var (x, y) = AgentCoordinates[id.Id];
            var (newX, newY) = GetNewCoordinates(x, y, id.Direction);
            if (x == newX && y == newY)
            {
                continue;
            }

            Predators[(x, y)].Remove(id.Id);
            AgentCoordinates[id.Id] = (newX, newY);
            Predators.TryAdd((newX, newY), new HashSet<AgentId>());
            Predators[(newX, newY)].Add(id.Id);
        }

        foreach (var predatorsCoordinates in Predators.Where(x => x.Value.Any() == false).Select(x => x.Key))
        {
            Predators.Remove(predatorsCoordinates);
        }
    }

    private (int X, int Y) GetNewCoordinates(int x, int y, MovingDirection direction)
    {
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
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    internal bool IsPredatorAt(int i, int j)
    {
        return Predators.ContainsKey((i, j));
    }

    internal bool IsPreyAt(int i, int j)
    {
        return Preys.ContainsKey((i, j));
    }

    internal bool AnotherPrey(AgentId id)
    {
        var coordinates = AgentCoordinates[id];
        return Preys[(coordinates.X, coordinates.Y)].Count > 1;
    }

    internal AgentId? EatPreyByPredator(AgentId id)
    {
        var coordinates = AgentCoordinates[id];

        if(Preys.ContainsKey((coordinates.X, coordinates.Y)) == false)
        {
            return null;
        }

        var preys = Preys[(coordinates.X, coordinates.Y)];
        var eatenPrey = preys.First();
        preys.Remove(eatenPrey);
        AgentCoordinates.Remove(eatenPrey);

        if(preys.Any() == false)
        {
            Preys.Remove((coordinates.X, coordinates.Y));
        }

        return eatenPrey;
    }

    internal void RemovePredator(AgentId id)
    {
        var coordinates = AgentCoordinates[id];
        Predators[(coordinates.X, coordinates.Y)].Remove(id);
        AgentCoordinates.Remove(id);
        if (Predators[(coordinates.X, coordinates.Y)].Any() == false)
        {
            Predators.Remove((coordinates.X, coordinates.Y));
        }
    }
}
