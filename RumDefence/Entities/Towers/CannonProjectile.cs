using Microsoft.Xna.Framework;
using RumDefence;
using System;

namespace RumDefence;

public class CannonProjectile : Projectile
{
    private Action<Vector2, int> _onHit;

    public CannonProjectile(Vector2 start, Troop target, float speed, int damage, Action<Vector2, int> onHit = null)
        : base(start, target, speed, damage)
    {
        _onHit = onHit;
    }

    public override void Update(GameTime gameTime)
    {
        if (IsFinished) return;

        // Call base update to handle movement and damage
        base.Update(gameTime);

        // If we just finished (hit target), trigger explosion callback
        if (IsFinished && _onHit != null)
        {
            int explosionIndex = new Random().Next(0, 3);
            _onHit(Position, explosionIndex);
        }
    }
}
