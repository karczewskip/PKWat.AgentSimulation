# TSP Simulations - Quick Start Guide

## What Was Created

Three complete TSP (Traveling Salesman Problem) simulations with visualizations:

1. **Brute Force Algorithm** - Checks all permutations
2. **Held-Karp Algorithm** - Dynamic programming with bitmask
3. **MST Approximation** - Using Prim's algorithm

## File Structure

### Examples Project (PKWat.AgentSimulation.Examples)

```
TspProblems/
??? Core Files
?   ??? TspPoint.cs                    # City representation
?   ??? TspSolution.cs                 # Route + distance
?   ??? TspEnvironment.cs              # Shared environment
?
??? Shared Stages
?   ??? InitializeTspSpace.cs          # Setup 2D space
?   ??? InitializeTspPoints.cs         # Generate random cities
?
??? BruteForce/
?   ??? Agents/BruteForceAgent.cs
?   ??? Stages/
?   ?   ??? InitializeBruteForceAgent.cs
?   ?   ??? CheckNextPermutation.cs
?   ??? TspBruteForceSimulationBuilder.cs
?
??? HeldKarp/
?   ??? Agents/HeldKarpAgent.cs
?   ??? Stages/
?   ?   ??? InitializeHeldKarpAgent.cs
?   ?   ??? ProcessNextHeldKarpState.cs
?   ??? TspHeldKarpSimulationBuilder.cs
?
??? Mst/
?   ??? Agents/MstAgent.cs
?   ??? Stages/
?   ?   ??? InitializeMstAgent.cs
?   ?   ??? BuildMstWithPrim.cs
?   ?   ??? AddNextMstNode.cs
?   ??? TspMstSimulationBuilder.cs
?
??? README.md                          # Detailed documentation
```

### Visualizer Project (PKWat.AgentSimulation.ExamplesVisualizer)

```
Simulations/TspProblems/
??? TspBruteForceDrawer.cs
??? TspHeldKarpDrawer.cs
??? TspMstDrawer.cs
??? TspBruteForceSimulationBuilder.cs
??? TspHeldKarpSimulationBuilder.cs
??? TspMstSimulationBuilder.cs
??? VISUAL_GUIDE.md                    # Visualization guide
```

## How to Run

### Method 1: Visualizer (Recommended)

1. **Run the Visualizer Application**
   ```
   PKWat.AgentSimulation.ExamplesVisualizer
   ```

2. **Select Algorithm from Dropdown**
   - `TspBruteForceSimulationBuilder` - Brute force approach
   - `TspHeldKarpSimulationBuilder` - Held-Karp DP algorithm
   - `TspMstSimulationBuilder` - MST approximation

3. **Click "Start Simulation"**

4. **Watch the visualization!**

### Method 2: Console Application

```csharp
// In your console app, inject the builder and run:

// Brute Force
var bruteForce = new TspBruteForceSimulationBuilder(simulationBuilder);
var simulation = bruteForce.Build(pointCount: 8);
await simulation.RunAsync();

// Held-Karp
var heldKarp = new TspHeldKarpSimulationBuilder(simulationBuilder);
var simulation = heldKarp.Build(pointCount: 8);
await simulation.RunAsync();

// MST Approximation
var mst = new TspMstSimulationBuilder(simulationBuilder);
var simulation = mst.Build(pointCount: 8);
await simulation.RunAsync();
```

## What Each Algorithm Does

### 1. Brute Force (All Permutations)

**Step Definition:** One permutation per step

**Process:**
- Generates all possible routes (permutations)
- Each step evaluates one route
- Tracks the shortest route found
- Guarantees optimal solution

**For 8 cities:** 40,320 permutations (8!)

**Speed:** Very slow, increases factorially

### 2. Held-Karp (Dynamic Programming)

**Step Definition:** One subset state per step

**Process:**
- Uses bitmask to represent city subsets
- Builds solution by increasing subset size
- Each step processes one (subset, last_city) pair
- Guarantees optimal solution

**For 8 cities:** ~16,384 states (2^8 subsets × various endpoints)

**Speed:** Slow, increases exponentially (but better than brute force)

### 3. MST Approximation (Prim's)

**Step Definition:** 
- Initialization: Build MST with Prim's
- Each step: Add one node to tour

**Process:**
- Builds Minimum Spanning Tree
- Performs DFS on MST
- Each step adds one node to visualization
- Approximates optimal (within 2×)

**For 8 cities:** 8 steps

**Speed:** Very fast, polynomial time

## Visualization Features

### Brute Force Visualization
- **Orange circles:** Cities (numbered 0-7)
- **Light blue route:** Current permutation being checked
- **Green route:** Best (shortest) route found so far
- **Info panel:** Permutations checked, best distance

### Held-Karp Visualization
- **Orange circles:** Cities
- **Blue glow:** Cities in current subset
- **Magenta highlight:** Current endpoint being processed
- **Green route:** Optimal solution (when complete)
- **Info panel:** Subset size, states processed, best distance

### MST Visualization
- **Orange circles:** Unvisited cities
- **Green circles:** Visited cities
- **Light blue route:** MST structure (DFS order)
- **Yellow route:** Current partial tour being built
- **Green route:** Final approximation
- **Dashed line:** Return to start
- **Info panel:** MST status, nodes added, distance

## Parameters

### Point Count
Default: 8 cities
- Recommended for Brute Force: 6-9
- Recommended for Held-Karp: 8-12
- Recommended for MST: Any (scales well)

### Visualization Speed
- Brute Force: 10ms between steps
- Held-Karp: 10ms between steps
- MST: 100ms between steps (slower to watch construction)

### Random Seed
All use seed 42 for reproducible results

## Performance Comparison

| Cities | Brute Force | Held-Karp | MST |
|--------|-------------|-----------|-----|
| 6 | 720 steps | ~4,000 steps | 6 steps |
| 8 | 40,320 steps | ~16,000 steps | 8 steps |
| 10 | 3,628,800 steps | ~100,000 steps | 10 steps |
| 12 | 479,001,600 steps | ~3,000,000 steps | 12 steps |

## Key Features

### Separate Agent Definitions
Each algorithm has its own agent class:
- `BruteForceAgent` - Tracks permutations
- `HeldKarpAgent` - Manages DP table
- `MstAgent` - Handles MST construction

### Separate Step Definitions
Each algorithm defines a "step" differently:
- **Brute Force:** Check one permutation
- **Held-Karp:** Process one DP state
- **MST:** Add one node to tour

### Initialization Phases
All simulations have initialization:
1. Initialize 2D space (100×100)
2. Generate random cities
3. Build distance matrix
4. Initialize agent
5. (MST only) Build MST using Prim's

### Real-time Visualization
Watch algorithms work step-by-step:
- See routes being evaluated
- Track best solution updates
- Understand algorithm behavior

## Next Steps

1. **Run MST first** - Fast and easy to understand
2. **Try Held-Karp** - See dynamic programming in action
3. **Watch Brute Force** - Appreciate why better algorithms exist
4. **Experiment** - Change city count, compare results
5. **Read docs** - Check README.md and VISUAL_GUIDE.md

## Documentation

- **README.md** - Complete algorithm explanations
- **VISUAL_GUIDE.md** - Detailed visualization guide
- **This file** - Quick start reference

## Success!

All three simulations are ready to run. The visualizations will show you how different algorithms approach the same problem with vastly different strategies and performance characteristics.

Enjoy exploring the TSP problem! ??
