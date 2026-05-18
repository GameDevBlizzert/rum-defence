using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace RumDefence;

public enum TowerType { Musket, Cannon, Fisher }

public record TowerData(
    TowerType Type,
    string TexturePath,
    string Label,
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
        Label: "musket",
        OverlayTexturePath: null,
        Range: 250f,
        FireRate: 3f,
        Damage: 10,
        ProjectileSpeed: 500f,
        AttackMode: AttackMode.First,
        Cost: 75
    );

    public static readonly TowerData Fisher = new(
        Type: TowerType.Fisher,
        TexturePath: "Art/Towers/fisher",
        Label: "fisher",
        OverlayTexturePath: null,
        Range: 150f,
        FireRate: 0.5f,
        Damage: 0,
        ProjectileSpeed: 200f,
        AttackMode: AttackMode.First,
        Cost: 35
    );

    public static readonly TowerData Cannon = new(
        Type: TowerType.Cannon,
        TexturePath: "Art/Towers/cannon",
        Label: "cannon",
        OverlayTexturePath: null,
        Range: 225f,
        FireRate: 0.7f,
        Damage: 50,
        ProjectileSpeed: 300f,
        AttackMode: AttackMode.First,
        Cost: 50
    );

    public static BaseTower Create(
        TowerData data,
        Vector2 location,
        List<Troop> troops)
    {
        return data.Type switch
        {
            TowerType.Musket => new MusketTower(data, location, troops),
            TowerType.Cannon => new CannonTower(data, location, troops),
            TowerType.Fisher => new FisherTower(data, location, troops),
            _ => throw new ArgumentException($"Unknown tower type: {data.Type}")
        };
    }
}
