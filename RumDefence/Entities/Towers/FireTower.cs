using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class FireTower : BaseTower
{
    public FireTower(TowerData data, Vector2 location, List<Troop> troops) : base(data, location, troops)
    {
        animation.AddLayerMatrix(
            [
                new(1, SpriteAction.Rotation, SpriteDirection.Left),
                new(1, SpriteAction.Rotation, SpriteDirection.Down),
                new(1, SpriteAction.Rotation, SpriteDirection.Right),
                new(1, SpriteAction.Rotation, SpriteDirection.Up),
            ]
        );
        animation.AddLayerMatrix(
            [
                new(1, SpriteAction.Static, SpriteDirection.Left),
                new(1, SpriteAction.Static, SpriteDirection.Down),
                new(1, SpriteAction.Static, SpriteDirection.Right),
                new(1, SpriteAction.Static, SpriteDirection.Up),
            ]
        , 3);
    }

    protected override void FireProjectile(Troop target)
    {
        // AudioManager.Instance.PlaySound("shoot", maxConcurrentInstances: 4);
        var dir = new Vector2(MathF.Cos(rotation), MathF.Sin(rotation));
        GameScreen.Instance.FlameEffects.Add(new FlameEffect(Position, dir, CurrentRange, CurrentDamage));
    }
}
