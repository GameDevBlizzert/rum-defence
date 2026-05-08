using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class WaveHud
{
    private readonly ShipSpawner spawner;
    private readonly SpriteFont font;
    private readonly Texture2D pixel;

    private const int PanelPaddingX = 24;
    private const int PanelPaddingY = 10;
    private const int LineSpacing = 6;
    private const int BarHeight = 18;
    private const int BarMinWidth = 120;

    public WaveHud(ShipSpawner spawner)
    {
        this.spawner = spawner;
        font = RumGame.Instance.Content.Load<SpriteFont>("Fonts/KenneyFuture");

        pixel = new Texture2D(RumGame.Instance.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        string waveText = $"Wave {spawner.CurrentWave} / {spawner.TotalWaves}";
        float progress = spawner.IsFinished ? 1f : spawner.WaveTroopProgress;
        string pctText = spawner.IsFinished ? "All waves complete!" : $"{(int)(progress * 100)}%";

        var waveSize = font.MeasureString(waveText);
        var pctSize = font.MeasureString(pctText);

        float innerWidth = spawner.IsFinished
            ? Math.Max(waveSize.X, pctSize.X)
            : Math.Max(Math.Max(waveSize.X, BarMinWidth), pctSize.X);

        float panelWidth = innerWidth + PanelPaddingX * 2;
        float panelHeight = waveSize.Y + LineSpacing + (spawner.IsFinished ? pctSize.Y : BarHeight) + PanelPaddingY * 2;
        float panelX = (RumGame.VirtualWidth - panelWidth) / 2f;
        float panelY = 20f;

        var panelRect = new Rectangle((int)panelX, (int)panelY, (int)panelWidth, (int)panelHeight);
        spriteBatch.Draw(pixel, panelRect, Color.Black * 0.55f);

        float waveX = panelX + (panelWidth - waveSize.X) / 2f;
        float waveY = panelY + PanelPaddingY;
        spriteBatch.DrawString(font, waveText, new Vector2(waveX, waveY), Color.White);

        float contentY = waveY + waveSize.Y + LineSpacing;

        if (spawner.IsFinished)
        {
            float pctX = panelX + (panelWidth - pctSize.X) / 2f;
            spriteBatch.DrawString(font, pctText, new Vector2(pctX, contentY), Color.White);
        }
        else
        {
            float barX = panelX + PanelPaddingX;
            float barWidth = panelWidth - PanelPaddingX * 2;

            var barBgRect = new Rectangle((int)barX, (int)contentY, (int)barWidth, BarHeight);
            var barFillRect = new Rectangle((int)barX, (int)contentY, (int)(barWidth * progress), BarHeight);

            spriteBatch.Draw(pixel, barBgRect, new Color(60, 60, 0));
            spriteBatch.Draw(pixel, barFillRect, Color.Yellow);

            var pctPos = new Vector2(
                barX + (barWidth - pctSize.X) / 2f,
                contentY + (BarHeight - pctSize.Y) / 2f
            );
            spriteBatch.DrawString(font, pctText, pctPos, Color.White);
        }
    }
}
