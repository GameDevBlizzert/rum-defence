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
    bool IsGhost = false
);

public static class TroopFactory
{
    public static readonly TroopData Regular = new(
        SpritePath: "Art/Pirates/pirates-green-sprite-sheet",
        Health: 100,
        BaseSpeed: 60f,
        Damage: 1,
        CoinValue: 10,
        Size: 10f,
        InitialSpeedMultiplier: 1f
    );

    public static readonly TroopData Boss = new(
        SpritePath: "Art/Pirates/pirates-red-sprite-sheet",
        Health: 500,
        BaseSpeed: 60f,
        Damage: 10,
        CoinValue: 100,
        Size: 12f,
        InitialSpeedMultiplier: 0.5f,
        IsBoss: true
    );

    public static readonly TroopData Ghost = new(
        SpritePath: "Art/Pirates/pirates-white-sprite-sheet",
        Health: 50,
        BaseSpeed: 45f,
        Damage: 1,
        CoinValue: 15,
        Size: 10f,
        InitialSpeedMultiplier: 1f,
        IsGhost: true
    );

    public static Troop Create(TroopData data, Vector2 start, Vector2 target)
        => data.IsGhost
            ? new GhostTroop(data, start, target)
            : data.IsBoss
                ? new BossTroop(data, start, target)
                : new Troop(data, start, target);
}
