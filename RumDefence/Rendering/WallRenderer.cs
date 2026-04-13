using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RumDefence;

public class WallRenderer
{
    private Grid grid;
    private IWallTheme theme;
    private Dictionary<Point, Wall> walls;

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

        int connections = (up ? 1 : 0) + (down ? 1 : 0) + (left ? 1 : 0) + (right ? 1 : 0);

        Texture2D texture;
        float rotation = 0f;

        // END
        if (connections <= 1)
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
        else if (connections == 2 && !(up && down) && !(left && right))
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
        else if (connections == 2)
        {
            texture = wall.IsDamaged
                ? theme.GetDamagedWall(p.X, p.Y)
                : theme.Wall;

            if (left && right)
                rotation = MathHelper.PiOver2;
        }
        // T-JUNCTION
        else if (connections == 3)
        {
            texture = wall.IsDamaged
                ? theme.GetDamagedTwall(p.X, p.Y)
                : theme.Twall;

            // Sprite assumed to open toward bottom (up+left+right connected, down missing)
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