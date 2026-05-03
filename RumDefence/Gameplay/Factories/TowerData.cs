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
