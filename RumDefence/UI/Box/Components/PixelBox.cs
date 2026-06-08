using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence.UI.Box;

public class PixelBox : IBox
{
    public new Color Color { get; set; }
    private Vector2 size;
    public PixelBox(Color color, int width = 0, int height = 0)
    {
        Color = color;
        size = new(width, height);
    }
    public override Vector2 Measure()
    {
        return size;
    }
    public override void DrawBox(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Primitives.Pixel, Slot, Color);
    }
}
