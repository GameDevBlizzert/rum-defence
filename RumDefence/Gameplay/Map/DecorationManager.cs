using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RumDefence;

public static class DecorationManager
{
    public enum DecorationType
    {
        Rock,
        Tree,
        Bush
    }

    public class Decoration
    {
        public Point GridPos;
        public DecorationType Type;
        private Texture2D texture;

        public Decoration(Point pos, Texture2D tex, DecorationType type)
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

        var rocks = level.Theme.Tiles.GetRocks();
        var trees = level.Theme.Tiles.GetTrees();
        var bushes = level.Theme.Tiles.GetBushes();

        if (rocks.Count == 0 && trees.Count == 0 && bushes.Count == 0)
            return result;

        var rng = new Random(level.Id);

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (!TileRules.IsLand(level.Map[y, x]) || TileRules.IsCoast(level.Map, x, y))
                    continue;

                if (rng.NextDouble() > density)
                    continue;

                var p = new Point(x, y);

                double roll = rng.NextDouble();

                DecorationType type;
                Texture2D tex = null;

                if (roll < 0.2 && rocks.Count > 0)
                {
                    type = DecorationType.Rock;
                    tex = rocks[SafeIndex(x * 73856093 ^ y * 19349663, rocks.Count)];
                }
                else if (roll < 0.5 && trees.Count > 0)
                {
                    type = DecorationType.Tree;
                    tex = trees[SafeIndex(x * 19349663 ^ y * 83492791, trees.Count)];
                }
                else if (bushes.Count > 0)
                {
                    type = DecorationType.Bush;
                    tex = bushes[SafeIndex(x * 83492791 ^ y * 1234567, bushes.Count)];
                }
                else
                {
                    continue;
                }

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

    private static bool CanPlace(List<Decoration> existing, Point p, DecorationType newType)
    {
        bool hasBushNeighbor = false;

        foreach (var d in existing)
        {
            int dx = Math.Abs(d.GridPos.X - p.X);
            int dy = Math.Abs(d.GridPos.Y - p.Y);

            if (dx > 1 || dy > 1)
                continue;

            if (newType == DecorationType.Rock || d.Type == DecorationType.Rock)
                return false;

            if (d.Type == DecorationType.Bush)
                hasBushNeighbor = true;
        }

        if (newType == DecorationType.Tree && !hasBushNeighbor)
            return false;

        return true;
    }
}