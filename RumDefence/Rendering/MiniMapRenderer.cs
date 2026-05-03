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
                    texture = theme.Tiles.GetTexture(MapMaskToTile(mask), x, y);
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
        if (IsLand(x, y + 1)) mask |= 8;
        if (IsLand(x - 1, y)) mask |= 4;

        return mask;
    }

    private static int MapMaskToTile(int mask)
    {
        if (mask == 15) return 5;

        if (mask == 14) return 2;
        if (mask == 13) return 6;
        if (mask == 11) return 8;
        if (mask == 7) return 4;

        if (mask == 10) return 3;
        if (mask == 12) return 1;
        if (mask == 2) return 9;
        if (mask == 4) return 7;

        if (mask == 3) return 10;
        if (mask == 6) return 11;
        if (mask == 9) return 12;
        if (mask == 12) return 13;

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