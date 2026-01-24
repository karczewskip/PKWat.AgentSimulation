using System.Text.Json;
using PKWat.TSPCalculator.Models;

namespace PKWat.TSPCalculator.Utilities;

public static class TspProblemLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static TspProblem LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        var json = File.ReadAllText(filePath);
        var problem = JsonSerializer.Deserialize<TspProblem>(json, JsonOptions);
        
        if (problem == null || problem.Points.Count == 0)
            throw new InvalidDataException("Invalid TSP problem file: no points found");

        return problem;
    }

    public static void SaveToFile(TspProblem problem, string filePath)
    {
        var json = JsonSerializer.Serialize(problem, JsonOptions);
        File.WriteAllText(filePath, json);
    }
}
