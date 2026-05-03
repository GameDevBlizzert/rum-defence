using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class CannonTower : BaseTower
{
    private Texture2D _baseTexture;
    private Texture2D _cannonTexture;
    private Vector2 _cannonOrigin;
    private Action<Vector2, int> _onProjectileHit;

    public CannonTower(Vector2 location, List<Troop> troops) : base(location, troops, "KenneyPiratePack/PNG/Retina/Ship parts/wood (3)")
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

        _baseTexture = Texture;
        _cannonTexture = RumGame.Instance.Content.Load<Texture2D>("KenneyPiratePack/PNG/Retina/Ship parts/cannonLoose");
        _cannonOrigin = new Vector2(_cannonTexture.Width / 2f, _cannonTexture.Height / 2f);

        Size = SizeSystem.Square(0.5f);
        ApplySize();
    }

    public void SetProjectileHitCallback(Action<Vector2, int> callback)
    {
        _onProjectileHit = callback;
    }

    protected override void FireProjectile(Troop target)
    {
        Projectiles.Add(new CannonProjectile(Position, target, ProjectileSpeed, CurrentDamage, _onProjectileHit));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Draw base layer (wood)
        spriteBatch.Draw(
            _baseTexture,
            Position,
            null,
            color,
            0f,
            origin,
            scale,
            spriteEffect,
            layerDepth
        );

        // Draw rotating cannon on top
        spriteBatch.Draw(
            _cannonTexture,
            Position,
            null,
            color,
            rotation,
            _cannonOrigin,
            scale,
            spriteEffect,
            layerDepth + 0.01f
        );

        // Draw projectiles
        foreach (var proj in Projectiles)
            proj.Draw(spriteBatch);
    }
}
