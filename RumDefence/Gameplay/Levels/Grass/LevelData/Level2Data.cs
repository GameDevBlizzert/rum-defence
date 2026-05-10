using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RumDefence;

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

    public static Level Create(Theme theme)
    {
        return new Level(
            2,
            MapData,
            theme,
            Waves,
            false,
            startingCoinBalance: 200
        );
    }

    private static readonly TroopData Regular = TroopFactory.Regular;
    private static readonly TroopData Boss = TroopFactory.Boss;

    private static List<Wave> Waves => new()
    {
        //                                                               ships  troops                    spawnDelay
        CreateWave(minSpawnTime: 4f, maxSpawnTime: 7f, holdingTime: 0f, (NormalShip, 1, [(Regular, 10, 100)], 1f)),
    };

    private static string[] MapData => new[]
    {
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 7 8 8 8 9 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 6 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 4 5 # 5 6 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 4 5 5 5 6 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 1 2 2 2 3 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0"
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