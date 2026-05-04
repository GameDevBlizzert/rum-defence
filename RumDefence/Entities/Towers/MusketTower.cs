using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class MusketTower : BaseTower
{
    private readonly MusketAnimation _animation = new MusketAnimation();

    public MusketTower(TowerData data, Vector2 location, List<Troop> troops) : base(data, location, troops)
    {
        BaseRange = 500f;
        RangeUpgradeFlat = 50f;
        RangeUpgradePercent = 0.1f;

        BaseFireRate = 3f;
        FireRateUpgradeFlat = 1f;
        FireRateUpgradePercent = 0.05f;

        BaseDamage = 10;
        DamageUpgradeFlat = 5;
        DamageUpgradePercent = 0.1f;

        BaseUpgradeCost = 75;

        ProjectileSpeed = 500f;
        AttackMode = AttackMode.First;

        // Sprite sheet is 512px wide (4 cells of 128px). Scale to one cell = one grid tile.
        scale *= 8f;

        // Rotation origin is the center of one 128×128 cell.
        origin = new Vector2(64f, 64f);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Vector2 dir = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
        bool facingHorizontal = Math.Abs(dir.X) >= Math.Abs(dir.Y);
        SpriteEffects musketEffect = facingHorizontal && dir.X > 0
            ? SpriteEffects.FlipVertically
            : SpriteEffects.None;

        // Barrel background — static, never rotates
        spriteBatch.Draw(Texture, Position, _animation.GetBarrelInnerRectangle(),
            color, 0f, origin, scale, SpriteEffects.None, layerDepth);

        // Pirate — directional frames, never rotates
        spriteBatch.Draw(Texture, Position, _animation.GetPirateRectangle(dir),
            color, 0f, origin, scale, SpriteEffects.None, layerDepth + 0.02f);

        // Barrel background — static, never rotates
        spriteBatch.Draw(Texture, Position, _animation.GetBarrelOuterRectangle(),
            color, 0f, origin, scale, SpriteEffects.None, layerDepth + 0.03f);

        // Musket — rotates toward target
        spriteBatch.Draw(Texture, Position, _animation.GetMusketRectangle(dir),
            color, rotation + rotationOffset, origin, scale, musketEffect, layerDepth + 0.04f);

        foreach (var proj in Projectiles)
            proj.Draw(spriteBatch);
    }
}
