using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RumDefence;

public class Level
{
    public int Id { get; set; }
    public int[,] Map { get; set; }
    public Theme Theme;
    public List<Wave> Waves { get; set; }
    public bool IsUnlocked { get; set; }
    public Point RumTile { get; private set; }

    public int StartingCoinBalance { get; private set; }
    public int StartingLives { get; private set; }


    public Level(int id, string[] mapData, Theme theme, List<Wave> waves, bool unlocked = false, int startingCoinBalance = 0, int startingLives = 10)
    {
        Id = id;
        Theme = theme;
        Waves = waves;
        IsUnlocked = unlocked;

        Map = ParseMap(mapData);
        StartingCoinBalance = startingCoinBalance;
        StartingLives = startingLives;
    }

    private int[,] ParseMap(string[] data)
    {
        int height = data.Length;
        var firstRow = data[0].Split(' ');
        int width = firstRow.Length;

        int[,] map = new int[height, width];

        for (int y = 0; y < height; y++)
        {
            var row = data[y].Split(' ');

            for (int x = 0; x < width; x++)
            {
                string cell = row[x];

                if (cell == "#")
                {
                    RumTile = new Point(x, y);
                    map[y, x] = TileRules.Center;
                }
                else
                {
                    map[y, x] = int.Parse(cell);
                }
            }
        }

        return map;
    }
}