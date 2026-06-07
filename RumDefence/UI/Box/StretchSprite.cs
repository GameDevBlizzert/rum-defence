using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence.UI.Box;

public static class StretchSprite
{
    public static Rectangle[] GetCorners(Rectangle rect, Point size)
    {
        var TopLeftCornerSource = new Rectangle(rect.Left, rect.Top, size.X, size.Y);
        var TopRightCornerSource = new Rectangle(rect.Right - size.X, rect.Top, size.X, size.Y);
        var BottomLeftCornerSource = new Rectangle(rect.Left, rect.Bottom - size.Y, size.X, size.Y);
        var BottomRightCornerSource = new Rectangle(rect.Right - size.X, rect.Bottom - size.Y, size.X, size.Y);
        return [TopLeftCornerSource, TopRightCornerSource, BottomLeftCornerSource, BottomRightCornerSource];
    }
    public static void Draw(SpriteBatch spriteBatch, Texture2D texture, Rectangle destination, Rectangle? source = null, Color? color = null)
    {
        var _color = color ?? Color.White;
        // expects the texture to be a rectangle.
        var _source = source ?? new Rectangle(0, 0, texture.Width, texture.Height);
        // splits source rectangle into 9 tileSizes. X and Y divided by 3.
        var tileSize = (_source.Size.ToVector2() / 3).ToPoint();

        float scaleX = 1f * destination.Size.X / _source.Size.X;
        float scaleY = 1f * destination.Size.Y / _source.Size.Y;
        // var a = 4 / texture.Bounds.Size.X;

        // spriteBatch.Draw(texture, destCorners.Item1.Location.ToVector2(), sourceCorners.Item1, _color, rotation: 0f, origin: Vector2.Zero, scale: new Vector2(scaleX, scaleY), SpriteEffects.None, layerDepth: 1f);

        // dynamic draw corners, edges and center,
        Rectangle destRect, sourceRect;
        int x1, y1, x2, y2;
        // scaleX
        for (int x = 0; x < scaleX * 3; x++)
        {
            for (int y = 0; y < scaleY * 3; y++)
            {
                x1 = destination.X + tileSize.X * x;
                y1 = destination.Y + tileSize.Y * y;
                destRect = new Rectangle(x1, y1, tileSize.X, tileSize.Y);
                if (x == 0)
                    x2 = 0;
                else if (x < scaleX * 3 - 1)
                    x2 = tileSize.X;
                else
                    x2 = tileSize.X * 2;

                if (y == 0)
                    y2 = 0;
                else if (y < scaleY * 3 - 1)
                    y2 = tileSize.Y;
                else
                    y2 = tileSize.Y * 2;
                sourceRect = new Rectangle(x2, y2, tileSize.X, tileSize.Y);
                // Console.WriteLine(sourceRect.ToString());
                spriteBatch.Draw(texture, destRect, sourceRect, _color);
            }
        }
        int a;
        // Console.WriteLine();
        // spriteBatch.Draw(texture, destination, _color);
    }
}