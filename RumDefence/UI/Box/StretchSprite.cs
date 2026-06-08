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
        // splits source rectangle into 9 tiles. X and Y divided by 3.
        var srcTileSize = (_source.Size.ToVector2() / 3).ToPoint();

        if (destination.Width < _source.Width && destination.Height < _source.Height)
        {
            spriteBatch.Draw(texture, destination, _source, _color);
            return;
        }

        // corners keep their native source size, clamped so they never overlap
        // when the destination is smaller than two corners along an axis.
        int cornerW = Math.Min(srcTileSize.X, destination.Width / 2);
        int cornerH = Math.Min(srcTileSize.Y, destination.Height / 2);

        int[] destXs = { destination.Left, destination.Left + cornerW, destination.Right - cornerW, destination.Right };
        int[] destYs = { destination.Top, destination.Top + cornerH, destination.Bottom - cornerH, destination.Bottom };
        int[] srcXs = { _source.Left, _source.Left + srcTileSize.X, _source.Right - srcTileSize.X, _source.Right };
        int[] srcYs = { _source.Top, _source.Top + srcTileSize.Y, _source.Bottom - srcTileSize.Y, _source.Top };
        srcYs[3] = _source.Bottom;

        for (int x = 0; x < 3; x++)
        {
            int destX = destXs[x];
            int destW = destXs[x + 1] - destXs[x];
            int srcX = srcXs[x];
            int srcW = srcXs[x + 1] - srcXs[x];

            for (int y = 0; y < 3; y++)
            {
                int destY = destYs[y];
                int destH = destYs[y + 1] - destYs[y];
                int srcY = srcYs[y];
                int srcH = srcYs[y + 1] - srcYs[y];

                if (destW <= 0 || destH <= 0)
                    continue;

                var destRect = new Rectangle(destX, destY, destW, destH);
                var sourceRect = new Rectangle(srcX, srcY, srcW, srcH);
                spriteBatch.Draw(texture, destRect, sourceRect, _color);
            }
        }
    }
}