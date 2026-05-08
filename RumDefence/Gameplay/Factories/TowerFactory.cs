using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace RumDefence;

public enum TowerType { Musket, Cannon }

public record TowerData(
    TowerType Type,
    string TexturePath,
    string OverlayTexturePath,
    float Range,
    float FireRate,
    int Damage,
    float ProjectileSpeed,
    AttackMode AttackMode,
    int Cost
);

public static class TowerFactory
{
    public static readonly TowerData Musket = new(
        Type: TowerType.Musket,
        TexturePath: "Art/Towers/musket",
        OverlayTexturePath: null,
        Range: 500f,
        FireRate: 3f,
        Damage: 10,
        ProjectileSpeed: 500f,
        AttackMode: AttackMode.First,
        Cost: 25
    );

    public static readonly TowerData Cannon = new(
        Type: TowerType.Cannon,
        TexturePath: "Art/Towers/cannon",
        OverlayTexturePath: null,
        Range: 700f,
        FireRate: 0.5f,
        Damage: 50,
        ProjectileSpeed: 300f,
        AttackMode: AttackMode.Closest,
        Cost: 100
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
