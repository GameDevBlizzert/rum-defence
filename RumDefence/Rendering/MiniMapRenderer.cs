using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public static class MiniMapRenderer
{
    private static Texture2D pixel;

    public static void Draw(
        SpriteBatch spriteBatch,
        Level level,
        Rectangle area,
        bool isUnlocked
    )
    {
        var map = level.Map;
        var theme = level.Theme;

        int width = map.GetLength(1);
        int height = map.GetLength(0);

        spriteBatch.Draw(GetPixel(spriteBatch), area, new Color(30, 144, 255));

        float tileWidth = (float)area.Width / width;
        float tileHeight = (float)area.Height / height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Texture2D texture;

                if (map[y, x] == 0)
                {
                    texture = theme.Tiles.GetTexture(0, x, y);
                }
                else
                {
                    int mask = GetMask(map, x, y);
                    int mapped = MapMaskToTile(mask);
                    texture = theme.Tiles.GetTexture(mapped, x, y);
                }

                if (texture == null) continue;

                var rect = new Rectangle(
                    (int)(area.X + x * tileWidth),
                    (int)(area.Y + y * tileHeight),
                    (int)tileWidth + 1,
                    (int)tileHeight + 1
                );

                spriteBatch.Draw(texture, rect, Color.White);
            }
        }
    }

    private static int GetMask(int[,] map, int x, int y)
    {
        int h = map.GetLength(0);
        int w = map.GetLength(1);

        bool IsLand(int px, int py)
        {
            if (px < 0 || px >= w || py < 0 || py >= h)
                return true;

            return map[py, px] == 1;
        }

        int mask = 0;

        if (IsLand(x, y - 1)) mask |= 1;
        if (IsLand(x + 1, y)) mask |= 2;
        if (IsLand(x - 1, y)) mask |= 4;
        if (IsLand(x, y + 1)) mask |= 8;

        return mask;
    }

    private static int MapMaskToTile(int mask)
    {
        bool top = (mask & 1) != 0;
        bool right = (mask & 2) != 0;
        bool left = (mask & 4) != 0;
        bool bottom = (mask & 8) != 0;

        bool wTop = !top;
        bool wRight = !right;
        bool wLeft = !left;
        bool wBottom = !bottom;

        if (!wTop && !wRight && !wLeft && !wBottom) return 5;

        if (wTop && !wRight && !wLeft && !wBottom) return 8;
        if (!wTop && wRight && !wLeft && !wBottom) return 6;
        if (!wTop && !wRight && !wLeft && wBottom) return 2;
        if (!wTop && !wRight && wLeft && !wBottom) return 4;

        if (wTop && wLeft) return 7;
        if (wTop && wRight) return 9;
        if (wBottom && wLeft) return 1;
        if (wBottom && wRight) return 3;

        if (wTop && wLeft && wRight) return 10;
        if (wTop && wRight && wBottom) return 11;
        if (wLeft && wBottom && wRight) return 12;
        if (wTop && wLeft && wBottom) return 13;

        return 5;
    }

    private static Texture2D GetPixel(SpriteBatch spriteBatch)
    {
        if (pixel == null)
        {
            pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }
        return pixel;
    }
}