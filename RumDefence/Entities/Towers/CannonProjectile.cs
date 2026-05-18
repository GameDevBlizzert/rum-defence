using Microsoft.Xna.Framework;
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
            GameScreen.Instance.Explosions.Add(new Explosion(Position, _aoeRadius));
            foreach (var troop in GameScreen.Instance.Troops)
            {
                if (troop.IsDead || troop.IsFinished) continue;
                distanceToCenter = Vector2.Distance(Position, troop.Position);
                if (distanceToCenter <= _aoeRadius)
                {
                    distanceFraction = distanceToCenter / _aoeRadius;
                    fractionDamage = _aoeDamageRange.Item2 - (distanceFraction * (_aoeDamageRange.Item2 - _aoeDamageRange.Item1));
                    troop.TakeDamage(Damage * fractionDamage);
                }
            }
        }
    }
}
