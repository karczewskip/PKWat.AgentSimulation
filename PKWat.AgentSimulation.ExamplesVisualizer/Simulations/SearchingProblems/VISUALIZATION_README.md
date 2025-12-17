# Searching Problems Visualization

## Overview

This visualization provides a real-time graphical representation of the searching algorithm simulation as it explores a 2D Euclidean surface to find the optimal solution.

## Visual Components

### 1. Background Heatmap (Grid)
- **Color Coding**: Shows the objective function landscape
  - **Blue areas**: Low values (poor solutions)
  - **Red areas**: High values (good solutions)
  - **Purple/Magenta**: Medium values
- **Purpose**: Helps visualize the search space topology and understand where the optimal solution is located
- **Transparency**: 50% opacity to allow other elements to be visible

### 2. Explored Points (Small Dots)
- **Appearance**: Small 2-pixel dots scattered across the surface
- **Color**: Ranges from blue (low value) to yellow (high value) based on objective function
- **Purpose**: Shows the complete search history - every point evaluated by the algorithm
- **Count**: Displayed in the info panel

### 3. Search Agents (Colored Circles)
- **Appearance**: Medium-sized circles (6 pixels diameter) with white outlines
- **Colors**:
  - Agent 1: Red
  - Agent 2: Green
  - Agent 3: Blue
  - Agent 4: Yellow
  - Agent 5: Magenta
- **Purpose**: Shows current positions of all active search agents
- **Movement**: Updates each iteration as agents explore neighboring points

### 4. Best Point (Golden Star)
- **Appearance**: Large golden star (10 pixels) with white outline
- **Color**: Bright gold (RGB: 255, 215, 0)
- **Purpose**: Highlights the best solution found so far
- **Updates**: Repositions whenever a better solution is discovered

### 5. Information Panel (Top-Left)
- **Iteration**: Current simulation step number
- **Points Explored**: Total number of points evaluated
- **Best Value**: Objective function value of the best point
- **Best Position**: (X, Y) coordinates of the best point
- **Style**: White text with black shadow for readability

## How to Run

1. **Start the ExamplesVisualizer application**
2. **Select "SearchingSimulationBuilder" from the dropdown**
3. **Click "Start Simulation"**
4. **Watch the visualization**:
   - Agents will move around exploring the space
   - Points will accumulate showing search history
   - The golden star will jump to better solutions
   - The info panel updates in real-time

## What to Observe

### Early Stage (Iterations 0-100)
- Agents spread out randomly
- Many blue/low-value points explored
- Agents occasionally accept worse solutions (simulated annealing)
- Golden star moves frequently as better solutions are found

### Middle Stage (Iterations 100-500)
- Agents converge toward the red/high-value region (center)
- More yellow/high-value points accumulate
- Golden star moves less frequently
- Dense cluster of points forms near the optimum

### Late Stage (Iterations 500+)
- Agents cluster tightly around the optimal point (50, 50)
- Mostly yellow points in the center area
- Golden star rarely moves (near-optimal solution found)
- Fine-tuning of the solution occurs

### Completion
- Simulation stops when:
  - Best value ? 99.5 (success message)
  - OR 1000 iterations reached (timeout message)
- Final golden star position shows the discovered optimum

## Customization

To modify the visualization, edit `SearchingDrawer.cs`:

- **Change scale**: Modify `Scale` constant (default: 8)
- **Adjust colors**: Edit color values in drawing methods
- **Modify agent size**: Change `agentSize` variable
- **Alter heatmap resolution**: Adjust `gridSize` variable
- **Change info display**: Modify `DrawInfoText` method

## Performance

- **Resolution**: 800x800 pixels (100x100 space * 8 scale)
- **Update Rate**: 50ms between frames (20 FPS)
- **Drawing Method**: GDI+ with Bitmap rendering
- **Freezing**: BitmapSource is frozen for thread-safe UI updates

## Algorithm Visualization

The visualization clearly shows how the search algorithm works:

1. **Exploration**: Agents move randomly, sampling the space
2. **Exploitation**: Agents converge toward high-value regions
3. **Simulated Annealing**: Occasional jumps away from local optima (visible early on)
4. **Convergence**: Gradual clustering around the global optimum
5. **Termination**: When the golden star reaches near (50, 50) with value ? 100

This makes the abstract optimization algorithm concrete and understandable!
