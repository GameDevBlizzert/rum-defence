using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace RumDefence;

public static class Level3Data
{

    private static readonly Ship.Data BossShip = new("Ships/boss_ship", 80f, 10, true, 2f, -90f);
    private static readonly Ship.Data NormalShip = new("Ships/ship_1", 80f, 10, false, 0.8f, -90f);

    public static Level Create(Theme theme)
    {
        return new Level(
            3,
            MapData,
            theme,
            Waves,
            true,
            startingCoinBalance: 200
        );
    }

    private static List<Wave> Waves => new()
    {
        CreateWave(3f, 6f, 0f, (NormalShip, 1)),
        CreateWave(2f, 4f, 0f, (NormalShip, 2)),
        CreateWave(1f, 3f, 0f, (NormalShip, 3), (BossShip, 3)),
        CreateWave(1f, 2f, 0f, (NormalShip, 8), (BossShip, 4)),
        CreateWave(0.5f, 1.5f, 0f, (NormalShip, 10), (BossShip, 5)),
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

    private static Wave CreateWave(float minSpawnTime, float maxSpawnTime, float holdingTime, params (Ship.Data data, int count)[] groups)
    {
        var list = new List<ShipGroup>();

        foreach (var (data, count) in groups)
            list.Add(new ShipGroup(data, count));

        return new Wave(list, minSpawnTime, maxSpawnTime, holdingTime);
    }
}