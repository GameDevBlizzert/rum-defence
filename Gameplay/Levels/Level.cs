using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RumDefence;

public class Level
{
    public int Id { get; set; }
    public int[,] Map { get; set; }
    public ITileTheme Theme { get; set; }
    public List<Wave> Waves { get; set; }
    public bool IsUnlocked { get; set; }
    public Point RumTile { get; private set; }

    public Level(int id, string[] mapData, ITileTheme theme, List<Wave> waves, bool unlocked = false)
    {
        Id = id;
        Theme = theme;
        Waves = waves;
        IsUnlocked = unlocked;

        Map = ParseMap(mapData);
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