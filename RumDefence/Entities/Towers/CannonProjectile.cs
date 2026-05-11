using Microsoft.Xna.Framework;
using RumDefence;
using System;

namespace RumDefence;

public class CannonProjectile : Projectile
{
    private readonly Action<Vector2, int, int, float> _onHit;
    private readonly float _aoeRadius;

    public CannonProjectile(Vector2 start, Troop target, float speed, int damage, float aoeRadius, Action<Vector2, int, int, float> onHit = null)
        : base(start, target, speed, damage)
    {
        ApplyDirectDamage = false;
        _aoeRadius = aoeRadius;
        _onHit = onHit;
    }

    public override void Update(GameTime gameTime)
    {
        if (IsFinished) return;

        base.Update(gameTime);

        if (IsFinished && _onHit != null)
        {
            int explosionIndex = new Random().Next(0, 3);
            _onHit(Position, explosionIndex, Damage, _aoeRadius);
        }
    }
}
