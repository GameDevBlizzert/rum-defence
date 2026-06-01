using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class CannonTower : BaseTower
{
    private const float AoeRadius = 1f;

    private float _recoilTimer = float.MaxValue;
    private const float RecoilDuration = 0.35f;
    private const float RecoilDistance = 16f;

    public CannonTower(TowerData data, Vector2 location, List<Troop> troops) : base(data, location, troops)
    {
        animation.AddLayerMatrix(new SpriteMatrixCell[,]
        {
            {
                new(1, SpriteAction.Rotation, SpriteDirection.Left),
            },
        });
    }

    protected override void FireProjectile(Troop target)
    {
        Projectiles.Add(new CannonProjectile(Position, target, ProjectileSpeed, CurrentDamage, AoeRadius));
        _recoilTimer = 0f;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _recoilTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Vector2 drawPos = Position;
        if (_recoilTimer < RecoilDuration)
        {
            float t = _recoilTimer / RecoilDuration;
            float recoilAmount = RecoilDistance * (1f - t) * (1f - t);
            Vector2 backward = new Vector2(-(float)Math.Cos(rotation), -(float)Math.Sin(rotation));
            drawPos = Position + backward * recoilAmount;
        }

        spriteBatch.Draw(
            Texture,
            drawPos,
            null,
            color,
            rotation + rotationOffset,
            origin,
            scale,
            spriteEffect,
            layerDepth
        );

        DrawProjectiles(spriteBatch);
        DrawLevelStripes(spriteBatch);
    }
}
