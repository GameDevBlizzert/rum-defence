using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class MusketTower : BaseTower
{
    private readonly MusketAnimation _animation = new MusketAnimation();

    public MusketTower(Vector2 location, List<Troop> troops) : base(location, troops, "Art/Towers/musket")
    {
        Range = 500f;
        FireRate = 3f;
        Damage = 15;
        ProjectileSpeed = 500f;
        AttackMode = AttackMode.First;

        // The sprite sheet is 128×64 (2 cells of 64×64). SizeSystem.ToScale uses the
        // full texture width, so we double the scale to match one cell to one grid tile.
        scale *= 2f;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // Derive a direction vector from the current rotation so the animation can
        // decide whether to show the left/right cell or the up/down cell.
        Vector2 dir = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
        sourceRectangles = _animation.GetCurrentLayerRectangles(gameTime, dir);
    }
}
