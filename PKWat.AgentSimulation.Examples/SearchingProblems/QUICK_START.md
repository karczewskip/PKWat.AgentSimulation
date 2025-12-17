# Quick Start Guide - Searching Problems Simulation

## What is this?

A simulation that demonstrates search algorithms finding optimal points on a 2D Euclidean surface. The algorithm combines hill climbing with simulated annealing to efficiently explore the search space and find the global optimum.

## How to Use

### Basic Usage

```csharp
// Get the simulation builder from DI
var simulationBuilder = serviceProvider.GetRequiredService<ISimulationBuilder>();

// Create the simulation
var builder = new SearchingSimulationBuilder(simulationBuilder);
var simulation = builder.Build();

// Run it
await simulation.StartAsync();

// Check results
if (simulation.Crash.IsCrash)
{
    Console.WriteLine(simulation.Crash.CrashReason);
    // Output: "Optimal solution found! Best value: 99.87 at (49.95, 50.12)"
}
```

### Custom Configuration

```csharp
var simulation = builder.Build(
    optimalThreshold: 95.0,    // Stop when value ? 95
    maxIterations: 500         // Or after 500 iterations
);
```

### With Progress Monitoring

```csharp
var simulation = simulationBuilder
    .CreateNewSimulation<SearchingEnvironment>()
    .AddInitializationStage<InitializeSearchSpace>(s => s.SetSize(100, 100))
    .AddInitializationStage<InitializeSearchPoints>()
    .AddInitializationStage<InitializeSearchAgents>()
    .AddStage<ExploreNextPoint>()
    .AddCallback(context =>
    {
        var env = context.GetSimulationEnvironment<SearchingEnvironment>();
        Console.WriteLine($"Step {context.Time.StepNo}: Best = {env.BestPoint?.Value:F2}");
    })
    .AddCrashCondition(new OptimalSolutionFoundCondition().CheckCondition)
    .Build();
```

## What Happens During Simulation?

1. **Setup (Iteration 0)**:
   - Creates 100x100 search space
   - Generates 50 random sample points
   - Places 5 search agents randomly

2. **Each Iteration**:
   - Each agent generates a neighbor point (±5 units)
   - Evaluates the objective function at new point
   - Accepts if better OR with probability based on temperature
   - Updates the best solution if improved

3. **Termination**:
   - Stops when best value ? 99.5 (optimal found)
   - OR when 1000 iterations completed

## Objective Function

Default: `f(x,y) = 100 - (x-50)?/25 - (y-50)?/25`

- Maximum value: 100 at point (50, 50)
- This creates a smooth "hill" landscape
- You can customize this in the stage files

## Algorithm Details

**Hill Climbing**: Always move to better solutions
**Simulated Annealing**: Sometimes accept worse solutions
**Temperature**: Starts at 100, cools linearly to 1
**Acceptance**: `P = exp((new - current) / temperature)`

Early iterations: High temperature ? explores widely
Late iterations: Low temperature ? converges to optimum

## Customization Points

1. **Search Space Size**: Modify `InitializeSearchSpace` stage
2. **Number of Agents**: Change in `InitializeSearchAgents` stage
3. **Number of Sample Points**: Adjust in `InitializeSearchPoints` stage
4. **Step Size**: Modify `stepSize` in `ExploreNextPoint` stage
5. **Objective Function**: Replace `CalculateObjectiveFunction` methods
6. **Cooling Schedule**: Change temperature formula in `AcceptWorseWithProbability`
7. **Termination Criteria**: Adjust thresholds in builder

## Example Output

```
Iteration 0: Best value = 87.23 at (45.12, 52.34)
Iteration 100: Best value = 94.56 at (48.76, 51.23)
Iteration 200: Best value = 98.12 at (49.45, 50.67)
Iteration 250: Best value = 99.67 at (50.12, 49.89)
Simulation completed: Optimal solution found! Best value: 99.67 at (50.12, 49.89)
```

## See Also

- **README.md**: Detailed algorithm explanation
- **FILE_STRUCTURE.md**: Complete file listing
- **Examples/SearchingSimulationExample.cs**: Code examples
