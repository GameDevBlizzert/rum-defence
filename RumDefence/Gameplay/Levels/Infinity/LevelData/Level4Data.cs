using System.Collections.Generic;

namespace RumDefence.Gameplay.Levels.Infinity.LevelData;

public static class Level4Data
{
    private static readonly Ship.Data BossShip = new()
    {
        Texture = "Ships/boss_ship",
        Speed = 80f,
        IsBoss = true,
        SizeMultiplier = 1f,
        RotationOffsetDegrees = -90f,
    };

    private static readonly Ship.Data NormalShip = new()
    {
        Texture = "Ships/ship_1",
        Speed = 80f,
        SizeMultiplier = 0.8f,
        RotationOffsetDegrees = -90f,
    };

    // All tuning parameters for the infinite wave scaler
    private static InfiniteWaveConfig InfiniteConfig => new()
    {
        NormalShip = NormalShip,
        BossShip = BossShip,

        // Base counts for the first generated wave
        BaseShipCount = 4,
        BaseTroopCountPerShip = 5,
        BaseTroopSpawnDelay = 0.8f,
        BaseMinSpawnTime = 6f,
        BaseMaxSpawnTime = 12f,

        // Per-wave multiplicative growth
        HealthScalePerWave = 1.15f,  // +15% HP each wave
        ShipCountScalePerWave = 1.10f,  // +10% ships each wave
        TroopCountScalePerWave = 1.10f,  // +10% troops per ship each wave
        SpawnTimeScalePerWave = 0.93f,  // -7% spawn interval each wave (faster)

        // Floors so timing never reaches zero
        MinSpawnTimeFloor = 0.5f,
        MaxSpawnTimeFloor = 2.0f,

        // Hard caps to keep late waves manageable
        MaxShipCount = 12,
        MaxTroopCountPerShip = 15,

        // Ghost wave: every 4th infinite wave also sends a ghost ship
        GhostWaveInterval = 4,
        GhostTroopCountRatio = 0.6f,

        // Bomber wave: every 3rd infinite wave adds bombers
        BomberWaveInterval = 3,
        BomberTroopCountRatio = 0.5f,

        // Boss wave: every 5th infinite wave sends a boss ship
        BossWaveInterval = 5,
        BaseBossCount = 1,
        BossCountScalePerBossWave = 1.3f,  // bosses grow per boss wave: 1, 1, 2, 2, 3...
    };

    public static Level Create(Theme theme, bool unlocked = true)
    {
        return new Level(
            4,
            MapData,
            theme,
            SeedWaves,
            unlocked,
            startingCoinBalance: 200,
            infiniteConfig: InfiniteConfig
        );
    }

    private static readonly TroopData Regular = TroopFactory.Regular;
    private static readonly TroopData Boss = TroopFactory.Boss;

    // Four intro waves identical to Level 2 — eases players in before infinite generation
    private static List<Wave> SeedWaves => new()
    {
        //                                                                ships  troops                   spawnDelay
        CreateWave(minSpawnTime: 10f, maxSpawnTime: 20f, (NormalShip, 2, [(Regular, 5,  75)],  2.5f)),
        CreateWave(minSpawnTime:  3f, maxSpawnTime:  8f, (NormalShip, 4, [(Regular, 6, 100)],  1.0f)),
        CreateWave(minSpawnTime:  0f, maxSpawnTime:  5f, (NormalShip, 4, [(Regular, 7, 100)],  0.4f)),
        CreateWave(minSpawnTime:  6f, maxSpawnTime: 11f, (NormalShip, 6, [(Regular, 4, 100)],  1.0f),
                                                         (BossShip,   1, [(Boss,    2, 500)],  0.1f)),
    };

    private static string[] MapData => new[]
    {
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 1 1 0 0 0 1 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 1 1 0 0 0 1 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 1 1 0 0 0 0 0 0 0 1 1 1 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 1 1 0 0 0 0 0 0 0 1 1 1 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 1 1 1 1 1 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 1 1 1 1 1 1 1 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 1 1 1 1 1 1 # 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 1 1 1 1 1 1 1 1 1 1 0 1 1 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 1 1 1 1 1 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 1 1 0 0 0 0 0 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 1 1 0 0 0 0 0 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 1 1 0 0 0 0 0 0 1 1 1 1 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0"
    };

    private static Wave CreateWave(float minSpawnTime, float maxSpawnTime, params (Ship.Data data, int count, (TroopData troop, int n, int hp)[] troops, float spawnDelay)[] groups)
    {
        var list = new List<ShipGroup>();

        foreach (var (data, count, troops, spawnDelay) in groups)
        {
            var troopGroups = new List<TroopGroup>();
            foreach (var (troop, n, hp) in troops)
                troopGroups.Add(new TroopGroup(troop with { Health = hp }, n));
            list.Add(new ShipGroup(data, count, troopGroups, spawnDelay));
        }

        return new Wave(list, minSpawnTime, maxSpawnTime);
    }
}
