using Microsoft.Xna.Framework;

namespace RumDefence;

public static class TroopFactory
{
    public static readonly TroopData Regular = new(
        SpritePath: "Art/Pirates/pirates-green-sprite-sheet",
        Health: 100,
        BaseSpeed: 60f,
        Damage: 1,
        CoinValue: 1,
        Size: 10f,
        InitialSpeedMultiplier: 1f
    );

    public static readonly TroopData Boss = new(
        SpritePath: "Art/Pirates/pirates-red-sprite-sheet",
        Health: 300,
        BaseSpeed: 60f,
        Damage: 10,
        CoinValue: 1,
        Size: 12f,
        InitialSpeedMultiplier: 0.5f,
        IsBoss: true
    );

    public static Troop Create(TroopData data, Vector2 start, Vector2 target)
        => data.IsBoss
            ? new BossTroop(data, start, target)
            : new Troop(data, start, target);
}
