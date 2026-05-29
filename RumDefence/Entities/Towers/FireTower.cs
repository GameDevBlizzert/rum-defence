using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class FireTower : BaseTower
{
    private const float AoeRadius = 70f;

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
        AudioManager.Instance.PlaySound("shoot", maxConcurrentInstances: 4);
        Projectiles.Add(new FireProjectile(Position, target, ProjectileSpeed, CurrentDamage, AoeRadius));
    }

    // public override void Draw(SpriteBatch spriteBatch)
    // {
    //     spriteBatch.Draw(
    //         Texture,
    //         Position,
    //         null,
    //         color,
    //         rotation + rotationOffset,
    //         origin,
    //         scale,
    //         spriteEffect,
    //         layerDepth
    //     );
    //     DrawProjectiles(spriteBatch);
    //     DrawLevelStripes(spriteBatch);
    // }
}
