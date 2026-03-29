using Microsoft.Xna.Framework;

namespace RumDefence;

public static class SpawnSystem
{
    public static Entity CreateShip(
        Level level,
        Grid grid,
        Ship.Data data,
        CoastTile coast
    )
    {
        Vector2 target = DockSystem.GetDockPosition(grid, coast);
        Vector2 start = DockSystem.GetSpawnPosition(grid, coast);

        var texture = level.Theme.Tiles.GetShip(data.Texture);

        return new Ship(start, target, data, texture);
    }
}