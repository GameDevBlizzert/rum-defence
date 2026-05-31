using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence;

public class BulletProjectile : BaseProjectile
{
    private readonly Animation _animation;

    public BulletProjectile(Vector2 start, Troop target, float speed, int damage)
        : base(start, target, speed, damage)
    {
        Texture = RumGame.Instance.Content.Load<Texture2D>("Art/Projectiles/bullet");

        int frameWidth = Texture.Width / 4;
        int frameHeight = Texture.Height;
        Size = SizeSystem.Square(0.7f);
        origin = new Vector2(frameWidth / 2f, frameHeight / 2f);
        scale = SizeSystem.ToScale(frameWidth, Size);

        Vector2 dir = target.Position - start;
        rotation = MathF.Atan2(dir.Y, dir.X) - MathF.PI;

        _animation = new Animation(frameWidth, frameHeight, 0.1f);
        _animation.AddLayerMatrix([new(4, SpriteAction.Static, SpriteDirection.None)]);
        _animation.ActivateLayers([new(SpriteAction.Static, SpriteDirection.None)]);
    }

    public override void Update(GameTime gameTime)
    {
        if (IsFinished) return;
        _animation.Update(gameTime);
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var layers = _animation.GetCurrentLayers();
        foreach (var layer in layers)
            spriteBatch.Draw(Texture, Position, layer.Item2, color, rotation + rotationOffset, origin, scale, layer.Item1.Effect, layer.Item1.Depth);
    }
}
