using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RumDefence;

public static class DockSystem
{
    private static Vector2 GetDirection(int tileType)
    {
        return tileType switch
        {
            8 => new Vector2(0, -1),
            2 => new Vector2(0, 1),
            4 => new Vector2(-1, 0),
            6 => new Vector2(1, 0),
            7 => new Vector2(-1, -1),
            9 => new Vector2(1, -1),
            1 => new Vector2(-1, 1),
            3 => new Vector2(1, 1),
            _ => Vector2.Zero
        };
    }

    public static Vector2 GetDockPosition(Grid grid, CoastTile coast)
    {
        Vector2 basePos = grid.GridToWorld(coast.GridPos);
        float offset = grid.TileSize * 0.3f;

        return basePos + GetDirection(coast.TileType) * offset;
    }

    public static Vector2 GetSpawnPosition(Grid grid)
    {
        var outerWaterTiles = new List<Point>();

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                bool isOuterTile = x == 0 || y == 0 || x == grid.Width - 1 || y == grid.Height - 1;
                if (isOuterTile && TileRules.IsWater(grid.Tiles[y, x]))
                    outerWaterTiles.Add(new Point(x, y));
            }
        }

        if (outerWaterTiles.Count == 0)
        {
            for (int x = 0; x < grid.Width; x++)
                for (int y = 0; y < grid.Height; y++)
                    if (TileRules.IsWater(grid.Tiles[y, x]))
                        outerWaterTiles.Add(new Point(x, y));
        }

        if (outerWaterTiles.Count == 0)
            return Vector2.Zero;

        var tile = outerWaterTiles[Random.Shared.Next(outerWaterTiles.Count)];
        return grid.GridToWorld(tile);
    }
}
