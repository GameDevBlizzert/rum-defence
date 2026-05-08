using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RumDefence;

public static class Level1Data
{

    private static readonly Ship.Data BossShip = new("Ships/boss_ship", 90f, 20, true, 2f, -90f);
    private static readonly Ship.Data NormalShip = new("Ships/ship_1", 80f, 10, false, 0.8f, -90f);

    public static Level Create(Theme theme)
    {
        return new Level(
            1,
            MapData,
            theme,
            Waves,
            true,
            startingCoinBalance: 200
        );
    }

    private static List<Wave> Waves => new()
    {
        CreateWave(10f, 20f, 0f, (NormalShip, 2)),
        CreateWave(7f, 13f, 0f, (NormalShip, 3)),
        CreateWave(6f, 12f, 0f, (NormalShip, 5)),
        CreateWave(6f, 11f, 0f, (NormalShip, 6), (BossShip, 1)),
    };

    private static string[] MapData => new[]
    {
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 7 8 8 8 8 8 8 8 9 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 6 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 6 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 6 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 4 5 5 5 # 5 5 5 6 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 6 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 6 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 4 5 5 5 5 5 5 5 6 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 1 2 2 2 2 2 2 2 3 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
        "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0"
    };

    private static Wave CreateWave(float minSpawnTime, float maxSpawnTime, float holdingTime, params (Ship.Data data, int count)[] groups)
    {
        var list = new List<ShipGroup>();

        foreach (var (data, count) in groups)
            list.Add(new ShipGroup(data, count));

        return new Wave(list, minSpawnTime, maxSpawnTime, holdingTime);
    }
}