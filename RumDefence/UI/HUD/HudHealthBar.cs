using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class HudHealthBar
{
    private readonly LevelProgressSystem progress;

    private readonly Rectangle barBounds;

    public HudHealthBar(LevelProgressSystem progress, Rectangle barBounds)
    {
        this.progress = progress;
        this.barBounds = barBounds;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        float healthPct = progress.LivesTotal > 0
            ? MathHelper.Clamp((float)progress.LivesRemaining / progress.LivesTotal, 0f, 1f)
            : 0f;

        var barColor = Color.Lerp(Color.Red, Color.LimeGreen, healthPct);

        // Background
        spriteBatch.Draw(Primitives.Pixel, barBounds, new Color(60, 0, 0));

        // Health fill
        var fillRect = new Rectangle(barBounds.X, barBounds.Y, (int)(barBounds.Width * healthPct), barBounds.Height);
        spriteBatch.Draw(Primitives.Pixel, fillRect, barColor);

        // Label: "X / Y"
        var label = $"{progress.LivesRemaining}";
        var labelSize = Primitives.Font.MeasureString(label);
        var labelPos = new Vector2(
            barBounds.X + (barBounds.Width - labelSize.X) / 2f,
            barBounds.Y + (barBounds.Height - labelSize.Y) / 2f
        );
        spriteBatch.DrawString(Primitives.Font, label, labelPos, Color.White);
    }
}
