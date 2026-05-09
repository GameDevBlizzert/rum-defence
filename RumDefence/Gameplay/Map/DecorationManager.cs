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

        if (rocks.Count == 0 || trees.Count == 0 || bushes.Count == 0)
            return result;

        var rng = new Random(level.Id);

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (level.Map[y, x] != TileRules.Center)
                    continue;

                if (rng.NextDouble() > density)
                    continue;

                var p = new Point(x, y);

                double roll = rng.NextDouble();

                DecorationType type;
                Texture2D tex;

                if (roll < 0.2)
                {
                    type = DecorationType.Rock;

                    int index = Math.Abs((x * 332371 + y * 881231 + rng.Next())) % rocks.Count;
                    tex = rocks[index];
                }
                else if (roll < 0.5)
                {
                    type = DecorationType.Tree;

                    int index = Math.Abs((x * 712371 + y * 551231 + rng.Next())) % trees.Count;
                    tex = trees[index];
                }
                else
                {
                    type = DecorationType.Bush;

                    int index = Math.Abs((x * 928371 + y * 123123 + rng.Next())) % bushes.Count;
                    tex = bushes[index];
                }

                if (!CanPlace(result, p, type))
                    continue;

                result.Add(new Decoration(p, tex, type));
            }
        }

        return result;
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