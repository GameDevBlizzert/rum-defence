using System.Collections.Generic;

namespace RumDefence.Gameplay.Levels.Grass.LevelData;

public static class Level2Data
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

    public static Level Create(Theme theme, bool unlocked = false, int startingLives = 100)
    {
        return new Level(
            2,
            MapData,
            theme,
            Waves,
            unlocked,
            startingCoinBalance: 200,
            startingLives: startingLives
        );
    }

    private static readonly TroopData Regular = TroopFactory.Regular;
    private static readonly TroopData Boss = TroopFactory.Boss;

    private static List<Wave> Waves => new()
    {
        //                                                               ships  troops                    spawnDelay
        CreateWave(minSpawnTime: 10f, maxSpawnTime: 20f, (NormalShip,  2, [(Regular,   5,  75)],         2.5f)),
        CreateWave(minSpawnTime:  3f, maxSpawnTime: 8f, (NormalShip,  4, [(Regular,   6,  100)],        1.0f)),
        CreateWave(minSpawnTime: 0f, maxSpawnTime: 5f, (NormalShip,  4, [(Regular,   7,  100)],        0.4f)),
        CreateWave(minSpawnTime:  6f, maxSpawnTime: 11f, (NormalShip,  6, [(Regular, 4,  100)],        1f), (BossShip, 1, [(Boss, 2, 500)], 0.1f)),
    };

    private static string[] MapData => StandardMap.Layout2;

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
