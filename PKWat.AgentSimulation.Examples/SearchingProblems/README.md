# Searching Problems Simulation

This folder contains a simulation for search algorithms on a 2D Euclidean surface.

## Overview

The simulation demonstrates a simple search algorithm (hill climbing with simulated annealing) that explores a 2D space to find optimal solutions. The algorithm combines greedy local search with probabilistic acceptance of worse solutions to escape local optima.

## Components

### Environment
- **SearchingEnvironment**: Manages the search space and tracks all explored points and the best solution found.

### Agents
- **SearchAgent**: Represents a search agent that explores the space, tracking its current position and the best solution it has found.

### Stages

#### Initialization Stages
1. **InitializeSearchSpace**: Sets up the search space dimensions (default: 100x100).
2. **InitializeSearchPoints**: Creates initial random sample points on the surface to evaluate the objective function.
3. **InitializeSearchAgents**: Creates search agents at random starting positions.

#### Simulation Stages
1. **ExploreNextPoint**: Each cycle, agents explore neighboring points using:
   - Random step generation in a local neighborhood
   - Greedy acceptance if the new point is better
   - Probabilistic acceptance of worse solutions (simulated annealing) to avoid local optima
   - Temperature-based acceptance probability that decreases over time

### Termination
- **OptimalSolutionFoundCondition**: The simulation terminates when:
  - The best value found exceeds the optimal threshold (default: 99.5)
  - OR the maximum number of iterations is reached (default: 1000)

## Objective Function

The default objective function is:
```
f(x, y) = 100 - (x-50)?/25 - (y-50)?/25
```

This creates a smooth landscape with a maximum value of 100 at the point (50, 50).

## Customization

You can customize the simulation by:
- Modifying the objective function in the `CalculateObjectiveFunction` methods
- Adjusting the number of search agents (default: 5)
- Changing the step size for exploration (default: 5.0)
- Setting different termination thresholds
- Modifying the temperature schedule for simulated annealing

## Example Usage

```csharp
var builder = new SearchingSimulationBuilder(simulationBuilder);
var simulation = builder.Build(optimalThreshold: 99.5, maxIterations: 1000);
await simulation.StartAsync();
```

## Algorithm Details

The search algorithm is a hybrid approach:
1. **Hill Climbing**: Always accepts better solutions
2. **Simulated Annealing**: Accepts worse solutions with probability `exp((new - current) / temperature)`
3. **Cooling Schedule**: Temperature decreases linearly: `T = max(1, 100 - iteration)`

This combination allows the algorithm to:
- Quickly move towards better solutions (exploitation)
- Escape local optima early in the search (exploration)
- Converge to the global optimum as temperature cools
