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
    
}