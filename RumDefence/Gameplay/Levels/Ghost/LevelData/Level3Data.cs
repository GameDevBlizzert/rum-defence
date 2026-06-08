using System.Collections.Generic;

namespace RumDefence.Gameplay.Levels.Ghost.LevelData;

public static class Level3Data
{
    private static readonly Ship.Data BossShip = new()
    {
        Texture = "Ships/boss_ship",
        Speed = 80f,
        IsBoss = true,
        SizeMultiplier = 2f,
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
            3,
            MapData,
            theme,
            Waves,
            unlocked,
            startingCoinBalance: 100,
            startingLives: startingLives
        );
    }

    private static readonly TroopData Regular = TroopFactory.Regular;
    private static readonly TroopData Boss = TroopFactory.Boss;
    private static readonly TroopData Ghost = TroopFactory.Ghost;

    private static List<Wave> Waves => new()
    {
        //                                                                    ships  troops                                spawnDelay
        CreateWave(minSpawnTime: 3f,   maxSpawnTime: 6f,   (NormalShip,  1, [(Regular,  5, 100)],              1.0f)),
        CreateWave(minSpawnTime: 2f,   maxSpawnTime: 4f,   (NormalShip,  2, [(Regular,  7, 100)],              1.0f)),
        CreateWave(minSpawnTime: 1f,   maxSpawnTime: 9f,   (NormalShip,  3, [(Regular,  6, 100), (Ghost, 2, 50)],1.5f), (BossShip, 1, [(Boss, 1, 500)], 0.5f)),
        CreateWave(minSpawnTime: 1f,   maxSpawnTime: 7f,   (NormalShip,  3, [(Regular, 8, 100), (Ghost, 3, 50)],0.8f), (BossShip, 1, [(Boss, 1, 500)], 0.5f)),
        CreateWave(minSpawnTime: 1f,   maxSpawnTime: 4f,   (NormalShip,  20, [(Regular, 4, 100), (Ghost, 2, 50)], 0.5f), (BossShip, 2, [(Boss, 1, 600)], 0.5f)),
        CreateWave(minSpawnTime: 0.5f, maxSpawnTime: 8f, (NormalShip, 10, [(Regular, 10, 100), (Ghost, 3, 50)], 0.3f), (BossShip, 3, [(Boss, 1, 600)], 0.3f)),
    };

    private static string[] MapData => StandardMap.Layout3;

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
