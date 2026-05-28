using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace RumDefence;

public enum TowerType { Musket, Cannon, Fisher, Fire }

public record TowerData(
    TowerType Type,
    string TexturePath,
    string Label,
    string OverlayTexturePath,
    string IconTexturePath,
    float Range,
    float FireRate,
    int Damage,
    float ProjectileSpeed,
    AttackMode AttackMode,
    int Cost,
    float RangeUpgradeFlat,
    float RangeUpgradePercent,
    float FireRateUpgradeFlat,
    float FireRateUpgradePercent,
    int DamageUpgradeFlat,
    float DamageUpgradePercent,
    int UpgradeCost,
    float ScaleMultiplier
);

public static class TowerFactory
{
    public static readonly TowerData Musket = new(
        Type: TowerType.Musket,
        TexturePath: "Art/Towers/musket",
        Label: "musket",
        OverlayTexturePath: null,
        IconTexturePath: "Art/Towers/musket-icon",
        Range: 250f,
        FireRate: 3f,
        Damage: 10,
        ProjectileSpeed: 500f,
        AttackMode: AttackMode.First,
        Cost: 75,
        RangeUpgradeFlat: 50f,
        RangeUpgradePercent: 0.1f,
        FireRateUpgradeFlat: 1f,
        FireRateUpgradePercent: 0.05f,
        DamageUpgradeFlat: 5,
        DamageUpgradePercent: 0.1f,
        UpgradeCost: 75,
        ScaleMultiplier: 8f
    );

    public static readonly TowerData Fisher = new(
        Type: TowerType.Fisher,
        TexturePath: "Art/Towers/fisher",
        Label: "fisher",
        OverlayTexturePath: null,
        IconTexturePath: "Art/Towers/fisher-icon",
        Range: 150f,
        FireRate: 0.5f,
        Damage: 0,
        ProjectileSpeed: 200f,
        AttackMode: AttackMode.First,
        Cost: 35,
        RangeUpgradeFlat: 50f,
        RangeUpgradePercent: 0.1f,
        FireRateUpgradeFlat: 1f,
        FireRateUpgradePercent: 0.05f,
        DamageUpgradeFlat: 5,
        DamageUpgradePercent: 0.1f,
        UpgradeCost: 75,
        ScaleMultiplier: 8f
    );

    public static readonly TowerData Cannon = new(
        Type: TowerType.Cannon,
        TexturePath: "Art/Towers/cannon",
        Label: "cannon",
        OverlayTexturePath: null,
        IconTexturePath: "Art/Towers/cannon-icon",
        Range: 225f,
        FireRate: 0.5f,
        Damage: 40,
        ProjectileSpeed: 300f,
        AttackMode: AttackMode.First,
        Cost: 50,
        RangeUpgradeFlat: 25f,
        RangeUpgradePercent: 0.05f,
        FireRateUpgradeFlat: 0f,
        FireRateUpgradePercent: 0.15f,
        DamageUpgradeFlat: 10,
        DamageUpgradePercent: 0.2f,
        UpgradeCost: 100,
        ScaleMultiplier: 3f
    );

    public static readonly TowerData Fire = new(
        Type: TowerType.Fire,
        TexturePath: "Art/Towers/fire",
        Label: "fire",
        OverlayTexturePath: null,
        IconTexturePath: "Art/Towers/fire-icon",
        Range: 200f,
        FireRate: 1f,
        Damage: 5,
        ProjectileSpeed: 350f,
        AttackMode: AttackMode.Closest,
        Cost: 65,
        RangeUpgradeFlat: 25f,
        RangeUpgradePercent: 0.05f,
        FireRateUpgradeFlat: 0.1f,
        FireRateUpgradePercent: 0.15f,
        DamageUpgradeFlat: 2,
        DamageUpgradePercent: 0.15f,
        UpgradeCost: 85,
        ScaleMultiplier: 3f
    );

    public static readonly TowerData[] All = [Cannon, Musket, Fisher, Fire];

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
            TowerType.Fire => new FireTower(data, location, troops),
            _ => throw new ArgumentException($"Unknown tower type: {data.Type}")
        };
    }
}
