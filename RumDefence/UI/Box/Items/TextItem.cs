using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence.UI.Box;

public class TextItem : BoxItem
{
    public string Text { get; set; }
    public float Scale { get; set; }
    public TextItem(string text, float scale = 1f, Color? color = null)
    {
        Text = text;
        Scale = scale;
        Color = color ?? Primitives.FontColor;
    }
    public override Vector2 Measure()
    {
        var s = Primitives.Font.MeasureString(Text);
        return new Vector2(s.X * Scale, s.Y * Scale);
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        // wip
        spriteBatch.Draw(Primitives.Pixel, Slot, Color.Red);
        spriteBatch.DrawString(
            Primitives.Font,
            Text,
            new Vector2(Slot.X, Slot.Y),
            Color,
            0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
    }
}
