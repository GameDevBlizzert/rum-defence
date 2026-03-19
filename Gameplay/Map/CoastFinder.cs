using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RumDefence;

public static class CoastFinder
{
    public static List<Point> GetCoastTiles(int[,] map)
    {
        var result = new List<Point>();

        int height = map.GetLength(0);
        int width = map.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int tile = map[y, x];

                if (TileHelper.IsCoast(tile))
                {
                    result.Add(new Point(x, y));
                }
            }
        }

        return result;
    }
}