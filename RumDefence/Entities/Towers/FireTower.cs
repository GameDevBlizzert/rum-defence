using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RumDefence;

public class FireTower : BaseTower
{
    private const float AoeRadius = 70f;

    public FireTower(TowerData data, Vector2 location, List<Troop> troops) : base(data, location, troops)
    {
        BaseRange = data.Range;
        RangeUpgradeFlat = 25f;
        RangeUpgradePercent = 0.05f;

        BaseFireRate = data.FireRate;
        FireRateUpgradeFlat = 0.1f;
        FireRateUpgradePercent = 0.15f;

        BaseDamage = data.Damage;
        DamageUpgradeFlat = 2;
        DamageUpgradePercent = 0.15f;

        BaseUpgradeCost = 85;

        ProjectileSpeed = data.ProjectileSpeed;
        AttackMode = data.AttackMode;

        scale *= 1.3f;
    }

    protected override void FireProjectile(Troop target)
    {
        AudioManager.Instance.PlaySound("shoot", maxConcurrentInstances: 4);
        Projectiles.Add(new FireProjectile(Position, target, ProjectileSpeed, CurrentDamage, AoeRadius));
    }
}
