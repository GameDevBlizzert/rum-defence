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
        public Point GridPos { get; private set; }
        public DecorationType Type { get; private set; }
        private Texture2D texture;

        public Decoration(Point gridPos, Texture2D texture, DecorationType type)
        {
            GridPos = gridPos;
            this.texture = texture;
            Type = type;
        }

        public void Draw(SpriteBatch spriteBatch, Grid grid)
        {
            var pos = grid.GridToWorld(GridPos);

            spriteBatch.Draw(
                texture,
                new Rectangle(
                    (int)(pos.X - grid.TileSize / 2),
                    (int)(pos.Y - grid.TileSize / 2),
                    grid.TileSize,
                    grid.TileSize
                ),
                Color.White
            );
        }
    }

    public static List<Decoration> Generate(Level level)
    {
        var result = new List<Decoration>();

        var textures = level.Theme.Tiles.GetDecorations();
        float density = level.Theme.Tiles.GetDecorationDensity();

        if (textures == null || textures.Count == 0)
            return result;

        var map = level.Map;

        int width = map.GetLength(1);
        int height = map.GetLength(0);

        var rocks = textures.GetRange(0, 6);
        var trees = textures.GetRange(6, 2);
        var bushes = textures.GetRange(8, 2);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var p = new Point(x, y);

                if (map[y, x] != TileRules.Center)
                    continue;

                int seed = (x * 73856093) ^ (y * 19349663) ^ level.Id;
                var rng = new Random(seed);

                if (rng.NextDouble() > density)
                    continue;

                double roll = rng.NextDouble();

                DecorationType type;
                Texture2D tex;

                if (roll < 0.2)
                {
                    type = DecorationType.Rock;
                    tex = rocks[rng.Next(rocks.Count)];
                }
                else if (roll < 0.6)
                {
                    type = DecorationType.Tree;
                    tex = trees[rng.Next(trees.Count)];
                }
                else
                {
                    type = DecorationType.Bush;
                    tex = bushes[rng.Next(bushes.Count)];
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
        foreach (var d in existing)
        {
            int dx = Math.Abs(d.GridPos.X - p.X);
            int dy = Math.Abs(d.GridPos.Y - p.Y);

            if (dx > 1 || dy > 1)
                continue;

            if (newType == DecorationType.Rock || d.Type == DecorationType.Rock)
                return false;
        }

        return true;
    }
}