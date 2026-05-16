using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class UpgradeMenu
{
    private Texture2D panelTexture;

    private Rectangle panelRect;
    private SimpleButton upgradeButton;
    private LevelProgressSystem progress;
    public bool UpgradeClicked { get; private set; }
    public bool IsDisabled { get; set; }

    public BaseTower SelectedTower { get; set; }
    public TowerData PreviewData { get; set; }

    public bool IsMouseOver(Vector2 mousePos)
    {
        return panelRect.Contains(mousePos);
    }

    public UpgradeMenu(LevelProgressSystem progress)
    {
        this.progress = progress;

        var content = RumGame.Instance.Content;
        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel");
        var buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button");

        int width = 340;
        int height = 300;
        int x = RumGame.VirtualWidth - width - 20; // right side
        int y = RumGame.VirtualHeight - height - 20; // bottom right
        panelRect = new Rectangle(x, y, width, height);

        upgradeButton = new SimpleButton(buttonTexture, "Upgrade", new Vector2(x + 20, y + 210), new Vector2(width - 40, 70));
        upgradeButton.OnClick = () => { UpgradeClicked = true; };
    }

    public void Update(GameTime gameTime)
    {
        // Don't reset UpgradeClicked immediately here, or GameScreen won't see it if polled after.
        // It's safer to provide a Consume method or let the click handler set it to true and the consumer reset it.
        // But since we are setting it here and GameScreen checks it in the same frame, we must ensure ordering.
        // Wait, if we set UpgradeClicked = false here... the OnClick delegates fire DURING upgradeButton.Update!
        // So putting it BEFORE upgradeButton.Update() is correct.

        UpgradeClicked = false;

        if (IsDisabled)
            return;

        if (SelectedTower == null)
            return;

        upgradeButton.IsDisabled = !SelectedTower.CanUpgrade || progress.CoinsRemaining < SelectedTower.GetUpgradeCost();

        // This will potentially fire OnClick and set UpgradeClicked = true.
        upgradeButton.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (SelectedTower != null)
            DrawForTower(spriteBatch);
        else if (PreviewData != null)
            DrawPreview(spriteBatch);
    }

    private void DrawForTower(SpriteBatch spriteBatch)
    {
        NineSlice.Draw(spriteBatch, panelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        var title = SelectedTower.Label;
        title += $" LVL {SelectedTower.CurrentLevel + 1}";

        float titleScale = 0.8f;
        spriteBatch.DrawString(Primitives.Font, title, new Vector2(panelRect.X + 20, panelRect.Y + 20), Primitives.FontColor, 0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

        var startY = panelRect.Y + 70;
        var spacing = 35;
        var color = Primitives.FontColor;
        float statScale = 0.65f;

        spriteBatch.DrawString(Primitives.Font, $"DAM: {SelectedTower.CurrentDamage}", new Vector2(panelRect.X + 20, startY), color, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
        spriteBatch.DrawString(Primitives.Font, $"RNG: {(int)SelectedTower.CurrentRange}", new Vector2(panelRect.X + 20, startY + spacing), color, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
        spriteBatch.DrawString(Primitives.Font, $"SPD: {SelectedTower.CurrentFireRate:F1}/s", new Vector2(panelRect.X + 20, startY + spacing * 2), color, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);

        if (SelectedTower.CanUpgrade)
        {
            var cost = SelectedTower.GetUpgradeCost();
            spriteBatch.DrawString(Primitives.Font, $"Cost: {cost} coins", new Vector2(panelRect.X + 20, startY + spacing * 3), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
            upgradeButton.Draw(spriteBatch);
        }
        else
        {
            spriteBatch.DrawString(Primitives.Font, "MAX LEVEL", new Vector2(panelRect.X + 20, startY + spacing * 3), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
        }
    }

    private void DrawPreview(SpriteBatch spriteBatch)
    {
        NineSlice.Draw(spriteBatch, panelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        var title = PreviewData.Label + " LVL 1";
        float titleScale = 0.8f;
        spriteBatch.DrawString(Primitives.Font, title, new Vector2(panelRect.X + 20, panelRect.Y + 20), Primitives.FontColor, 0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

        var startY = panelRect.Y + 70;
        var spacing = 35;
        var color = Primitives.FontColor;
        float statScale = 0.65f;

        spriteBatch.DrawString(Primitives.Font, $"DAM: {PreviewData.Damage}", new Vector2(panelRect.X + 20, startY), color, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
        spriteBatch.DrawString(Primitives.Font, $"RNG: {(int)PreviewData.Range}", new Vector2(panelRect.X + 20, startY + spacing), color, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
        spriteBatch.DrawString(Primitives.Font, $"SPD: {PreviewData.FireRate:F1}/s", new Vector2(panelRect.X + 20, startY + spacing * 2), color, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
        spriteBatch.DrawString(Primitives.Font, $"Cost: {PreviewData.Cost} coins", new Vector2(panelRect.X + 20, startY + spacing * 3), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
    }
}
