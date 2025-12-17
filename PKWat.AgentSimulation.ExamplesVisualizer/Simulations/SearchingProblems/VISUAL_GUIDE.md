# Searching Problems Visualization - Quick Visual Guide

## What You'll See

```
???????????????????????????????????????????????????????????????????
? Iteration: 342                                                  ?
? Points Explored: 1742                                           ?
? Best Value: 98.45                                               ?
? Best Position: (49.8, 50.3)                                     ?
?                                                                 ?
?                                                                 ?
?        [Background: Blue?Purple?Red heatmap gradient]          ?
?                                                                 ?
?              . . . . .     ?    . . .                         ?
?           . . . . . . . . . . . . . . .                        ?
?         . . . . . . . . ?? . . . . . . .                      ?
?        . . . . . . . ?? . . . . . . . . .                     ?
?       . . . . . . . . . . . . . . . . . .                      ?
?      . . . . . . . ?? . . . . . . . . . .                     ?
?      . . . . . . . . . . . . . . . . . .                       ?
?       . . . . . . . . ?? . . . . . . .                        ?
?        . . . . . . . . . . . . . . .                           ?
?         . . . . . . . . . . . . . .                            ?
?           . . . . . ?? . . . . .                               ?
?              . . . . . . . .                                    ?
?                                                                 ?
?                                                                 ?
???????????????????????????????????????????????????????????????????

Legend:
? = Best Point Found (Golden Star)
?? = Agent 1 (Red)      ?? = Agent 3 (Blue)     ?? = Agent 5 (Magenta)
?? = Agent 2 (Green)    ?? = Agent 4 (Yellow)
. = Explored Point (color varies by value: blue=low, yellow=high)
Background = Heatmap showing objective function landscape
```

## Color Meanings

### Background Heatmap
- **Dark Blue**: Value ? 0-40 (very poor solutions)
- **Purple**: Value ? 40-70 (moderate solutions)  
- **Red**: Value ? 70-100 (excellent solutions)
- **Brightest Red**: Center at (50, 50) = optimal solution (value 100)

### Explored Points (dots)
- **Blue dots**: Points with low objective values
- **Purple dots**: Points with medium objective values
- **Yellow/Orange dots**: Points with high objective values
- **Density**: Shows where agents spent time searching

### Search Agents (moving circles)
- **5 colored circles**: Current agent positions
- **White outlines**: Makes agents visible on any background
- **Movement patterns**: 
  - Early: Erratic, exploring widely
  - Later: Concentrated near center (50, 50)

### Best Point (star)
- **Golden star**: Always marks the best solution found so far
- **Position changes**: Jumps when a better solution is discovered
- **Final position**: Should be near (50, 50) when optimal

## Animation Phases

### Phase 1: Initial Exploration (0-100 iterations)
```
Agents: Scattered randomly across the space
Points: Sparse, multi-colored dots
Star: Jumping frequently
Background: Visible across all areas
```

### Phase 2: Convergence (100-500 iterations)
```
Agents: Moving toward center (red zone)
Points: Concentrating in center, forming a cluster
Star: Moving less frequently, staying in center area
Background: Red center becomes more prominent
```

### Phase 3: Fine-Tuning (500-1000 iterations)
```
Agents: Tight cluster around (50, 50)
Points: Dense yellow/orange cluster at center
Star: Barely moving, value > 99
Background: Agents stay in brightest red area
```

### Phase 4: Completion
```
Message: "Optimal solution found! Best value: 99.67 at (50.12, 49.89)"
OR
Message: "Max iterations reached. Best value: XX.XX at (XX.XX, XX.XX)"
```

## Typical Progress

| Iteration | Best Value | Star Position | Agent Behavior |
|-----------|------------|---------------|----------------|
| 0         | ~85-90     | Random        | Scattered |
| 50        | ~92-95     | Moving        | Spreading |
| 100       | ~95-97     | Near center   | Converging |
| 200       | ~97-99     | Very close    | Clustered |
| 300-500   | ~99-99.5+  | Optimal       | Fine-tuning |

## Success Indicators

? **Good Run**: 
- Star reaches center within 300 iterations
- Best value > 99.5
- Agents cluster tightly around (50, 50)

?? **Slow Run**:
- Star still far from center at iteration 500
- Best value < 99
- Agents spread across multiple regions

?? **Interesting Pattern**:
- Agents occasionally jump away from center (simulated annealing)
- Star might temporarily move backward (local optimum escape)
- Multiple agents exploring different high-value regions

## Tips for Observation

1. **Watch the star**: Its movement shows search progress
2. **Watch agent trails**: Dot density shows search intensity
3. **Watch the colors**: Shift from blue?yellow shows improvement
4. **Watch agent clustering**: Convergence indicates solution found
5. **Read the info panel**: Numbers confirm visual observations

Enjoy watching the optimization in action! ??
