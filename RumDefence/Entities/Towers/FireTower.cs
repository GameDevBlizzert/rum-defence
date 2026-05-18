using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RumDefence;

public class FireTower : BaseTower
{
    private const float AoeRadius = 70f;

    public FireTower(TowerData data, Vector2 location, List<Troop> troops) : base(data, location, troops)
    {
    }

    protected override void FireProjectile(Troop target)
    {
        AudioManager.Instance.PlaySound("shoot", maxConcurrentInstances: 4);
        Projectiles.Add(new FireProjectile(Position, target, ProjectileSpeed, CurrentDamage, AoeRadius));
    }
}
