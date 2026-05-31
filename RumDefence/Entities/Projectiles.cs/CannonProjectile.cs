using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence;
using System;

namespace RumDefence;

public class CannonProjectile : BaseProjectile
{
    private readonly float _aoeRadius;
    private readonly Tuple<float, float> _aoeDamageRange = new(0.2f, 0.8f);

    public CannonProjectile(Vector2 start, Troop target, float speed, int damage, float aoeRadius)
        : base(start, target, speed, damage)
    {
        ApplyDirectDamage = false;
        _aoeRadius = aoeRadius;
        Texture = RumGame.Instance.Content.Load<Texture2D>("Art/Projectiles/cannonball");
        Size = SizeSystem.Square(0.25f);
    }

    public override void Update(GameTime gameTime)
    {
        if (IsFinished) return;

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
