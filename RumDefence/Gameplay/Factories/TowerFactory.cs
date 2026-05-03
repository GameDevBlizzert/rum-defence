using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace RumDefence;

public static class TowerFactory
{
    public static readonly TowerData Musket = new(
        Type: TowerType.Musket,
        TexturePath: "Art/Towers/musket",
        OverlayTexturePath: null,
        Range: 500f,
        FireRate: 3f,
        Damage: 15,
        ProjectileSpeed: 500f,
        AttackMode: AttackMode.First,
        Cost: 75
    );

    public static readonly TowerData Cannon = new(
        Type: TowerType.Cannon,
        TexturePath: "KenneyPiratePack/PNG/Retina/Ship parts/wood (3)",
        OverlayTexturePath: "KenneyPiratePack/PNG/Retina/Ship parts/cannonLoose",
        Range: 700f,
        FireRate: 1.5f,
        Damage: 40,
        ProjectileSpeed: 300f,
        AttackMode: AttackMode.Closest,
        Cost: 85
    );

    public static BaseTower Create(
        TowerData data,
        Vector2 location,
        List<Troop> troops,
        Action<Vector2, int> onProjectileHit = null)
    {
        return data.Type switch
        {
            TowerType.Musket => new MusketTower(data, location, troops),
            TowerType.Cannon => CreateCannon(data, location, troops, onProjectileHit),
            _ => throw new ArgumentException($"Unknown tower type: {data.Type}")
        };
    }

    private static CannonTower CreateCannon(
        TowerData data,
        Vector2 location,
        List<Troop> troops,
        Action<Vector2, int> onProjectileHit)
    {
        var cannon = new CannonTower(data, location, troops);
        if (onProjectileHit != null)
            cannon.SetProjectileHitCallback(onProjectileHit);
        return cannon;
    }
}
