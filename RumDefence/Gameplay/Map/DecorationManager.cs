using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RumDefence;

public static class DecorationManager
{


    public class Decoration
    {
        public Point GridPos;
        private Texture2D texture;
        public string Type;

        public Decoration(Point pos, Texture2D tex, string type)
        {
            GridPos = pos;
            texture = tex;
            Type = type;
        }

        public void Draw(SpriteBatch sb, Grid grid)
        {
            var pos = grid.GridToWorld(GridPos);

            sb.Draw(texture,
                new Rectangle(
                    (int)(pos.X - grid.TileSize / 2),
                    (int)(pos.Y - grid.TileSize / 2),
                    grid.TileSize,
                    grid.TileSize),
                Color.White);
        }
    }

    public static List<Decoration> Generate(Level level)
    {
        var result = new List<Decoration>();

        float density = level.Theme.Tiles.GetDecorationDensity();

        int w = level.Map.GetLength(1);
        int h = level.Map.GetLength(0);

        var rng = new Random(level.Id);

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (!TileRules.IsLand(level.Map[y, x]) || TileRules.IsCoast(level.Map, x, y))
                    continue;

                if (rng.NextDouble() > density)
                    continue;

                var (tex, type) = level.Theme.Tiles.GetRandomDecoration(rng, x, y);

                if (tex == null || type == null)
                    continue;

                var p = new Point(x, y);

                if (!CanPlace(result, p, type))
                    continue;

                result.Add(new Decoration(p, tex, type));
            }
        }

        return result;
    }

    private static int SafeIndex(int value, int count)
    {
        return Math.Abs(value) % count;
    }

    private static bool CanPlace(List<Decoration> existing, Point p, string newType)
    {
        foreach (var d in existing)
        {
            int dx = Math.Abs(d.GridPos.X - p.X);
            int dy = Math.Abs(d.GridPos.Y - p.Y);

            if (dx > 1 || dy > 1)
                continue;

            // rocks mogen niet naast elkaar
            if (newType == "rock" && d.Type == "rock")
                return false;

            // trees alleen bij bushes
            if (newType == "tree" && d.Type != "bush")
                return false;
        }

        return true;
    }
}