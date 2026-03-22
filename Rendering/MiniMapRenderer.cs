using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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
                var texture = theme.GetTexture(map[y, x], x, y);
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