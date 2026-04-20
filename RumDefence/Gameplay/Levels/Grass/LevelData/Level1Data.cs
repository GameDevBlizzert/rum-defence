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
        CreateWave(1f, 1f, 10f, (NormalShip, 2)),
         CreateWave(2f, 20f, 10f, (NormalShip, 3)),

         CreateWave(10f, 15f, 10f, (NormalShip, 10)),
        //
         CreateWave(5, 5f, 10f,
             (NormalShip, 10),
             (BossShip, 1)
         ),
        //
        // CreateWave(1f, 2f, 8f,
        //     (NormalShip, 15),
        //     (BossShip, 3)
        // ),
        //
        // CreateWave(0.8f, 1.5f, 5f,
        //     (NormalShip, 20),
        //     (BossShip, 6)
        // )
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

    private static Wave CreateWave(float min, float max, float holdTime, params (Ship.Data data, int count)[] groups)
    {
        var list = new List<ShipGroup>();

        foreach (var (data, count) in groups)
        {
            list.Add(new ShipGroup(data, count));
        }

        return new Wave(list, min, max, holdTime);
    }
}