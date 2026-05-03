using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
namespace RumDefence;

public class MusketTower : BaseTower
{
    private readonly MusketAnimation _animation = new MusketAnimation();

    public MusketTower(Vector2 location, List<Troop> troops) : base(location, troops, "Art/Towers/musket")
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

        // The sprite sheet is 128×64 (2 cells of 64×64). SizeSystem.ToScale uses the
        // full texture width, so we double the scale to match one cell to one grid tile.
        scale *= 2f;

        // Rotation origin must be the center of one cell (64×64), not the full texture.
        origin = new Vector2(32f, 32f);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // Derive a direction vector from the current rotation so the animation can
        // decide whether to show the left/right cell or the up/down cell.
        Vector2 dir = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
        sourceRectangles = _animation.GetCurrentLayerRectangles(gameTime, dir);

        // Cell 0 faces left; flip vertically when the tower is pointing right.
        bool facingHorizontal = Math.Abs(dir.X) >= Math.Abs(dir.Y);
        spriteEffect = facingHorizontal && dir.X > 0
            ? SpriteEffects.FlipVertically
            : SpriteEffects.None;
    }
}
