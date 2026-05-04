using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class CannonTower : BaseTower
{
    private Action<Vector2, int> _onProjectileHit;

    private float _recoilTimer = float.MaxValue;
    private const float RecoilDuration = 0.35f;
    private const float RecoilDistance = 16f;

    public CannonTower(TowerData data, Vector2 location, List<Troop> troops) : base(data, location, troops)
    {
        BaseRange = 400f;
        RangeUpgradeFlat = 25f;
        RangeUpgradePercent = 0.05f;

        BaseFireRate = 0.5f;
        FireRateUpgradeFlat = 0f;
        FireRateUpgradePercent = 0.15f;

        BaseDamage = 40;
        DamageUpgradeFlat = 10;
        DamageUpgradePercent = 0.2f;

        ProjectileSpeed = 300f;
        AttackMode = AttackMode.Closest;
        BaseUpgradeCost = 100;

        scale *= 1.4f;
    }

    public void SetProjectileHitCallback(Action<Vector2, int> callback)
    {
        _onProjectileHit = callback;
    }

    protected override void FireProjectile(Troop target)
    {
        Projectiles.Add(new CannonProjectile(Position, target, ProjectileSpeed, CurrentDamage, _onProjectileHit));
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
            // Quadratic ease-out: snaps back quickly then eases to rest
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

        foreach (var proj in Projectiles)
            proj.Draw(spriteBatch);
    }
}
