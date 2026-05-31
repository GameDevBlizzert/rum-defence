using Microsoft.Xna.Framework;

namespace RumDefence;

public enum TroopType { Grunt, Boss, Ghost, Bomber }

public record TroopData(
    string SpritePath,
    int Health,
    float BaseSpeed,
    int Damage,
    int CoinValue,
    float SizeInTiles,
    int SpriteFrameSize,
    float InitialSpeedMultiplier,
    TroopType Type = TroopType.Grunt
);

public static class TroopFactory
{
    public static readonly TroopData Regular = new(
        SpritePath: "Art/Pirates/pirate.grunt",
        Health: 100,
        BaseSpeed: 60f,
        Damage: 1,
        CoinValue: 10,
        SizeInTiles: 0.5f,
        SpriteFrameSize: 16,
        InitialSpeedMultiplier: 1f
    );

    public static readonly TroopData Boss = new(
        SpritePath: "Art/Pirates/pirate.captain",
        Health: 500,
        BaseSpeed: 60f,
        Damage: 10,
        CoinValue: 100,
        SizeInTiles: 0.5f,
        SpriteFrameSize: 16,
        InitialSpeedMultiplier: 0.5f,
        Type: TroopType.Boss
    );

    public static readonly TroopData Ghost = new(
        SpritePath: "Art/Pirates/pirate.ghost",
        Health: 50,
        BaseSpeed: 45f,
        Damage: 1,
        CoinValue: 15,
        SizeInTiles: 0.5f,
        SpriteFrameSize: 16,
        InitialSpeedMultiplier: 1f,
        Type: TroopType.Ghost
    );

    public static readonly TroopData Bomber = new(
        SpritePath: "Art/Pirates/pirate.bomber",
        Health: 80,
        BaseSpeed: 70f,
        Damage: 25,
        CoinValue: 20,
        SizeInTiles: 0.5f,
        SpriteFrameSize: 16,
        InitialSpeedMultiplier: 1f,
        Type: TroopType.Bomber
    );

    public static Troop Create(TroopData data, Vector2 start, Vector2 target)
        => data.Type switch
        {
            TroopType.Ghost => new GhostTroop(data, start, target),
            TroopType.Boss => new BossTroop(data, start, target),
            TroopType.Bomber => new BomberTroop(data, start, target),
            TroopType.Grunt => new GruntTroop(data, start, target),
            _ => new GruntTroop(data, start, target),
        };
}
