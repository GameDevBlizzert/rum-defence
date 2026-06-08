using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.UI.Box;

namespace RumDefence;

public class HudHealthBar
{
    private readonly LevelProgressSystem progress;
    private readonly ProgressBarBox bar;

    public HudHealthBar(LevelProgressSystem progress, Rectangle barBounds)
    {
        this.progress = progress;

        bar = new ProgressBarBox
        {
            TrackColor = new Color(60, 0, 0),
            FillColorFunc = pct => Color.Lerp(Color.Red, Color.LimeGreen, pct)
        };
        bar.Arrange(barBounds);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        bar.Progress = progress.LivesTotal > 0
            ? MathHelper.Clamp((float)progress.LivesRemaining / progress.LivesTotal, 0f, 1f)
            : 0f;
        bar.Label = $"{progress.LivesRemaining}";
        bar.Draw(spriteBatch);
    }
}
