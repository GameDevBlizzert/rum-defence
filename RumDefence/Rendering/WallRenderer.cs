using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RumDefence;

public class WallRenderer
{
    private readonly Grid grid;
    private readonly IWallTheme theme;
    private readonly Dictionary<Point, Wall> walls;

    private double totalTime;

    public WallRenderer(Grid grid, IWallTheme theme, Dictionary<Point, Wall> walls)
    {
        this.grid = grid;
        this.theme = theme;
        this.walls = walls;
    }

    public void Update(GameTime gameTime)
    {
        totalTime = gameTime.TotalGameTime.TotalSeconds;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var wall in walls.Values)
        {
            DrawWall(spriteBatch, wall);
        }
    }

    private void DrawWall(SpriteBatch spriteBatch, Wall wall)
    {
        var p = wall.GridPos;

        bool up = HasWall(new Point(p.X, p.Y - 1));
        bool down = HasWall(new Point(p.X, p.Y + 1));
        bool left = HasWall(new Point(p.X - 1, p.Y));
        bool right = HasWall(new Point(p.X + 1, p.Y));

        bool nw = HasWall(new Point(p.X - 1, p.Y - 1));
        bool ne = HasWall(new Point(p.X + 1, p.Y - 1));
        bool se = HasWall(new Point(p.X + 1, p.Y + 1));
        bool sw = HasWall(new Point(p.X - 1, p.Y + 1));

        int cardinalCount = (up ? 1 : 0) + (down ? 1 : 0) + (left ? 1 : 0) + (right ? 1 : 0);

        Texture2D texture;
        float rotation = 0f;

        if (cardinalCount == 0 && wall.IsDiagonal)
        {
            bool backslash = nw || se; // \ direction (NW–SE)
            bool slash = ne || sw; // / direction (NE–SW)

            if (!backslash && !slash)
            {
                // Isolated — no neighbours at all
                texture = theme.Single;
            }
            else if (backslash && slash)
            {
                // Both diagonals — use single as placeholder until a diagonal-X asset exists
                texture = theme.Single;
            }
            else if (backslash)
            {
                // \ direction
                bool isEnd = !(nw && se);
                texture = isEnd ? theme.DiagonalEnd : theme.Diagonal;
                rotation = isEnd && se ? 0f : MathHelper.Pi;
                if (!isEnd) rotation = 0f;

                DrawTexture(spriteBatch, theme.DiagonalFill, new Point(p.X + 1, p.Y), MathHelper.PiOver2, Color.White);
                DrawTexture(spriteBatch, theme.DiagonalFill, new Point(p.X, p.Y + 1), -MathHelper.PiOver2, Color.White);
            }
            else // slash (/ direction)
            {
                bool isEnd = !(ne && sw);
                texture = isEnd ? theme.DiagonalEnd : theme.Diagonal;
                rotation = MathHelper.PiOver2;
                if (isEnd && ne) rotation = MathHelper.PiOver2;
                if (isEnd && sw) rotation = -MathHelper.PiOver2;

                DrawTexture(spriteBatch, theme.DiagonalFill, new Point(p.X - 1, p.Y), 0f, Color.White);
                DrawTexture(spriteBatch, theme.DiagonalFill, new Point(p.X, p.Y + 1), MathHelper.Pi, Color.White);
            }
        }
        // SINGLE (cardinal neighbours exist but this is a lone end-cap)
        else if (cardinalCount == 1)
        {
            texture = wall.IsDamaged
                ? theme.GetDamagedEnd(p.X, p.Y)
                : theme.End;

            if (down) rotation = 0f;
            if (left) rotation = MathHelper.PiOver2;
            if (up) rotation = MathHelper.Pi;
            if (right) rotation = -MathHelper.PiOver2;
        }
        // CORNER
        else if (cardinalCount == 2 && !(up && down) && !(left && right))
        {
            texture = wall.IsDamaged
                ? theme.GetDamagedCorner(p.X, p.Y)
                : theme.Corner;

            if (up && right) rotation = 0f;
            if (right && down) rotation = MathHelper.PiOver2;
            if (down && left) rotation = MathHelper.Pi;
            if (left && up) rotation = -MathHelper.PiOver2;
        }
        // STRAIGHT
        else if (cardinalCount == 2)
        {
            texture = wall.IsDamaged
                ? theme.GetDamagedWall(p.X, p.Y)
                : theme.Wall;

            if (left && right) rotation = MathHelper.PiOver2;
        }
        // T-JUNCTION
        else if (cardinalCount == 3)
        {
            texture = wall.IsDamaged
                ? theme.GetDamagedTwall(p.X, p.Y)
                : theme.Twall;

            if (!down) rotation = 0f;
            if (!left) rotation = MathHelper.PiOver2;
            if (!up) rotation = MathHelper.Pi;
            if (!right) rotation = -MathHelper.PiOver2;
        }
        // X-JUNCTION
        else
        {
            texture = wall.IsDamaged
                ? theme.GetDamagedXwall(p.X, p.Y)
                : theme.Xwall;
        }

        // Draw the base wall texture normally
        DrawTexture(spriteBatch, texture, p, rotation, Color.White);

        // Overlay the upgrade effect — same texture, same size, stays inside the wall
        if (wall.UpgradeLevel >= 1)
            DrawTexture(spriteBatch, texture, p, rotation, GetUpgradeOverlay(wall));
    }

    // Semi-transparent colour layer drawn over the same texture — stays 100% inside the wall shape.
    private Color GetUpgradeOverlay(Wall wall)
    {
        return wall.UpgradeLevel switch
        {
            1 => new Color(170, 95, 25, 80),  // bronze: warm copper at ~31% opacity
            2 => new Color(50, 85, 140, 75),  // steel: cold blue-grey at ~29% opacity
            3 => GetGoldOverlay(),
            _ => Color.Transparent
        };
    }

    private Color GetGoldOverlay()
    {
        float t = (float)(Math.Sin(totalTime * 2.5) * 0.5 + 0.5); // 0..1
        int r = 210;
        int g = (int)(145 + 40 * t);  // 145–185
        int b = 0;
        int a = (int)(80 + 35 * t);   // 80–115, pulses in intensity
        return new Color(r, g, b, a);
    }

    private bool HasWall(Point p)
    {
        return walls.ContainsKey(p);
    }

    private void DrawTexture(SpriteBatch spriteBatch, Texture2D texture, Point gridPos, float rotation, Color color, float scaleMultiplier = 1f)
    {
        Vector2 world = grid.GridToWorld(gridPos);

        float scale = (float)grid.TileSize / texture.Width * scaleMultiplier;

        spriteBatch.Draw(
            texture,
            world,
            null,
            color,
            rotation,
            new Vector2(texture.Width / 2f, texture.Height / 2f),
            scale,
            SpriteEffects.None,
            0f
        );
    }
}
