namespace PKWat.AgentSimulation.Core.Snapshots;

using System;
using System.Text.Json;
using System.Threading.Tasks;
using PKWat.AgentSimulation.Core;

internal record SimulationTimeSnapshot(SimulationTime SimulationTime);

internal record SimulationEnvironmentSnapshot(string Snapshot);

internal record SimulationAgentSnapshot(AgentId Id, string Snapshot);

internal record SimulationSnapshotConfiguration(string Directory);

internal record SimulationSnapshot(SimulationTimeSnapshot TimeSnapshot, SimulationEnvironmentSnapshot EnvironmentSnapshot, SimulationAgentSnapshot[] AgentSnapshots);

internal class SimulationSnapshotStore(SimulationSnapshotConfiguration simulationSnapshotConfiguration)
{
    public async Task SaveSnapshotAsync(SimulationSnapshot snapshot, CancellationToken cancellationToken)
    {
        string filePath = Path.Combine(simulationSnapshotConfiguration.Directory, $"{snapshot.TimeSnapshot.SimulationTime.StepNo}.json");

        using var output = new Utf8JsonWriter(File.OpenWrite(filePath), new JsonWriterOptions { Indented = true });
        output.WriteStartObject();

        using JsonDocument time = JsonDocument.Parse(JsonSerializer.Serialize(snapshot.TimeSnapshot, new JsonSerializerOptions { WriteIndented = true }));

        output.WritePropertyName("Time");
        time.RootElement.WriteTo(output);

        using JsonDocument environment = JsonDocument.Parse(snapshot.EnvironmentSnapshot.Snapshot);

        output.WritePropertyName("Environment");
        environment.RootElement.WriteTo(output);

        output.WritePropertyName("Agents");
        output.WriteStartArray();

        foreach (var agentSnapshot in snapshot.AgentSnapshots)
        {
            using JsonDocument agent = JsonDocument.Parse(agentSnapshot.Snapshot);

            output.WriteStartObject();

            output.WritePropertyName("Id");
            output.WriteStringValue(agentSnapshot.Id.Id.ToString());

            output.WritePropertyName("Snapshot");
            agent.RootElement.WriteTo(output);

            output.WriteEndObject();
        }

        output.WriteEndArray();
        output.WriteEndObject();

        await output.FlushAsync(cancellationToken);
    }
}
