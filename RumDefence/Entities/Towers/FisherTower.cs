using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class FisherTower : BaseTower
{

    public FisherTower(TowerData data, Vector2 location, List<Troop> troops) : base(data, location, troops)
    {
        // Sprite sheet columns: 0=Right, 1=Down, 2=Left, 3=Up  (4 rows)
        animation.AddLayerMatrix(
            [
                new(1, SpriteAction.Static, SpriteDirection.Right),
                new(1, SpriteAction.Static, SpriteDirection.Down),
                new(1, SpriteAction.Static, SpriteDirection.Left),
                new(1, SpriteAction.Static, SpriteDirection.Up),
            ]
        , 5);
    }

    protected override void FireProjectile(Troop target)
    {
        Projectiles.Add(new NetProjectile(Position, target, ProjectileSpeed, aoeRadius: 30f));
    }
}
