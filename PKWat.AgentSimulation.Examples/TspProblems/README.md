# TSP (Traveling Salesman Problem) Simulations

## Overview

This project contains three different implementations of algorithms solving the Traveling Salesman Problem (TSP) on a Euclidean surface. Each algorithm is implemented as a separate agent-based simulation with real-time visualization.

## Problem Description

The Traveling Salesman Problem asks: "Given a list of cities and the distances between each pair of cities, what is the shortest possible route that visits each city exactly once and returns to the origin city?"

In our implementation:
- Cities are represented as random points on a 2D Euclidean plane (100x100 grid)
- Distances are calculated using Euclidean distance formula: `sqrt((x2-x1)? + (y2-y1)?)`
- Default: 8 cities (configurable)

## Three Algorithms Implemented

### 1. Brute Force (Complete Permutation Search)
**File:** `TspProblems/BruteForce/`

**Algorithm:** Checks all possible permutations of cities to find the optimal route.

**How it works:**
- Generates all permutations of city orders
- Each step evaluates one permutation
- Calculates total distance for each route
- Keeps track of the best (shortest) route found

**Complexity:** O(n!) - Very slow for large numbers of cities
**Best for:** Small instances (? 10 cities)

**Step Definition:** One permutation checked per step

### 2. Held-Karp Algorithm (Dynamic Programming with Bitmask)
**File:** `TspProblems/HeldKarp/`

**Algorithm:** Uses dynamic programming with bitmask to efficiently solve TSP.

**How it works:**
- Uses bit manipulation to represent subsets of cities
- Builds solution incrementally by subset size
- Each step processes one state (subset + last city)
- DP table stores optimal solutions for subproblems

**Complexity:** O(n? × 2?) - Faster than brute force but still exponential
**Best for:** Medium instances (? 20 cities)

**Step Definition:** One additional subset state processed per step

### 3. MST Approximation (Prim's Algorithm)
**File:** `TspProblems/Mst/`

**Algorithm:** Uses Minimum Spanning Tree to approximate TSP solution.

**How it works:**
1. **Initialization Phase:** Calculates all pairwise distances
2. **MST Construction:** Builds MST using Prim's algorithm (done in initialization)
3. **DFS Traversal:** Performs depth-first search on MST to create tour
4. **Incremental Display:** Each step adds one node to the visualization

**Complexity:** O(n?) - Polynomial time
**Best for:** Large instances, when approximation is acceptable
**Approximation Factor:** At most 2× optimal (with triangle inequality)

**Step Definition:** 
- First: Calculate all distances
- Then: Build MST with Prim's algorithm
- Each step: Add one node from MST DFS traversal

## File Structure

```
TspProblems/
??? TspPoint.cs                          # City/point representation
??? TspSolution.cs                       # Solution (route + distance)
??? TspEnvironment.cs                    # Shared simulation environment
??? Stages/
?   ??? InitializeTspSpace.cs           # Set up 2D space
?   ??? InitializeTspPoints.cs          # Generate random cities
??? BruteForce/
?   ??? Agents/
?   ?   ??? BruteForceAgent.cs          # Agent state for brute force
?   ??? Stages/
?   ?   ??? InitializeBruteForceAgent.cs
?   ?   ??? CheckNextPermutation.cs     # Check one permutation
?   ??? TspBruteForceSimulationBuilder.cs
??? HeldKarp/
?   ??? Agents/
?   ?   ??? HeldKarpAgent.cs            # Agent state for Held-Karp
?   ??? Stages/
?   ?   ??? InitializeHeldKarpAgent.cs
?   ?   ??? ProcessNextHeldKarpState.cs # Process one DP state
?   ??? TspHeldKarpSimulationBuilder.cs
??? Mst/
    ??? Agents/
    ?   ??? MstAgent.cs                 # Agent state for MST
    ??? Stages/
    ?   ??? InitializeMstAgent.cs
    ?   ??? BuildMstWithPrim.cs         # Build MST (initialization)
    ?   ??? AddNextMstNode.cs           # Add one node per step
    ??? TspMstSimulationBuilder.cs
```

## Visualization

Each algorithm has its own visualizer in `ExamplesVisualizer/Simulations/TspProblems/`:

### BruteForce Visualization
- **Cities:** Orange circles with white borders
- **Current Permutation:** Light blue route (being evaluated)
- **Best Route:** Green route (shortest found so far)
- **Info:** Shows permutations checked and best distance

### HeldKarp Visualization
- **Cities:** Orange circles with white borders
- **Current Subset:** Blue glow around cities in current bitmask
- **Current Last Node:** Magenta highlight
- **Best Route:** Green route (optimal solution when complete)
- **Info:** Shows subset size and states processed

### MST Visualization
- **Unvisited Cities:** Orange circles
- **Visited Cities:** Green circles
- **MST Route:** Light blue path (DFS order on MST)
- **Current Route:** Yellow path (being built incrementally)
- **Best Route:** Green route (final approximation)
- **Return to Start:** Dashed line
- **Info:** Shows MST status and nodes added

## How to Run

### In Console Application
```csharp
// Brute Force
var bruteForceBuilder = new TspBruteForceSimulationBuilder(simulationBuilder);
var simulation = bruteForceBuilder.Build(pointCount: 8);
await simulation.RunAsync();

// Held-Karp
var heldKarpBuilder = new TspHeldKarpSimulationBuilder(simulationBuilder);
var simulation = heldKarpBuilder.Build(pointCount: 8);
await simulation.RunAsync();

// MST Approximation
var mstBuilder = new TspMstSimulationBuilder(simulationBuilder);
var simulation = mstBuilder.Build(pointCount: 8);
await simulation.RunAsync();
```

### In Visualizer Application
1. Run `PKWat.AgentSimulation.ExamplesVisualizer`
2. Select from dropdown:
   - "TspBruteForceSimulationBuilder"
   - "TspHeldKarpSimulationBuilder"
   - "TspMstSimulationBuilder"
3. Click "Start Simulation"
4. Watch the algorithm solve TSP in real-time!

## Performance Comparison

| Algorithm | Time Complexity | Space Complexity | 8 Cities | 10 Cities | 12 Cities |
|-----------|----------------|------------------|----------|-----------|-----------|
| Brute Force | O(n!) | O(n) | ~40,320 steps | ~3.6M steps | ~479M steps |
| Held-Karp | O(n? × 2?) | O(n × 2?) | ~16,384 steps | ~102,400 steps | ~3.15M steps |
| MST | O(n?) | O(n?) | ~8 steps | ~10 steps | ~12 steps |

## Customization

Modify simulation parameters in the builder:
```csharp
builder.Build(
    pointCount: 10,      // Number of cities
    maxIterations: 100000 // Maximum steps before timeout
);
```

Modify visualization speed:
```csharp
.SetWaitingTimeBetweenSteps(TimeSpan.FromMilliseconds(50))
```

## Educational Value

This simulation demonstrates:
1. **Exponential vs Polynomial Complexity:** See how brute force slows dramatically
2. **Dynamic Programming:** Watch how Held-Karp builds solutions incrementally
3. **Approximation Algorithms:** See how MST provides fast "good enough" solutions
4. **Trade-offs:** Optimality vs Speed vs Simplicity

## Notes

- Brute Force guarantees optimal solution but is impractical for > 10 cities
- Held-Karp is optimal and faster than brute force but still exponential
- MST is very fast but gives approximation (usually within 2× of optimal)
- All use same random seed (42) for reproducible results
