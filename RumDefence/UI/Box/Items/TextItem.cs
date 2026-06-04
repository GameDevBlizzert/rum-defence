using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence.UI.Box;

public class TextItem : IBoxItem
{
    public string Text { get; set; }
    public float Scale { get; set; }
    public Color Color { get; set; } = Color.White;
    private Rectangle slot;
    public TextItem(string text, float scale = 1f, Color? color = null)
    {
        Text = text;
        Scale = scale;
        Color = color ?? Primitives.FontColor;
    }
    public Vector2 Measure()
    {
        var s = Primitives.Font.MeasureString(Text);
        return new Vector2(s.X * Scale, s.Y * Scale);
    }
    public void Arrange(Rectangle rect) => slot = rect;
    public void Update(GameTime gameTime) { }
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(
            Primitives.Font,
            Text,
            new Vector2(slot.X, slot.Y),
            Color,
            0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
    }
}
