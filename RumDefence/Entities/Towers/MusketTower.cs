using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class MusketTower : BaseTower
{
    private readonly MusketAnimation _animation = new MusketAnimation();

    public MusketTower(TowerData data, Vector2 location, List<Troop> troops) : base(data, location, troops)
    {
        origin = new Vector2(64f, 64f);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        _animation.DrawLayers(spriteBatch, Texture, Position, origin, scale, color, rotation, rotationOffset, layerDepth);

        foreach (var proj in Projectiles)
            proj.Draw(spriteBatch);

        DrawLevelStripes(spriteBatch);
    }
}
