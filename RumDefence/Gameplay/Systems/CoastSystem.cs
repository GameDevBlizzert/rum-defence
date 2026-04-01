using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RumDefence;

public static class CoastSystem
{
    public static List<CoastTile> GetCoastTiles(int[,] map)
    {
        var result = new List<CoastTile>();

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                int tile = map[y, x];

                if (TileRules.IsCoast(tile))
                {
                    result.Add(new CoastTile(new Point(x, y), tile));
                }
            }
        }

        return result;
    }
}