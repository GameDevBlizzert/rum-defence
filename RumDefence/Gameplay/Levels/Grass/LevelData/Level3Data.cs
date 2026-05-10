using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace RumDefence;

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

    public static Level Create(Theme theme)
    {
        return new Level(
            3,
            MapData,
            theme,
            Waves,
            true,
            startingCoinBalance: 250
        );
    }

    private static readonly TroopData Regular = TroopFactory.Regular;
    private static readonly TroopData Boss = TroopFactory.Boss;

    private static List<Wave> Waves => new()
    {
        //                                                                    ships  troops                                spawnDelay
        CreateWave(minSpawnTime: 3f,   maxSpawnTime: 6f,   holdingTime: 5f, (NormalShip,  1, [(Regular,  5, 100)],              1.0f)),
        CreateWave(minSpawnTime: 2f,   maxSpawnTime: 4f,   holdingTime: 0f, (NormalShip,  2, [(Regular,  7, 100)],              1.0f)),
        CreateWave(minSpawnTime: 1f,   maxSpawnTime: 3f,   holdingTime: 0f, (NormalShip,  3, [(Regular,  8, 150)],              0.8f), (BossShip, 1, [(Boss, 1, 500)], 0.5f)),
        CreateWave(minSpawnTime: 1f,   maxSpawnTime: 2f,   holdingTime: 0f, (NormalShip,  4, [(Regular, 10, 200)],              0.6f), (BossShip, 3, [(Boss, 1, 600)], 0.5f)),
        CreateWave(minSpawnTime: 1f,   maxSpawnTime: 2f,   holdingTime: 0f, (NormalShip,  8, [(Regular, 10, 200)],              0.5f), (BossShip, 4, [(Boss, 1, 700)], 0.5f)),
        CreateWave(minSpawnTime: 0.5f, maxSpawnTime: 1.5f, holdingTime: 0f, (NormalShip, 10, [(Regular, 10, 250)],              0.3f), (BossShip, 5, [(Boss, 1, 800)], 0.3f)),
    };

    private static string[] MapData => new[]
    {
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 5",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 5",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 5",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 5",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 5",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 5",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 5",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 5",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 5",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 5",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 #",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 5",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 5",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 5",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 5",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 5",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 5",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 5 5 5 5"
    };

    private static Wave CreateWave(float minSpawnTime, float maxSpawnTime, float holdingTime, params (Ship.Data data, int count, (TroopData troop, int n, int hp)[] troops, float spawnDelay)[] groups)
    {
        var list = new List<ShipGroup>();

        foreach (var (data, count, troops, spawnDelay) in groups)
        {
            var troopGroups = new List<TroopGroup>();
            foreach (var (troop, n, hp) in troops)
                troopGroups.Add(new TroopGroup(troop with { Health = hp }, n));
            list.Add(new ShipGroup(data, count, troopGroups, spawnDelay));
        }

        return new Wave(list, minSpawnTime, maxSpawnTime, holdingTime);
    }
}