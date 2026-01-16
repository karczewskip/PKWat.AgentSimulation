# TSP Benchmark - Quick Start Guide

## Overview

This benchmark automatically compares three TSP algorithms as the problem size increases. Watch as algorithms compete to solve increasingly complex problems!

## How It Works

1. **Starts with 3 cities** and increases by 1 each round
2. **Each algorithm solves 10 identical test cases** per city count
3. **Sequential execution** - algorithms run one at a time
4. **60-second time limit** per test case (configurable)
5. **Automatic stop** when only 0-1 algorithms remain within time limit

## Running the Benchmark

### Step 1: Launch
```
1. Open PKWat.AgentSimulation.sln
2. Set PKWat.AgentSimulation.ExamplesVisualizer as startup project
3. Run (F5)
```

### Step 2: Select Benchmark
```
1. In the dropdown, select: "TspBenchmarkSimulationBuilder"
2. Click "Start Simulation"
```

### Step 3: Watch Progress
The screen updates every 200ms showing:
- Which algorithm is currently running
- Elapsed time for each algorithm
- Current best distance found
- Live rankings

## What You'll See

### Phase 1: Small Problems (3-8 cities)
- All three algorithms complete quickly
- Brute Force and Held-Karp find optimal solutions
- MST+Prim is fastest but slightly longer paths

### Phase 2: Medium Problems (9-12 cities)
- Brute Force starts slowing down significantly
- Held-Karp still finds optimal solutions
- MST+Prim remains fast

### Phase 3: Large Problems (13+ cities)
- Brute Force likely exceeds time limit first
- Held-Karp continues but slows down
- MST+Prim still very fast

### Phase 4: End
- Simulation stops when ?1 algorithm remains
- Final rankings displayed

## Understanding the Display

```
???????????????????????????????????????????????
? TSP Benchmark - Points: 8, Example: 5/10   ? ? Current test
???????????????????????????????????????????????
? Algorithm Status:                           ?
? BruteForce     : Running    0.234s  Dist:45.2?
? HeldKarp       : Complete   0.156s  Dist:45.2?? Fastest
? MstPrim        : Complete   0.012s  Dist:48.5?? Approximate
???????????????????????????????????????????????
? Current Test Case:                          ?
? [visual dots showing city positions]        ?
???????????????????????????????????????????????
? Rankings:                                   ?
? Best Time:                                  ?
?   1. MstPrim          : 0.0089s avg        ? ? Gold
?   2. HeldKarp         : 0.1234s avg        ? ? Silver
?   3. BruteForce       : 0.5678s avg        ? ? Bronze
?                                             ?
? Best Distance:                              ?
?   1. HeldKarp         : 123.45 avg         ? ? Optimal
?   2. BruteForce       : 123.45 avg         ? ? Optimal
?   3. MstPrim          : 145.67 avg         ? ? ~2x optimal
???????????????????????????????????????????????
```

## Customizing the Benchmark

Edit `TspBenchmarkSimulationBuilder.cs`:

```csharp
// Longer time limit (2 minutes)
timeLimit: TimeSpan.FromSeconds(120)

// Test more cities
maxPointCount: 20

// Start with more cities
startingPointCount: 5
```

## Expected Timing

On a typical modern CPU:

| Cities | Brute Force | Held-Karp | MST+Prim |
|--------|-------------|-----------|----------|
| 5      | <0.1s      | <0.1s     | <0.01s   |
| 8      | ~4s        | ~0.5s     | <0.01s   |
| 10     | ~360s      | ~2s       | <0.01s   |
| 12     | TIMEOUT    | ~10s      | <0.01s   |
| 15     | TIMEOUT    | ~60s      | ~0.02s   |

## Tips

- **Watch the Log Window**: Shows detailed cycle information
- **Pause Point**: Around 10-11 cities is when things get interesting
- **Best Comparison**: Look at both rankings - fastest vs best quality
- **Fair Competition**: All algorithms solve identical test cases

## What Makes This Different?

Unlike the individual TSP visualizations:
- ? **Fair comparison** - same test cases for all
- ? **Automated** - no manual setup needed
- ? **Sequential** - ensures accurate timing
- ? **Time-limited** - prevents infinite waits
- ? **Comprehensive** - multiple tests per size
- ? **Ranked** - clear winners by criteria

## Interpreting Results

### If MST+Prim wins time ranking:
? Expected - it's O(n?) vs O(n!) and O(n?·2?)

### If Brute Force or Held-Karp win distance ranking:
? Expected - they find optimal solutions

### If all algorithms timeout quickly:
?? Reduce `maxPointCount` or increase `timeLimit`

### If simulation never stops:
?? MST+Prim will run forever - lower `maxPointCount`

## Troubleshooting

**Simulation doesn't start:**
- Check that the correct builder is selected
- Rebuild solution (Ctrl+Shift+B)

**Screen doesn't update:**
- Wait 200ms between updates
- Check if simulation crashed (see logs)

**All algorithms timeout at small sizes:**
- Increase `timeLimit` parameter
- Check CPU isn't throttled

**Want to test specific sizes:**
```csharp
startingPointCount: 10,  // Start at 10
maxPointCount: 12        // End at 12
```

## Next Steps

After the benchmark:
- Compare with individual visualizations to see algorithm mechanics
- Try different random seeds (change in `InitializeBenchmark.cs`)
- Modify algorithms to experiment with optimizations

Enjoy watching the algorithms compete! ??
