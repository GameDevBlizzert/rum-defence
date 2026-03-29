using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RumDefence;

public static class Level3Data
{

    private static readonly Ship.Data BossShip = new( "Ships/boss_ship", 80f, 10, true, 1f, -90f);
    private static readonly Ship.Data NormalShip = new("Ships/ship_1", 80f, 10, false, 0.8f, -90f);

    public static Level Create(ITileTheme theme)
    {
        return new Level(
            3,
            MapData,
            theme,
            Waves,
            true
        );
    }

    private static List<Wave> Waves => new()
    {
        CreateWave(2f, 4f, (NormalShip, 5)),

        CreateWave(1.5f, 3f, (NormalShip, 10)),

        CreateWave(1f, 2.5f,
            (NormalShip, 10),
            (BossShip, 1)
        ),

        CreateWave(1f, 2f,
            (NormalShip, 15),
            (BossShip, 3)
        ),

        CreateWave(0.8f, 1.5f,
            (NormalShip, 20),
            (BossShip, 6)
        )
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

    private static Wave CreateWave(float min, float max, params (Ship.Data data, int count)[] groups)
    {
        var list = new List<ShipGroup>();

        foreach (var (data, count) in groups)
        {
            list.Add(new ShipGroup(data, count));
        }

        return new Wave(list, min, max);
    }
}