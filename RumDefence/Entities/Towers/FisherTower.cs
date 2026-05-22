using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class FisherTower : BaseTower
{
    private readonly MusketAnimation _animation = new MusketAnimation();

    public FisherTower(TowerData data, Vector2 location, List<Troop> troops) : base(data, location, troops)
    {
        origin = new Vector2(64f, 64f);
    }

    protected override void FireProjectile(Troop target)
    {
        Projectiles.Add(new NetProjectile(Position, target, ProjectileSpeed, aoeRadius: 30f));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        _animation.DrawLayers(spriteBatch, Texture, Position, origin, scale, color, rotation, rotationOffset, layerDepth);

        foreach (var proj in Projectiles)
            proj.Draw(spriteBatch);

        DrawLevelStripes(spriteBatch);
    }
}
