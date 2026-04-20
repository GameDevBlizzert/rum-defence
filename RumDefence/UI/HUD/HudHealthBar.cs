using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class HudHealthBar
{
    private readonly LevelProgressSystem progress;
        
    private readonly SpriteFont font;
    private readonly Texture2D pixel;

    private readonly Rectangle barBounds;

    public HudHealthBar(LevelProgressSystem progress, Rectangle barBounds)
    {
        this.progress = progress;
        font = RumGame.Instance.Content.Load<SpriteFont>("Fonts/KenneyFuture");
        this.barBounds = barBounds;

        pixel = new Texture2D(RumGame.Instance.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        float healthPct = progress.LivesTotal > 0
            ? MathHelper.Clamp((float)progress.LivesRemaining / progress.LivesTotal, 0f, 1f)
            : 0f;

        var barColor = Color.Lerp(Color.Red, Color.LimeGreen, healthPct);

        // Background
        spriteBatch.Draw(pixel, barBounds, new Color(60, 0, 0));

        // Health fill
        var fillRect = new Rectangle(barBounds.X, barBounds.Y, (int)(barBounds.Width * healthPct), barBounds.Height);
        spriteBatch.Draw(pixel, fillRect, barColor);

        // Label: "X / Y"
        var label = $"{progress.LivesRemaining} / {progress.LivesTotal}";
        var labelSize = font.MeasureString(label);
        var labelPos = new Vector2(
            barBounds.X + (barBounds.Width - labelSize.X) / 2f,
            barBounds.Y + (barBounds.Height - labelSize.Y) / 2f
        );
        spriteBatch.DrawString(font, label, labelPos, Color.White);
    }
}
