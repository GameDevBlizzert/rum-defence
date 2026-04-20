# Rum Defence

## Audio credits

- [Kenney Interface Sounds](https://www.kenney.nl/assets/interface-sounds)
- [Kenney Impact Sounds](https://www.kenney.nl/assets/impact-sounds)
- [Cozy Tunes by Pizza Doggy](https://pizzadoggy.itch.io/cozy-tunes)

## Pathfinding

Each troop has a `PathfindingSystem` that computes a route from the troop's current grid position to a fixed destination (the end of the level).

**Algorithm — A\***

`UpdatePath` runs A* on the tile grid. Each tile normally costs 1 to enter. Tiles that contain an untraversable structure (e.g. a wall) are assigned a cost of 100 000, which effectively blocks that route unless no other path exists. The heuristic is Manhattan distance, keeping the search admissible and optimal.

**Path representation**

The resulting path is stored as a `Queue<Vector2>` of world-space waypoints — one per tile centre. `GetNextDirection` pops the front waypoint once the troop is within 5 pixels of it and returns a normalised direction vector the troop uses to move each frame.

**Replanning**

`UpdatePath` is called whenever the grid changes (e.g. a wall is placed or removed), so troops dynamically reroute around new obstacles.

## Debug

### Pathfinding

You can use the environment variable `SHOW_PATHFINDING` to visualize the pathfinding and the waypoints that are
determined by the pathfinding algorithm. 