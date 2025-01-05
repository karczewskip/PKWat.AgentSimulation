namespace PKWat.AgentSimulation.Core.Snapshots;

using System.Text.Json;
using System.Threading.Tasks;
using PKWat.AgentSimulation.Core.Agent;
using PKWat.AgentSimulation.Core.Time;

internal record SimulationTimeSnapshot(IReadOnlySimulationTime SimulationTime);

internal record SimulationEnvironmentSnapshot(object Snapshot);

internal record SimulationAgentSnapshot(string AgentType, AgentId Id, object Snapshot);

internal record SimulationSnapshotConfiguration(string Directory);

internal record SimulationSnapshot(SimulationTimeSnapshot TimeSnapshot, SimulationEnvironmentSnapshot EnvironmentSnapshot, SimulationAgentSnapshot[] AgentSnapshots);

internal interface ISimulationSnapshotStore
{
    void CleanExistingSnapshots();
    Task SaveSnapshotAsync(SimulationSnapshot snapshot, CancellationToken cancellationToken);
}

internal class SimulationSnapshotStore(SimulationSnapshotConfiguration simulationSnapshotConfiguration) : ISimulationSnapshotStore
{
    public void CleanExistingSnapshots()
    {
        if (Directory.Exists(simulationSnapshotConfiguration.Directory))
        {
            Directory.Delete(simulationSnapshotConfiguration.Directory, true);
        }

        Directory.CreateDirectory(simulationSnapshotConfiguration.Directory);
    }

    public async Task SaveSnapshotAsync(SimulationSnapshot snapshot, CancellationToken cancellationToken)
    {
        string filePath = Path.Combine(simulationSnapshotConfiguration.Directory, $"{snapshot.TimeSnapshot.SimulationTime.StepNo}.json");

        using var output = new Utf8JsonWriter(File.OpenWrite(filePath), new JsonWriterOptions { Indented = true });
        output.WriteStartObject();

        using JsonDocument time = JsonDocument.Parse(JsonSerializer.Serialize(snapshot.TimeSnapshot, new JsonSerializerOptions { WriteIndented = true }));

        output.WritePropertyName("Time");
        time.RootElement.WriteTo(output);

        using JsonDocument environment = JsonDocument.Parse(JsonSerializer.Serialize(snapshot.EnvironmentSnapshot, new JsonSerializerOptions { WriteIndented = true }));

        output.WritePropertyName("Environment");
        environment.RootElement.WriteTo(output);

        output.WritePropertyName("Agents");
        output.WriteStartArray();

        foreach (var agentSnapshot in snapshot.AgentSnapshots)
        {
            using JsonDocument agent = JsonDocument.Parse(JsonSerializer.Serialize(agentSnapshot.Snapshot, new JsonSerializerOptions { WriteIndented = true }));

            output.WriteStartObject();

            output.WritePropertyName("Id");
            output.WriteStringValue(agentSnapshot.Id.ToString());

            output.WritePropertyName("Type");
            output.WriteStringValue(agentSnapshot.AgentType);

            output.WritePropertyName("Snapshot");
            agent.RootElement.WriteTo(output);

            output.WriteEndObject();
        }

        output.WriteEndArray();
        output.WriteEndObject();

        await output.FlushAsync(cancellationToken);
    }
}

internal class NullSimulationSnapshotStore : ISimulationSnapshotStore
{
    public void CleanExistingSnapshots()
    {
    }
    public Task SaveSnapshotAsync(SimulationSnapshot snapshot, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
