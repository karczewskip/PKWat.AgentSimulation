namespace PKWat.AgentSimulation.Core.Agent;

public record AgentId
{
    public static AgentId Empty { get; } = new AgentId(Guid.Empty);

    public Guid Id { get; }

    private AgentId(Guid id)
    {
        Id = id;
    }

    public static AgentId GenerateNew() => new AgentId(Guid.NewGuid());

    public override string ToString() => Id.ToString();

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
