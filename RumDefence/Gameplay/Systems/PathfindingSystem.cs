using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using RumDefence.Exceptions;

namespace RumDefence;

public class PathfindingSystem : IGameLoopSystem
{
    public Vector2 Destination { get; private set; }

    public Queue<Vector2> Path { get; private set; } = new();


    public PathfindingSystem(Vector2 destination)
    {
        Destination = destination;

        Path.Enqueue(destination);
    }

    public Vector2 GetNextDirection(Vector2 currentPosition)
    {
        if (Path.Count == 0)
            throw new NoPathPossibleException();

        var nextPoint = Path.Peek();
        if (Vector2.Distance(currentPosition, nextPoint) < 5f)
        {
            Path.Dequeue();
            if (Path.Count == 0)
                return Vector2.Zero;
        }

        Vector2 direction = nextPoint - currentPosition;
        if (direction != Vector2.Zero)
            direction.Normalize();
        return direction;
    }

    public void UpdatePath(Vector2 currentPosition, Grid grid, HashSet<Point> untraversableTiles = null)
    {
        int[,] map = new int[grid.Width, grid.Height];

        for (int i = 0; i < map.GetLength(0); i++)
        for (int j = 0; j < map.GetLength(1); j++)
            map[i, j] = int.MaxValue;

        var entityPosition = grid.WorldToGrid(currentPosition);
        var targetPosition = grid.WorldToGrid(Destination);

        if (null == entityPosition)
            throw new ArgumentException("Current position is out of grid bounds.");

        if (null == targetPosition)
            throw new ArgumentException("Destination is out of grid bounds.");

        map[entityPosition.Value.X, entityPosition.Value.Y] = 0;

        Queue<Point> tiles = new Queue<Point>();
        tiles.Enqueue(entityPosition.Value);

        while (tiles.Count > 0)
        {
            Point current = tiles.Dequeue();
            Point[] neighbors = new[]
            {
                new Point(current.X + 1, current.Y),
                new Point(current.X - 1, current.Y),
                new Point(current.X, current.Y + 1),
                new Point(current.X, current.Y - 1)
            };

            foreach (var next in neighbors)
            {
                if (untraversableTiles != null && untraversableTiles.Contains(next))
                    continue;

                // Check bounds
                if (next.X < 0 || next.Y < 0 || next.X >= grid.Width || next.Y >= grid.Height)
                    continue;

                if (next == targetPosition.Value)
                {
                    map[next.X, next.Y] = map[current.X, current.Y] + 1;
                    tiles.Clear();
                    break;
                }

                var cost = map[current.X, current.Y] + 1;

                if (cost < map[next.X, next.Y])
                {
                    map[next.X, next.Y] = cost;
                    tiles.Enqueue(next);
                }
            }
        }

        var path = new Stack<Vector2>();
        Point currentPoint = targetPosition.Value;

        while (currentPoint != entityPosition.Value)
        {
            path.Push(grid.GridToWorld(currentPoint));

            Point[] neighbors = new[]
            {
                new Point(currentPoint.X + 1, currentPoint.Y),
                new Point(currentPoint.X - 1, currentPoint.Y),
                new Point(currentPoint.X, currentPoint.Y + 1),
                new Point(currentPoint.X, currentPoint.Y - 1)
            };

            int minCost = int.MaxValue;
            Point nextPoint = currentPoint;

            foreach (var neighbor in neighbors)
            {
                if (neighbor.X < 0 || neighbor.Y < 0 || neighbor.X >= grid.Width || neighbor.Y >= grid.Height)
                    continue;

                if (map[neighbor.X, neighbor.Y] < minCost)
                {
                    minCost = map[neighbor.X, neighbor.Y];
                    nextPoint = neighbor;
                }
            }

            if (nextPoint == currentPoint)
            {
                Path = new Queue<Vector2>();
                return;
            }

            currentPoint = nextPoint;
        }

        if (null == untraversableTiles)
        {
            Path = new Queue<Vector2>(path);
            return;
        }
        
        var pathAsList = path.ToList();
        
        for (int i = 0; i < pathAsList.Count - 2; i++)
        {
            var current = pathAsList[i];
            var next = pathAsList[i + 2];
            var crossedTiles = grid.GetTilesOnLine(current, next);
            if (!crossedTiles.Intersect(untraversableTiles).Any())
            {
                pathAsList.RemoveAt(i + 1);
                i--;
            }
        }

        Path = new Queue<Vector2>(pathAsList);
    }
}