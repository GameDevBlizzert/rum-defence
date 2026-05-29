using Microsoft.Xna.Framework;

namespace RumDefence;

public record TroopData(
    string SpritePath,
    int Health,
    float BaseSpeed,
    int Damage,
    int CoinValue,
    float Size,
    float InitialSpeedMultiplier,
    bool IsBoss = false,
    bool IsGhost = false,
    bool IsBomber = false
);

public static class TroopFactory
{
    public static readonly TroopData Regular = new(
        SpritePath: "Art/Pirates/pirate.grunt",
        Health: 100,
        BaseSpeed: 60f,
        Damage: 1,
        CoinValue: 10,
        Size: 20f,
        InitialSpeedMultiplier: 1f
    );

    public static readonly TroopData Boss = new(
        SpritePath: "Art/Pirates/pirate.captain",
        Health: 500,
        BaseSpeed: 60f,
        Damage: 10,
        CoinValue: 100,
        Size: 22f,
        InitialSpeedMultiplier: 0.5f,
        IsBoss: true
    );

    public static readonly TroopData Ghost = new(
        SpritePath: "Art/Pirates/pirate.ghost",
        Health: 50,
        BaseSpeed: 45f,
        Damage: 1,
        CoinValue: 15,
        Size: 20f,
        InitialSpeedMultiplier: 1f,
        IsGhost: true
    );

    public static readonly TroopData Bomber = new(
        SpritePath: "Art/Pirates/pirate.bomber",
        Health: 80,
        BaseSpeed: 70f,
        Damage: 25,
        CoinValue: 20,
        Size: 20f,
        InitialSpeedMultiplier: 1f,
        IsBomber: true
    );

    public static Troop Create(TroopData data, Vector2 start, Vector2 target)
        => data.IsGhost
            ? new GhostTroop(data, start, target)
            : data.IsBoss
                ? new BossTroop(data, start, target)
                : data.IsBomber
                    ? new BomberTroop(data, start, target)
                    : new Troop(data, start, target);
}
