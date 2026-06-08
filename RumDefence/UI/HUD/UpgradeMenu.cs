using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class UpgradeMenu
{
    private Rectangle panelRect;
    private SimpleButton upgradeButton;
    private SimpleButton targetModeButton;
    private LevelProgressSystem progress;
    public bool UpgradeClicked { get; private set; }
    public bool TargetModeClicked { get; private set; }
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

        int width = 340;
        int height = 400;
        int x = RumGame.VirtualWidth - width - 20; // right side
        int y = RumGame.VirtualHeight - height - 20; // bottom right
        panelRect = new Rectangle(x, y, width, height);

        upgradeButton = new SimpleButton(Primitives.ButtonTexture, "Upgrade", new Vector2(x + 20, y + 315), new Vector2(width - 40, 52));
        upgradeButton.TextScale = 0.72f;
        upgradeButton.OnClick = () => { UpgradeClicked = true; };

        targetModeButton = new SimpleButton(Primitives.ButtonTexture, "Nearest", new Vector2(x + 20, y + 205), new Vector2(width - 40, 52));
        targetModeButton.TextScale = 0.72f;
        targetModeButton.OnClick = () => { TargetModeClicked = true; };
    }

    public void Update(GameTime gameTime)
    {
        UpgradeClicked = false;
        TargetModeClicked = false;

        if (IsDisabled)
            return;

        if (SelectedTower == null)
            return;

        if (InputManager.Instance.IsActionJustPressed("Upgrade"))
            UpgradeClicked = true;

        var upgradeKeyLabel = InputManager.GetKeyDisplayName(InputManager.Instance.GetBinding("Upgrade"));
        upgradeButton.Text = SelectedTower.CanUpgrade ? $"Upgrade ({upgradeKeyLabel})" : "MAX LEVEL";

        targetModeButton.Text = SelectedTower.CurrentAttackModeLabel;
        targetModeButton.IsDisabled = false;
        upgradeButton.IsDisabled = !SelectedTower.CanUpgrade || progress.CoinsRemaining < SelectedTower.GetUpgradeCost();

        // This will potentially fire OnClick and set UpgradeClicked = true.
        targetModeButton.Update(gameTime);
        upgradeButton.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (SelectedTower != null)
            DrawForTower(spriteBatch);
    }

    private void DrawForTower(SpriteBatch spriteBatch)
    {
        NineSlice.Draw(spriteBatch, Primitives.PanelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        var title = SelectedTower.Label;
        title += $" LVL {SelectedTower.CurrentLevel + 1}";

        float titleScale = 0.8f;
        spriteBatch.DrawString(Primitives.Font, title, new Vector2(panelRect.X + 20, panelRect.Y + 20), Primitives.FontColor, 0f, Vector2.Zero, titleScale, SpriteEffects.None, 0f);

        var startY = panelRect.Y + 70;
        var spacing = 34;
        var color = Primitives.FontColor;
        float statScale = 0.65f;

        spriteBatch.DrawString(Primitives.Font, $"DAM: {SelectedTower.CurrentDamage}", new Vector2(panelRect.X + 20, startY), color, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
        spriteBatch.DrawString(Primitives.Font, $"RNG: {(int)SelectedTower.CurrentRange}", new Vector2(panelRect.X + 20, startY + spacing), color, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
        spriteBatch.DrawString(Primitives.Font, $"SPD: {SelectedTower.CurrentFireRate:F1}/s", new Vector2(panelRect.X + 20, startY + spacing * 2), color, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);

        spriteBatch.DrawString(
            Primitives.Font,
            "Target:",
            new Vector2(panelRect.X + 20, panelRect.Y + 166),
            Primitives.FontColor,
            0f,
            Vector2.Zero,
            0.6f,
            SpriteEffects.None,
            0f
        );
        targetModeButton.Draw(spriteBatch);

        if (SelectedTower.CanUpgrade)
        {
            var cost = SelectedTower.GetUpgradeCost();
            spriteBatch.DrawString(Primitives.Font, $"Cost: {cost} coins", new Vector2(panelRect.X + 20, panelRect.Y + 258), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
            upgradeButton.Draw(spriteBatch);
        }
        else
        {
            spriteBatch.DrawString(Primitives.Font, "MAX LEVEL", new Vector2(panelRect.X + 20, panelRect.Y + 258), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
        }
    }

    private void DrawPreview(SpriteBatch spriteBatch)
    {
        NineSlice.Draw(spriteBatch, Primitives.PanelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

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
