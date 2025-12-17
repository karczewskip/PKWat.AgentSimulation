# Searching Problems Simulation - File Structure

This document lists all files created for the SearchingProblems simulation.

## Folder: PKWat.AgentSimulation.Examples/SearchingProblems/

### Core Components

1. **SearchPoint.cs**
   - Represents a point in 2D Euclidean space with coordinates (X, Y) and an objective function value
   - Provides distance calculation between points

2. **SearchingEnvironment.cs**
   - Manages the search space (width, height)
   - Tracks all explored points
   - Maintains the best point found
   - Provides methods to check if optimal solution is found

3. **OptimalSolutionFoundCondition.cs**
   - Defines the termination conditions for the simulation
   - Stops when optimal threshold is reached OR max iterations exceeded

4. **SearchingSimulationBuilder.cs**
   - Provides a convenient builder for creating search simulations
   - Configures default parameters and stages

### Agents

5. **Agents/SearchAgent.cs**
   - Represents a search agent exploring the space
   - Tracks current position and value
   - Maintains the best point found by this agent

### Stages

6. **Stages/InitializeSearchSpace.cs**
   - Initialization stage: Sets the search space dimensions

7. **Stages/InitializeSearchPoints.cs**
   - Initialization stage: Creates random sample points on the surface
   - Evaluates the objective function at these points

8. **Stages/InitializeSearchAgents.cs**
   - Initialization stage: Creates search agents at random starting positions

9. **Stages/ExploreNextPoint.cs**
   - Main simulation stage: Runs each cycle
   - Generates neighbor points to explore
   - Implements hybrid hill climbing + simulated annealing acceptance criteria
   - Updates best solutions found

### Examples & Documentation

10. **Examples/SearchingSimulationExample.cs**
    - Provides example code for creating and running the simulation
    - Shows basic usage, custom configurations, and monitoring

11. **README.md**
    - Comprehensive documentation of the simulation
    - Explains the algorithm, components, and customization options

## Total Files Created: 11

## Simulation Flow

1. **Initialization Phase:**
   - InitializeSearchSpace ? Sets dimensions
   - InitializeSearchPoints ? Creates sample points
   - InitializeSearchAgents ? Creates agents at random positions

2. **Simulation Loop (Each Cycle):**
   - ExploreNextPoint ? Agents explore neighboring points
   - Accept/reject based on value and simulated annealing
   - Update best solutions

3. **Termination:**
   - OptimalSolutionFoundCondition checks if:
     - Best value ? threshold (success)
     - OR iterations ? max (timeout)

## Key Features

- **Euclidean 2D search space**: Points have (X, Y) coordinates
- **Objective function**: Maximization problem with customizable landscape
- **Hybrid search algorithm**: Hill climbing with simulated annealing
- **Multiple agents**: 5 agents search in parallel (configurable)
- **Configurable parameters**: Space size, thresholds, iterations, random seed
- **Progress monitoring**: Callback support for tracking search progress
