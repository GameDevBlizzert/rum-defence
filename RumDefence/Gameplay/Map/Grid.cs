using System.Collections.Generic;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RumDefence;

public class Grid
{
    public int[,] Tiles;
    public HashSet<Point> UntraversableTiles { get; set; } = new();

    public int Width => Tiles.GetLength(1);
    public int Height => Tiles.GetLength(0);

    public int TileSize { get; set; }
    public Vector2 Offset { get; set; }

    public Grid(int[,] map)
    {
        Tiles = map;
    }

    public Vector2 GridToWorld(Point gridPos)
    {
        return Offset + new Vector2(
            gridPos.X * TileSize + TileSize / 2f,
            gridPos.Y * TileSize + TileSize / 2f
        );
    }

    public Point? WorldToGrid(Vector2 worldPos)
    {
        Vector2 local = worldPos - Offset;

        int x = (int)(local.X / TileSize);
        int y = (int)(local.Y / TileSize);

        if (x < 0 || y < 0 || x >= Width || y >= Height)
            return null;

        return new Point(x, y);
    }

    public int GetTileCost(Point tile)
    {
        if (UntraversableTiles.Contains(tile))
            return 100_000;
        return 1;
    }


    /// <summary>
    /// Finds the tiles on a given line using Bresenham's line algorithm.
    /// </summary>
    /// <param name="start">Starting point in world</param>
    /// <param name="end">End point in world</param>
    /// <exception cref="ArgumentException">Thrown when one or both points are not in the inside the grid or one or both points are negative</exception>
    public List<Point> GetTilesOnLine(Vector2 start, Vector2 end)
    {
        var startTile = WorldToGrid(start);
        var endTile = WorldToGrid(end);

        if (null == startTile || null == endTile)
            throw new ArgumentException("Start or end position is out of grid bounds.");

        var x0 = startTile.Value.X;
        var y0 = startTile.Value.Y;

        var x1 = endTile.Value.X;
        var y1 = endTile.Value.Y;

        var dx = Math.Abs(x1 - x0);
        var dy = Math.Abs(y1 - y0);
        var sx = x0 < x1 ? 1 : -1;
        var sy = y0 < y1 ? 1 : -1;
        var err = dx - dy;

        List<Point> crossedTiles = [];

        while (true)
        {
            crossedTiles.Add(new Point(x0, y0));

            if (x0 == x1 && y0 == y1)
                break;

            var e2 = 2 * err;

            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 >= dx) continue;

            err += dx;
            y0 += sy;
        }

        return crossedTiles;
    }
}