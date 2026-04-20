using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RumDefence;

public class WallRenderer
{
    private readonly Grid grid;
    private readonly IWallTheme theme;
    private readonly Dictionary<Point, Wall> walls;

    public WallRenderer(Grid grid, IWallTheme theme, Dictionary<Point, Wall> walls)
    {
        this.grid = grid;
        this.theme = theme;
        this.walls = walls;
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

                // The diagonal's SE corner is shared with the right cell's SW corner
                // and the bottom cell's NE corner — fill both.
                DrawTexture(spriteBatch, theme.DiagonalFill, new Point(p.X + 1, p.Y), MathHelper.PiOver2);   // SW corner
                DrawTexture(spriteBatch, theme.DiagonalFill, new Point(p.X, p.Y + 1), -MathHelper.PiOver2);  // NE corner
            }
            else // slash (/ direction)
            {
                bool isEnd = !(ne && sw);
                texture = isEnd ? theme.DiagonalEnd : theme.Diagonal;
                rotation = MathHelper.PiOver2;
                if (isEnd && ne) rotation = MathHelper.PiOver2;
                if (isEnd && sw) rotation = -MathHelper.PiOver2;

                // The diagonal's SW corner is shared with the left cell's SE corner
                // and the bottom cell's NW corner — fill both.
                DrawTexture(spriteBatch, theme.DiagonalFill, new Point(p.X - 1, p.Y), 0f);           // SE corner
                DrawTexture(spriteBatch, theme.DiagonalFill, new Point(p.X, p.Y + 1), MathHelper.Pi); // NW corner
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

        DrawTexture(spriteBatch, texture, p, rotation);
    }

    private bool HasWall(Point p)
    {
        return walls.ContainsKey(p);
    }

    private void DrawTexture(SpriteBatch spriteBatch, Texture2D texture, Point gridPos, float rotation)
    {
        Vector2 world = grid.GridToWorld(gridPos);

        float scale = (float)grid.TileSize / texture.Width;

        spriteBatch.Draw(
            texture,
            world,
            null,
            Color.White,
            rotation,
            new Vector2(texture.Width / 2f, texture.Height / 2f),
            scale,
            SpriteEffects.None,
            0f
        );
    }
}
