using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence;

public static class MiniMapRenderer
{
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

        spriteBatch.Draw(Primitives.Pixel, area, new Color(30, 144, 255));

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
                    int mapped = TileSystem.GetTile(map, level.RumTile, x, y);
                    texture = theme.Tiles.GetTexture(mapped, x, y);
                }

                if (texture == null) continue;

                float px = area.X + x * tileWidth;
                float py = area.Y + y * tileHeight;

                var rect = new Rectangle(
                    (int)px,
                    (int)py,
                    (int)Math.Ceiling(tileWidth),
                    (int)Math.Ceiling(tileHeight)
                );

                spriteBatch.Draw(texture, rect, Color.White);
            }
        }
    }

<<<<<<< tower-level

=======
    private static Texture2D GetPixel(SpriteBatch spriteBatch)
    {
        if (pixel == null)
        {
            pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }
        return pixel;
    }
>>>>>>> main
}