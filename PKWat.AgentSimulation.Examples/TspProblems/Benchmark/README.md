# TSP Benchmark Simulation

This benchmark simulation compares three different algorithms for solving the Traveling Salesman Problem (TSP) in a Euclidean space.

## Algorithms Compared

### 1. Brute Force (All Permutations)
- **Algorithm**: Checks all possible permutations of cities
- **Complexity**: O(n!)
- **Optimal**: Yes - guarantees finding the shortest path
- **Expected Performance**: Fast for small n, becomes impractical after ~10 cities

### 2. Held-Karp Algorithm (Dynamic Programming with Bitmask)
- **Algorithm**: Uses dynamic programming to solve TSP optimally
- **Complexity**: O(n? × 2?)
- **Optimal**: Yes - guarantees finding the shortest path
- **Expected Performance**: Better than brute force, practical up to ~20 cities

### 3. MST + Prim's Algorithm (Approximation)
- **Algorithm**: Builds Minimum Spanning Tree using Prim's algorithm, then performs DFS traversal
- **Complexity**: O(n?)
- **Optimal**: No - provides 2-approximation (at most 2× optimal)
- **Expected Performance**: Very fast, scales well to large problems

## Benchmark Configuration

### Test Cases
- **Starting Point Count**: 3 (configurable)
- **Maximum Point Count**: 15 (configurable)
- **Examples Per Round**: 10 (same 10 random test cases for all algorithms)
- **Point Generation**: Random coordinates in 100×100 space, seeded for reproducibility

### Execution Rules
1. Each cycle increases the number of points by 1
2. For each point count, all algorithms solve the same 10 test cases
3. Algorithms run **sequentially** (one at a time, not in parallel)
4. Each algorithm has a **configurable time limit** (default: 60 seconds)
5. If an algorithm exceeds the time limit, it's marked as failed for that test
6. **Stop Condition**: Simulation stops when ? 1 algorithm remains within time limit

### Rankings

Two separate rankings are maintained:

1. **Best Time Ranking**: Average execution time for completed tests
2. **Best Distance Ranking**: Average solution quality (shortest path length)

## How to Run

### In ExamplesVisualizer

1. Open the `PKWat.AgentSimulation.ExamplesVisualizer` project
2. Select `TspBenchmarkSimulationBuilder` from the dropdown
3. Click "Start Simulation"
4. Watch real-time progress and rankings

### Customizing Parameters

Modify the builder parameters in `TspBenchmarkSimulationBuilder.cs`:

```csharp
var simulation = builder.Build(
    maxPointCount: 15,           // Maximum number of cities
    startingPointCount: 3,       // Starting number of cities
    timeLimit: TimeSpan.FromSeconds(60)  // Time limit per test case
);
```

## Visualization

The benchmark displays:

- **Algorithm Status**: Current state (Running/Complete/Time Limit Exceeded)
- **Execution Time**: Real-time elapsed time for each algorithm
- **Current Distance**: Best distance found so far
- **Current Test Case**: Visual representation of city positions
- **Time Ranking**: Ordered by average execution time
- **Distance Ranking**: Ordered by average solution quality

## Expected Results

Based on algorithmic complexity:

| Algorithm | Expected to Handle | When It Fails |
|-----------|-------------------|---------------|
| Brute Force | 3-10 cities | 11+ cities (factorial explosion) |
| Held-Karp | 3-18 cities | 19+ cities (exponential memory/time) |
| MST+Prim | 3-15+ cities | Never (within reasonable limits) |

### Quality vs Performance Trade-off

- **Brute Force & Held-Karp**: Optimal solutions but slow
- **MST+Prim**: Fast but ~2× longer paths than optimal

## Implementation Details

### Key Features

- ? All algorithms solve the same test cases (fair comparison)
- ? Sequential execution (no parallel interference)
- ? Time limit enforcement (60 seconds default, configurable)
- ? Automatic stop when most algorithms exceed time limit
- ? Real-time progress visualization
- ? Detailed rankings by time and quality

### Architecture

- **Environment**: `TspBenchmarkEnvironment` - manages test cases and current state
- **Agents**: `TspBenchmarkAgent` - one per algorithm, tracks results
- **Stages**:
  - `InitializeBenchmark` - generates test cases
  - `RunBruteForce` - executes brute force algorithm
  - `RunHeldKarp` - executes Held-Karp algorithm
  - `RunMstPrim` - executes MST+Prim algorithm
  - `PrepareNextTestCase` - moves to next test when all complete
- **Crash Condition**: Stops when ?1 algorithm active

## Notes

- All algorithms are guaranteed to visit each city exactly once
- Distance calculation uses Euclidean distance: ?[(x?-x?)? + (y?-y?)?]
- Random seed (42) ensures reproducible test cases
- Results are averaged across all completed tests for each point count
