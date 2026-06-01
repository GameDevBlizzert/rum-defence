using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RumDefence;

public static class SpawnSystem
{
    public static Entity CreateShip(
        Level level,
        Grid grid,
        Vector2 start,
        Ship.Data data,
        CoastTile coast,
        IReadOnlyList<TroopGroup> troops,
        float troopSpawnDelay
    )
    {
        Vector2 target = DockSystem.GetDockPosition(grid, coast);

        var texture = level.Theme.Tiles.GetShip(data.Texture);

        return new Ship(start, target, coast, data, texture, troops, troopSpawnDelay);
    }
}
