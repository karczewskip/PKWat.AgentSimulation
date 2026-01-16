# TSP Visualizations - Visual Guide

## Brute Force Visualization

### What You'll See

1. **Cities (Orange Circles)**
   - 8 random points on the 100x100 grid
   - White border around each
   - Numbered 0-7

2. **Current Route Being Checked (Light Blue)**
   - Thin line showing current permutation
   - Changes every step
   - Transparent to see overlap

3. **Best Route Found (Bright Green)**
   - Thick line showing shortest route discovered
   - Updates when better route is found
   - Final answer when simulation completes

4. **Info Panel**
   - Algorithm name
   - Current iteration
   - Permutations checked
   - Best distance found
   - Status (Running/Complete)

### What to Observe
- Early: Best route changes frequently
- Middle: Updates become less frequent
- Late: Rarely improves (near optimal found)
- Complete: After all 40,320 permutations checked

---

## Held-Karp Visualization

### What You'll See

1. **Cities (Orange Circles)**
   - Same as Brute Force

2. **Current Subset Being Processed (Blue Glow)**
   - Larger blue circles around cities in current bitmask
   - Shows which cities are in current subproblem
   - Grows as subset size increases

3. **Current Last Node (Magenta)**
   - Bright magenta highlight
   - Shows which city is the endpoint in current state
   - Moves as algorithm processes different states

4. **Best Route (Green)**
   - Only appears when algorithm completes
   - Shows optimal solution

5. **Info Panel**
   - Current subset size (1 to n)
   - States processed (DP table size)
   - Best distance (when complete)

### What to Observe
- **Phase 1 (Size 1):** Just starting node
- **Phase 2 (Size 2):** All pairs including start
- **Phase 3 (Size 3):** All 3-city subsets
- ...continues until all cities included
- Blue glow expands as subset size increases
- Magenta marker shows current computation

---

## MST Approximation Visualization

### What You'll See

1. **Phase 1: Building MST (Purple Edges)**
   - **Orange circles:** Points not yet added to MST
   - **Purple circles:** Points added to MST
   - **Purple lines:** MST edges connecting nodes
   - Each step adds one new edge to the MST
   - Watch Prim's algorithm grow the tree

2. **Phase 2: DFS Traversal (Yellow Route)**
   - **Purple lines:** Complete MST structure (remains visible)
   - **Purple circles:** Points in MST but not yet visited
   - **Green circles:** Points visited in DFS traversal
   - **Yellow route:** DFS path being built (in-progress)
   - Each step adds one node to the tour

3. **Phase 3: Completion (Green Route)**
   - **Green circles:** All points visited
   - **Green route:** Final TSP approximation
   - **Dashed line:** Return to start

4. **Info Panel**
   - Current phase (Building MST / DFS Traversal / Complete)
   - MST edges built (out of n-1 total)
   - DFS progress (nodes visited out of n total)
   - Best distance

### What to Observe

**MST Building Phase (Steps 1 to n):**
- Starts with node 0 (purple)
- Each step: one new purple edge appears
- New nodes turn purple as they're added to MST
- Prim's algorithm always picks the minimum-weight edge

**DFS Traversal Phase (Steps n+1 to 2n):**
- MST is complete (all purple edges visible)
- Yellow route starts growing from node 0
- Nodes turn green as they're visited
- Route follows the tree structure (DFS order)

**Completion:**
- Yellow route turns green
- Dashed line shows return to start
- Final approximation is at most 2× optimal

---

## Comparison Table

| Feature | Brute Force | Held-Karp | MST |
|---------|-------------|-----------|-----|
| **Speed** | Very Slow | Slow | Very Fast |
| **Steps** | 40,320 | ~16,384 | 8 |
| **Visual Changes** | Every step | Every step | Every step |
| **Route Quality** | Optimal | Optimal | ~2× optimal |
| **Best For Learning** | Exhaustive search | Dynamic programming | Approximation |

---

## Tips for Watching

1. **Brute Force**
   - Look at iteration counter - goes to 40,320
   - Watch how rarely the green route updates near the end
   - Perfect for understanding "trying everything"

2. **Held-Karp**
   - Watch subset size counter increment
   - See blue glow expand to include more cities
   - Magenta shows algorithm "thinking"
   - Understand how subproblems build on each other

3. **MST**
   - Fast! Don't blink
   - Watch nodes turn green one by one
   - Yellow route follows blue MST structure
   - Perfect for understanding "good enough fast"

---

## Recommended Viewing Order

1. **Start with MST** - Fast and easy to understand
2. **Then Held-Karp** - See dynamic programming in action
3. **Finally Brute Force** - Appreciate why we need better algorithms!

---

## Fun Experiments

- Run all three with same seed ? Compare final routes
- Change point count to 6 ? Much faster brute force
- Change point count to 10 ? Brute force takes forever!
- Compare final distances ? MST usually within 20% of optimal
