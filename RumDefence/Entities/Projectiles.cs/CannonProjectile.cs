using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence;
using System;

namespace RumDefence;

public class CannonProjectile : BaseProjectile
{
    private readonly float _aoeRadius;
    private readonly Tuple<float, float> _aoeDamageRange = new(0.2f, 0.8f);
    private readonly Animation _animation;

    public CannonProjectile(Vector2 start, Troop target, float speed, int damage, float aoeRadius)
        : base(start, target, speed, damage)
    {
        ApplyDirectDamage = false;
        _aoeRadius = aoeRadius;
        Texture = RumGame.Instance.Content.Load<Texture2D>("Art/Projectiles/cannonball");

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

    public override void Draw(SpriteBatch spriteBatch)
    {
        var layers = _animation.GetCurrentLayers();
        foreach (var layer in layers)
            spriteBatch.Draw(Texture, Position, layer.Item2, color, rotation + rotationOffset, origin, scale, layer.Item1.Effect, layer.Item1.Depth);
    }

    public override void Update(GameTime gameTime)
    {
        if (IsFinished) return;

        _animation.Update(gameTime);
        base.Update(gameTime);

        if (IsFinished)
        {
            float distanceToCenter;
            float fractionDamage;
            float distanceFraction;
            var explosion = new Explosion(Position, _aoeRadius);
            GameScreen.Instance.Explosions.Add(explosion);
            foreach (var troop in GameScreen.Instance.Troops)
            {
                if (troop.IsDead || troop.IsFinished) continue;
                distanceToCenter = Vector2.Distance(Position, troop.Position);
                if (distanceToCenter <= explosion.Size.X / 2)
                {
                    distanceFraction = distanceToCenter / explosion.Size.X / 2;
                    fractionDamage = _aoeDamageRange.Item2 - (distanceFraction * (_aoeDamageRange.Item2 - _aoeDamageRange.Item1));
                    troop.TakeDamage(Damage * fractionDamage);
                }
            }
        }
    }
}
