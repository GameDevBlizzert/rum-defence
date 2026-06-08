using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace RumDefence;

public enum TowerType { Musket, Cannon, Fisher, Fire, Bandit }

public record TowerData(
    TowerType Type,
    string TexturePath,
    string Label,
    string Description,
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
    int SpriteFrameSize,
    float SizeInTiles
);

public static class TowerFactory
{
    public static readonly TowerData Musket = new(
        Type: TowerType.Musket,
        TexturePath: "Art/Towers/musket",
        Label: "musket",
        Description: "A reliable all-rounder that fires steadily at the\nnearest enemy. Cheap to build and easy to upgrade -\na solid backbone for any defence.",
        OverlayTexturePath: null,
        IconTexturePath: "Art/Towers/musket-icon",
        Range: 250f,
        FireRate: 0.7f,
        Damage: 20,
        ProjectileSpeed: 400f,
        AttackMode: AttackMode.Nearest,
        Cost: 55,
        RangeUpgradeFlat: 50f,
        RangeUpgradePercent: 0.1f,
        FireRateUpgradeFlat: 1f,
        FireRateUpgradePercent: 0.05f,
        DamageUpgradeFlat: 5,
        DamageUpgradePercent: 0.1f,
        UpgradeCost: 70,
        SpriteFrameSize: 64,
        SizeInTiles: 1f
    );

    public static readonly TowerData Fisher = new(
        Type: TowerType.Fisher,
        TexturePath: "Art/Towers/fisher",
        Label: "fisher",
        Description: "Casts weighted nets that slow every enemy caught\nin the splash. Doesn't hit hard, but buys your other\ntowers more time to finish the job.",
        OverlayTexturePath: null,
        IconTexturePath: "Art/Towers/fisher-icon",
        Range: 150f,
        FireRate: 0.5f,
        Damage: 5,
        ProjectileSpeed: 200f,
        AttackMode: AttackMode.Nearest,
        Cost: 35,
        RangeUpgradeFlat: 50f,
        RangeUpgradePercent: 0.1f,
        FireRateUpgradeFlat: 1f,
        FireRateUpgradePercent: 0.05f,
        DamageUpgradeFlat: 5,
        DamageUpgradePercent: 0.1f,
        UpgradeCost: 45,
        SpriteFrameSize: 64,
        SizeInTiles: 1f
    );

    public static readonly TowerData Cannon = new(
        Type: TowerType.Cannon,
        TexturePath: "Art/Towers/cannon",
        Label: "cannon",
        Description: "Lobs heavy cannonballs that explode on impact,\ndamaging every enemy near the blast. Great against\ngroups, but it reloads slowly.",
        OverlayTexturePath: null,
        IconTexturePath: "Art/Towers/cannon-icon",
        Range: 250f,
        FireRate: 0.5f,
        Damage: 35,
        ProjectileSpeed: 300f,
        AttackMode: AttackMode.Nearest,
        Cost: 85,
        RangeUpgradeFlat: 25f,
        RangeUpgradePercent: 0.05f,
        FireRateUpgradeFlat: 0f,
        FireRateUpgradePercent: 0.15f,
        DamageUpgradeFlat: 10,
        DamageUpgradePercent: 0.2f,
        UpgradeCost: 100,
        SpriteFrameSize: 64,
        SizeInTiles: 1.4f
    );

    public static readonly TowerData Fire = new(
        Type: TowerType.Fire,
        TexturePath: "Art/Towers/fire",
        Label: "fire",
        Description: "Sprays a cone of flame that keeps burning enemies\ncaught inside it. Excellent for covering chokepoints\nwhere troops have to walk through.",
        OverlayTexturePath: null,
        IconTexturePath: "Art/Towers/fire-icon",
        Range: 150f,
        FireRate: 1.5f,
        Damage: 10,
        ProjectileSpeed: 300f,
        AttackMode: AttackMode.Nearest,
        Cost: 75,
        RangeUpgradeFlat: 25f,
        RangeUpgradePercent: 0.05f,
        FireRateUpgradeFlat: 0.1f,
        FireRateUpgradePercent: 0.15f,
        DamageUpgradeFlat: 2,
        DamageUpgradePercent: 0.15f,
        UpgradeCost: 85,
        SpriteFrameSize: 64,
        SizeInTiles: 1.4f
    );

    public static readonly TowerData Bandit = new(
        Type: TowerType.Bandit,
        TexturePath: "Art/Towers/bandit",
        Label: "bandit",
        Description: "Your cheapest tower - fires quick, light shots that\nchip away at anything nearby. Perfect for filling gaps\nearly on when coins are tight.",
        OverlayTexturePath: null,
        IconTexturePath: "Art/Towers/bandit-icon",
        Range: 150f,
        FireRate: 1f,
        Damage: 10,
        ProjectileSpeed: 160f,
        AttackMode: AttackMode.Nearest,
        Cost: 25,
        RangeUpgradeFlat: 25f,
        RangeUpgradePercent: 0.05f,
        FireRateUpgradeFlat: 0.1f,
        FireRateUpgradePercent: 0.15f,
        DamageUpgradeFlat: 2,
        DamageUpgradePercent: 0.15f,
        UpgradeCost: 35,
        SpriteFrameSize: 64,
        SizeInTiles: 1f
    );

    public static readonly TowerData[] All = [Cannon, Musket, Fisher, Fire, Bandit];

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
            TowerType.Bandit => new BanditTower(data, location, troops),
            _ => throw new ArgumentException($"Unknown tower type: {data.Type}")
        };
    }
}
