using System;
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
        float margin = grid.TileSize * 2f;
        float x;
        float y;

        switch (Random.Shared.Next(4))
        {
            case 0:
                x = -margin;
                y = (float)(Random.Shared.NextDouble() * RumGame.VirtualHeight);
                break;
            case 1:
                x = RumGame.VirtualWidth + margin;
                y = (float)(Random.Shared.NextDouble() * RumGame.VirtualHeight);
                break;
            case 2:
                x = (float)(Random.Shared.NextDouble() * RumGame.VirtualWidth);
                y = -margin;
                break;
            default:
                x = (float)(Random.Shared.NextDouble() * RumGame.VirtualWidth);
                y = RumGame.VirtualHeight + margin;
                break;
        }

        return new Vector2(x, y);
    }
}
