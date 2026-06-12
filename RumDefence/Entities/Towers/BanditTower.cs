using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class BanditTower : BaseTower
{
    public BanditTower(TowerData data, Vector2 location, List<Troop> troops) : base(data, location, troops)
    {
        animation.AddLayerMatrix(
            [
                new(2, SpriteAction.Static, SpriteDirection.Right),
                new(2, SpriteAction.Static, SpriteDirection.Down),
                new(2, SpriteAction.Static, SpriteDirection.Left),
                new(2, SpriteAction.Static, SpriteDirection.Up),
            ]
        , 5);
    }

    protected override void FireProjectile(Troop target)
    {
        Projectiles.Add(new KnifeProjectile(Position, target, ProjectileSpeed, CurrentDamage));
    }
}
