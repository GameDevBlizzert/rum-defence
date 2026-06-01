using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RumDefence;

public class GhostTroop : Troop
{
    public GhostTroop(TroopData data, Vector2 start, Vector2 target)
        : base(data, start, target)
    {
        color = Color.White * 0.5f;
    }

    protected override bool CanAttackWalls => false;

    public override void UpdatePathfinding()
    {
        NeedsPathInit = false;
        var grid = RumGame.Instance.CurrentGrid;

        var waterOnly = new HashSet<Point>();
        for (int x = 0; x < grid.Width; x++)
            for (int y = 0; y < grid.Height; y++)
                if (TileRules.IsWater(grid.Tiles[y, x]))
                    waterOnly.Add(new Point(x, y));

        pathfinding.UpdatePath(Position, grid, waterOnly);
    }
}
