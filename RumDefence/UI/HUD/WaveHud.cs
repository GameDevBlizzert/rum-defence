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
        string subText = spawner.IsFinished
            ? "All waves complete!"
            : $"{(int)spawner.WaveTimeRemaining} sec";

        var waveSize = font.MeasureString(waveText);
        var subSize = font.MeasureString(subText);

        float panelWidth = Math.Max(waveSize.X, subSize.X) + PanelPaddingX * 2;
        float panelHeight = waveSize.Y + LineSpacing + subSize.Y + PanelPaddingY * 2;
        float panelX = (RumGame.VirtualWidth - panelWidth) / 2f;
        float panelY = 20f;

        var panelRect = new Rectangle((int)panelX, (int)panelY, (int)panelWidth, (int)panelHeight);
        spriteBatch.Draw(pixel, panelRect, Color.Black * 0.55f);

        float waveX = panelX + (panelWidth - waveSize.X) / 2f;
        float waveY = panelY + PanelPaddingY;
        spriteBatch.DrawString(font, waveText, new Vector2(waveX, waveY), Color.White);

        Color subColor = spawner.IsFinished ? Color.White : Color.Yellow;
        float subX = panelX + (panelWidth - subSize.X) / 2f;
        float subY = waveY + waveSize.Y + LineSpacing;
        spriteBatch.DrawString(font, subText, new Vector2(subX, subY), subColor);
    }
}
