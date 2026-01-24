using PKWat.AgentSimulation.SimMath.Algorithms.TSP;
using PKWat.TSPCalculator.Models;
using PKWat.TSPCalculator.Utilities;

Console.WriteLine("=== TSP Calculator ===");
Console.WriteLine();

bool continueRunning = true;
TspProblem? currentProblem = null;

while (continueRunning)
{
    if (currentProblem == null)
    {
        currentProblem = LoadOrGenerateProblem();
        if (currentProblem == null)
        {
            Console.WriteLine("Failed to load or generate problem.");
            Console.WriteLine();
            continue;
        }
        
        Console.WriteLine($"\nProblem: {currentProblem.Name}");
        Console.WriteLine($"Number of points: {currentProblem.Points.Count}");
        Console.WriteLine();
    }

    var algorithm = ChooseAlgorithm();
    if (algorithm == null)
    {
        Console.WriteLine("Invalid algorithm selection.");
        Console.WriteLine();
        continue;
    }

    SolveProblem(currentProblem, algorithm);

    var nextAction = ShowPostSolutionMenu();
    switch (nextAction)
    {
        case "1":
            currentProblem = null;
            break;
        case "2":
            break;
        case "3":
            currentProblem = null;
            break;
        case "4":
            continueRunning = false;
            break;
    }
    
    Console.WriteLine();
}

Console.WriteLine("Thank you for using TSP Calculator!");

static TspProblem? LoadOrGenerateProblem()
{
    TspProblem? problem = null;
    
    while (problem == null)
    {
        Console.WriteLine("Choose an option:");
        Console.WriteLine("1. Load TSP problem from JSON file");
        Console.WriteLine("2. Generate random TSP problem");
        Console.Write("\nYour choice: ");
        
        var choice = Console.ReadLine();
        
        if (choice == "1")
        {
            problem = LoadProblemFromFile();
        }
        else if (choice == "2")
        {
            problem = GenerateRandomProblem();
        }
        else
        {
            Console.WriteLine("Invalid choice. Please enter 1 or 2.");
            Console.WriteLine();
        }
        
        if (problem != null)
            break;
    }
    
    return problem;
}

static string ShowPostSolutionMenu()
{
    Console.WriteLine("\n=== What would you like to do next? ===");
    Console.WriteLine("1. Load another problem");
    Console.WriteLine("2. Choose another algorithm (same problem)");
    Console.WriteLine("3. Generate new problem");
    Console.WriteLine("4. Exit");
    Console.Write("\nYour choice: ");
    
    var choice = Console.ReadLine();
    return choice ?? "4";
}

static TspProblem? LoadProblemFromFile()
{
    Console.Write("Enter the path to the JSON file: ");
    var path = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(path))
    {
        Console.WriteLine("Invalid path.");
        return null;
    }
    
    try
    {
        var problem = TspProblemLoader.LoadFromFile(path);
        Console.WriteLine($"Successfully loaded problem with {problem.Points.Count} points.");
        return problem;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error loading file: {ex.Message}");
        return null;
    }
}

static TspProblem? GenerateRandomProblem()
{
    Console.Write("Enter number of points to generate: ");
    var input = Console.ReadLine();
    
    if (!int.TryParse(input, out int numberOfPoints) || numberOfPoints < 2)
    {
        Console.WriteLine("Invalid number. Please enter a number >= 2.");
        return null;
    }
    
    Console.Write("Enter problem name (or press Enter for default): ");
    var name = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(name))
        name = "Generated Problem";
    
    var problem = TspProblemGenerator.Generate(numberOfPoints, name);
    
    Console.Write("\nDo you want to save this problem to a file? (y/n): ");
    var save = Console.ReadLine()?.ToLower();
    
    if (save == "y" || save == "yes")
    {
        Console.Write("Enter file path to save (e.g., problem.json): ");
        var path = Console.ReadLine();
        
        if (!string.IsNullOrWhiteSpace(path))
        {
            try
            {
                TspProblemLoader.SaveToFile(problem, path);
                Console.WriteLine($"Problem saved to {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving file: {ex.Message}");
            }
        }
    }
    
    return problem;
}

static ITspAlgorithm? ChooseAlgorithm()
{
    Console.WriteLine("Choose an algorithm:");
    Console.WriteLine("1. Brute Force (exact, slow for n > 10)");
    Console.WriteLine("2. Held-Karp (exact, dynamic programming, good for n <= 20)");
    Console.WriteLine("3. MST Prim (approximation, fast for large problems)");
    Console.Write("\nYour choice: ");
    
    var choice = Console.ReadLine();
    
    return choice switch
    {
        "1" => new BruteForceAlgorithm(),
        "2" => new HeldKarpAlgorithm(),
        "3" => new MstPrimAlgorithm(),
        _ => null
    };
}

static void SolveProblem(TspProblem problem, ITspAlgorithm algorithm)
{
    var tspPoints = problem.Points
        .Select(p => TspPoint.Create(p.Id, p.X, p.Y))
        .ToList();
    
    Console.WriteLine("\nSolving...");
    var startTime = DateTime.Now;
    
    using var cts = new CancellationTokenSource();
    var solution = algorithm.Solve(tspPoints, cts.Token);
    
    var elapsed = DateTime.Now - startTime;
    
    if (solution == null)
    {
        Console.WriteLine("Solution was cancelled or failed.");
        return;
    }
    
    Console.WriteLine("\n=== SOLUTION ===");
    Console.WriteLine($"Total distance: {solution.TotalDistance:F2}");
    Console.WriteLine($"Time taken: {elapsed.TotalMilliseconds:F0} ms");
    Console.WriteLine($"\nRoute (by point ID): {string.Join(" -> ", solution.Route)}");
    
    Console.WriteLine("\nDetailed route:");
    for (int i = 0; i < solution.Route.Count; i++)
    {
        var pointId = solution.Route[i];
        var point = problem.Points.First(p => p.Id == pointId);
        Console.WriteLine($"  {i + 1}. Point {point.Id}: ({point.X:F2}, {point.Y:F2})");
    }
}
