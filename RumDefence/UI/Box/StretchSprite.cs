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
        var src = source ?? new Rectangle(0, 0, texture.Width, texture.Height);
        var tileSize = (texture.Bounds.Size.ToVector2() / 3).ToPoint();

        var wt = texture.Bounds.Width;
        var ht = texture.Bounds.Height;

        var destSize = destination.Size;

        // var SameShape = destination.Size.ToVector2() % texture.Bounds.Size.ToVector2();
        float scaleX = 1f * destination.Size.X / texture.Bounds.Size.X;
        float scaleY = 1f * destination.Size.Y / texture.Bounds.Size.Y;
        // var a = 4 / texture.Bounds.Size.X;

        // Corners
        var sourceCorners = GetCorners(src, tileSize);
        var destCorners = GetCorners(destination, tileSize);
        for (int i = 0; i < sourceCorners.Length; i++)
        {
            spriteBatch.Draw(texture, destCorners[i], sourceCorners[i], _color);
        }

        // spriteBatch.Draw(texture, destCorners.Item1.Location.ToVector2(), sourceCorners.Item1, _color, rotation: 0f, origin: Vector2.Zero, scale: new Vector2(scaleX, scaleY), SpriteEffects.None, layerDepth: 1f);
        for (int x = 0; x < scaleX; x++)
        {
            for (int y = 0; y < scaleY; y++)
            {

            }
        }
        // Edges

        // Center

        // spriteBatch.Draw(texture, destination, _color);
    }
}