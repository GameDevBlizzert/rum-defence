namespace RumDefence;

public record TroopData(
    string SpritePath,
    int Health,
    float BaseSpeed,
    int Damage,
    int CoinValue,
    float Size,
    float InitialSpeedMultiplier,
    bool IsBoss = false
);
