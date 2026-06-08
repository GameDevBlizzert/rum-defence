using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RumDefence.UI.Box.Components;

public class ProgressBarBox : IBox
{
    public float Progress { get; set; }
    public Color TrackColor { get; set; } = new Color(60, 60, 60);
    public Color FillColor { get; set; } = Color.White;
    public Func<float, Color> FillColorFunc { get; set; }
    public string Label { get; set; }

    // Fixed size so a parent Box can lay this bar out alongside its siblings.
    public Vector2 Size { get; set; }

    public override Vector2 Measure() => Size;

    public override void DrawBox(SpriteBatch spriteBatch)
    {
        var pct = MathHelper.Clamp(Progress, 0f, 1f);
        var fillColor = FillColorFunc?.Invoke(pct) ?? FillColor;

        spriteBatch.Draw(Primitives.Pixel, Slot, TrackColor);

        var fillRect = new Rectangle(Slot.X, Slot.Y, (int)(Slot.Width * pct), Slot.Height);
        spriteBatch.Draw(Primitives.Pixel, fillRect, fillColor);

        if (string.IsNullOrEmpty(Label))
            return;

        var labelSize = Primitives.Font.MeasureString(Label);
        var labelPos = new Vector2(
            Slot.X + (Slot.Width - labelSize.X) / 2f,
            Slot.Y + (Slot.Height - labelSize.Y) / 2f
        );
        spriteBatch.DrawString(Primitives.Font, Label, labelPos, Primitives.FontColor);
    }
}
