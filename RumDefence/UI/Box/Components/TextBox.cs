using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence.UI.Box.Components;

public class TextItem : IBox
{
    public string Text { get; set; } = "";
    public float Scale { get; set; } = 1f;
    public TextItem()
    {
        Color = Primitives.FontColor;
    }
    public TextItem(string text, float scale = 1f, Color? color = null)
    {
        if (color != null)
            Color = (Color)color;
        else
            Color = Primitives.FontColor;
        Text = text;
        Scale = scale;
    }
    public override Vector2 Measure()
    {
        var s = Primitives.Font.MeasureString(Text);
        return new Vector2(s.X * Scale, s.Y * Scale);
    }
    public override void DrawBox(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(
            Primitives.Font,
            Text,
            new Vector2(Slot.X, Slot.Y),
            Color,
            0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
    }
}
