using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.UI.Box;

namespace RumDefence;

public class WaveHud
{
    private readonly ShipSpawner spawner;
    private readonly ProgressBarBox bar;

    private const int PanelPaddingX = 24;
    private const int PanelPaddingY = 10;
    private const int LineSpacing = 6;
    private const int BarHeight = 18;
    private const int BarMinWidth = 120;

    public WaveHud(ShipSpawner spawner)
    {
        this.spawner = spawner;
        bar = new ProgressBarBox
        {
            TrackColor = new Color(60, 60, 0),
            FillColor = Color.Yellow
        };
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        string waveText = $"Wave {spawner.CurrentWave} / {spawner.TotalWaves}";
        float progress = spawner.IsFinished ? 1f : spawner.WaveTroopProgress;
        string pctText = spawner.IsFinished ? "All waves complete!" : $"{(int)(progress * 100)}%";

        var waveSize = Primitives.Font.MeasureString(waveText);
        var pctSize = Primitives.Font.MeasureString(pctText);

        float innerWidth = spawner.IsFinished
            ? Math.Max(waveSize.X, pctSize.X)
            : Math.Max(Math.Max(waveSize.X, BarMinWidth), pctSize.X);

        float panelWidth = innerWidth + PanelPaddingX * 2;
        float panelHeight = waveSize.Y + pctSize.Y;
        float panelX = (RumGame.VirtualWidth - panelWidth) / 2f;
        float panelY = 20f;

        var panelRect = new Rectangle((int)panelX, (int)panelY, (int)panelWidth, (int)panelHeight);
        spriteBatch.Draw(Primitives.Pixel, panelRect, Color.Black * 0.55f);

        float waveX = panelX + (panelWidth - waveSize.X) / 2f;
        float waveY = panelY + PanelPaddingY;
        spriteBatch.DrawString(Primitives.Font, waveText, new Vector2(waveX, waveY), Primitives.FontColor);

        float contentY = waveY + waveSize.Y + LineSpacing;

        if (spawner.IsFinished)
        {
            float pctX = panelX + (panelWidth - pctSize.X) / 2f;
            spriteBatch.DrawString(Primitives.Font, pctText, new Vector2(pctX, contentY), Primitives.FontColor);
        }
        else
        {
            float barX = panelX + PanelPaddingX;
            float barWidth = panelWidth - PanelPaddingX * 2;

            bar.Progress = progress;
            bar.Label = pctText;
            bar.Arrange(new Rectangle((int)barX, (int)contentY, (int)barWidth, BarHeight));
            bar.Draw(spriteBatch);
        }
    }
}
