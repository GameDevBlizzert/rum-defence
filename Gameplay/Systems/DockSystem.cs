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

    public static Vector2 GetSpawnPosition(Grid grid, CoastTile coast)
    {
        Vector2 dir = GetDirection(coast.TileType);

        float margin = grid.TileSize * 2f;

        float left = grid.Offset.X;
        float right = grid.Offset.X + grid.Width * grid.TileSize;
        float top = grid.Offset.Y;
        float bottom = grid.Offset.Y + grid.Height * grid.TileSize;

        float x = 0;
        float y = 0;

        if (dir.X < 0) x = left - margin;
        else if (dir.X > 0) x = right + margin;
        else x = grid.Offset.X + (grid.Width * grid.TileSize) / 2f;

        if (dir.Y < 0) y = top - margin;
        else if (dir.Y > 0) y = bottom + margin;
        else y = grid.Offset.Y + (grid.Height * grid.TileSize) / 2f;

        return new Vector2(x, y);
    }
}