using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence.UI.Box;

public class ImageBox : IBox
{
    public Texture2D Texture { get; set; }
    public float Scale { get; set; } = 1f;
    private Vector2 size;
    private Rectangle? srcRectangle = null;
    public ImageBox(Texture2D texture2D, Rectangle? source = null)
    {
        Texture = texture2D;
        srcRectangle = source;
    }

    public ImageBox(Texture2D texture2D, int width, int height, Rectangle? source = null)
    {
        Texture = texture2D;
        size = new(width, height);
        srcRectangle = source;
    }
    public override Vector2 Measure()
    {
        return size;
    }
    public override void DrawBox(SpriteBatch spriteBatch)
    {
        StretchSprite.Draw(spriteBatch, Texture, Slot, color: Color);
        // NineSlice.Draw(spriteBatch, Texture, Slot, new(0, 0, Texture.Width, Texture.Height), 20, Color);
    }
}
