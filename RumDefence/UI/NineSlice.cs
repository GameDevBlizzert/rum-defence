using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public static class NineSlice
{
    public static void Draw(SpriteBatch spriteBatch, Texture2D texture, Rectangle destination, Rectangle? source, int cornerSize, Color color)
    {
        var src = source ?? new Rectangle(0, 0, texture.Width, texture.Height);

        int innerW = destination.Width - cornerSize * 2;
        int innerH = destination.Height - cornerSize * 2;
        int srcInnerW = src.Width - cornerSize * 2;
        int srcInnerH = src.Height - cornerSize * 2;

        // Corners
        spriteBatch.Draw(texture, new Rectangle(destination.X, destination.Y, cornerSize, cornerSize),
            new Rectangle(src.X, src.Y, cornerSize, cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(destination.Right - cornerSize, destination.Y, cornerSize, cornerSize),
            new Rectangle(src.Right - cornerSize, src.Y, cornerSize, cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(destination.X, destination.Bottom - cornerSize, cornerSize, cornerSize),
            new Rectangle(src.X, src.Bottom - cornerSize, cornerSize, cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(destination.Right - cornerSize, destination.Bottom - cornerSize, cornerSize, cornerSize),
            new Rectangle(src.Right - cornerSize, src.Bottom - cornerSize, cornerSize, cornerSize), color);

        // Edges
        spriteBatch.Draw(texture, new Rectangle(destination.X + cornerSize, destination.Y, innerW, cornerSize),
            new Rectangle(src.X + cornerSize, src.Y, srcInnerW, cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(destination.X + cornerSize, destination.Bottom - cornerSize, innerW, cornerSize),
            new Rectangle(src.X + cornerSize, src.Bottom - cornerSize, srcInnerW, cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(destination.X, destination.Y + cornerSize, cornerSize, innerH),
            new Rectangle(src.X, src.Y + cornerSize, cornerSize, srcInnerH), color);
        spriteBatch.Draw(texture, new Rectangle(destination.Right - cornerSize, destination.Y + cornerSize, cornerSize, innerH),
            new Rectangle(src.Right - cornerSize, src.Y + cornerSize, cornerSize, srcInnerH), color);

        // Center
        spriteBatch.Draw(texture, new Rectangle(destination.X + cornerSize, destination.Y + cornerSize, innerW, innerH),
            new Rectangle(src.X + cornerSize, src.Y + cornerSize, srcInnerW, srcInnerH), color);
    }
}
