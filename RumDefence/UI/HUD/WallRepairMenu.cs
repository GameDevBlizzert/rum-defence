using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RumDefence;

public class WallRepairMenu
{
    private Texture2D panelTexture;
    private Rectangle panelRect;
    private SimpleButton repairButton;
    private SimpleButton upgradeButton;
    private LevelProgressSystem progress;

    public bool RepairClicked { get; private set; }
    public bool UpgradeClicked { get; private set; }
    public bool IsDisabled { get; set; }

    public Wall SelectedWall { get; set; }

    public bool IsMouseOver(Vector2 mousePos)
    {
        return panelRect.Contains(mousePos);
    }

    public WallRepairMenu(LevelProgressSystem progress)
    {
        this.progress = progress;

        var content = RumGame.Instance.Content;
        panelTexture = content.Load<Texture2D>("Art/UI/Panels/panel");
        var buttonTexture = content.Load<Texture2D>("Art/UI/Buttons/button");

        int width = 340;
        int height = 390;
        int x = RumGame.VirtualWidth - width - 20;
        int y = RumGame.VirtualHeight - height - 320;
        panelRect = new Rectangle(x, y, width, height);

        repairButton = new SimpleButton(buttonTexture, "Repair", Vector2.Zero, new Vector2(width - 40, 52));
        repairButton.SetBounds(new Rectangle(panelRect.X + 20, panelRect.Y + 150, width - 40, 52));
        repairButton.TextScale = 0.72f;
        repairButton.OnClick = () => { RepairClicked = true; };

        upgradeButton = new SimpleButton(buttonTexture, "Upgrade", Vector2.Zero, new Vector2(width - 40, 52));
        upgradeButton.SetBounds(new Rectangle(panelRect.X + 20, panelRect.Y + 308, width - 40, 52));
        upgradeButton.TextScale = 0.72f;
        upgradeButton.OnClick = () => { UpgradeClicked = true; };
    }

    public void Update(GameTime gameTime)
    {
        RepairClicked = false;
        UpgradeClicked = false;

        if (IsDisabled || SelectedWall == null)
            return;

        if (InputManager.Instance.IsActionJustPressed("Repair"))
            RepairClicked = true;

        if (InputManager.Instance.IsActionJustPressed("Upgrade"))
            UpgradeClicked = true;

        var repairKeyLabel = InputManager.GetKeyDisplayName(InputManager.Instance.GetBinding("Repair"));
        var upgradeKeyLabel = InputManager.GetKeyDisplayName(InputManager.Instance.GetBinding("Upgrade"));

        int repairCost = SelectedWall.GetRepairCostToFull();
        repairButton.IsDisabled = SelectedWall.IsDestroyed || !SelectedWall.IsDamaged || progress.CoinsRemaining < repairCost || repairCost <= 0;
        repairButton.Text = $"Repair ({repairKeyLabel})";

        int upgradeCost = SelectedWall.GetUpgradeCost();
        upgradeButton.IsDisabled = !SelectedWall.CanUpgrade || progress.CoinsRemaining < upgradeCost;
        upgradeButton.Text = SelectedWall.CanUpgrade ? $"Upgrade ({upgradeKeyLabel})" : "MAX LEVEL";

        repairButton.Update(gameTime);
        upgradeButton.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (SelectedWall == null) return;

        NineSlice.Draw(spriteBatch, panelTexture, panelRect, new Rectangle(0, 0, 128, 128), 20, Color.White);

        // Title + level
        string title = SelectedWall.UpgradeLevel > 0
            ? $"Wall  LVL {SelectedWall.UpgradeLevel}"
            : "Wall";
        spriteBatch.DrawString(Primitives.Font, title, new Vector2(panelRect.X + 20, panelRect.Y + 18), Primitives.FontColor, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);

        // Level pip indicators
        DrawLevelPips(spriteBatch);

        float statScale = 0.65f;

        // HP stat
        spriteBatch.DrawString(Primitives.Font, $"HP:  {SelectedWall.Health} / {SelectedWall.MaxHealth}", new Vector2(panelRect.X + 20, panelRect.Y + 82), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);

        // ── REPAIR ──────────────────────────────────────────────────
        spriteBatch.DrawString(Primitives.Font, "REPAIR", new Vector2(panelRect.X + 20, panelRect.Y + 114), Color.Gray, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

        int repairCost = SelectedWall.GetRepairCostToFull();
        string repairLine = (SelectedWall.IsDamaged && repairCost > 0)
            ? $"Cost: {repairCost} coins"
            : "Full health";
        spriteBatch.DrawString(Primitives.Font, repairLine, new Vector2(panelRect.X + 20, panelRect.Y + 128), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);

        repairButton.Draw(spriteBatch);

        // ── UPGRADE ─────────────────────────────────────────────────
        spriteBatch.DrawString(Primitives.Font, "UPGRADE", new Vector2(panelRect.X + 20, panelRect.Y + 225), Color.Gray, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

        if (SelectedWall.CanUpgrade)
        {
            int upgradeCost = SelectedWall.GetUpgradeCost();
            int hpGain = SelectedWall.NextMaxHealth - SelectedWall.MaxHealth;
            spriteBatch.DrawString(Primitives.Font, $"Cost: {upgradeCost} coins", new Vector2(panelRect.X + 20, panelRect.Y + 242), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(Primitives.Font, $"HP: {SelectedWall.MaxHealth} -> {SelectedWall.NextMaxHealth}  (+{hpGain})", new Vector2(panelRect.X + 20, panelRect.Y + 268), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
        }
        else
        {
            spriteBatch.DrawString(Primitives.Font, "Max level reached", new Vector2(panelRect.X + 20, panelRect.Y + 242), Primitives.FontColor, 0f, Vector2.Zero, statScale, SpriteEffects.None, 0f);
        }

        upgradeButton.Draw(spriteBatch);
    }

    private void DrawLevelPips(SpriteBatch spriteBatch)
    {
        const int pipSize = 14;
        const int pipGap = 6;
        int startX = panelRect.X + 20;
        int pipY = panelRect.Y + 52;

        for (int i = 0; i < Wall.MaxUpgradeLevel; i++)
        {
            int x = startX + i * (pipSize + pipGap);
            // Dark outline so pips are visible on any background
            spriteBatch.Draw(Primitives.Pixel, new Rectangle(x - 1, pipY - 1, pipSize + 2, pipSize + 2), Color.Black);
            Color pipColor = i < SelectedWall.UpgradeLevel ? GetPipColor(i) : new Color(70, 70, 70);
            spriteBatch.Draw(Primitives.Pixel, new Rectangle(x, pipY, pipSize, pipSize), pipColor);
        }
    }

    private static Color GetPipColor(int level) => level switch
    {
        0 => new Color(200, 148, 80),  // bronze
        1 => new Color(148, 188, 220), // silver
        2 => new Color(250, 205, 50),  // gold
        _ => Color.White
    };
}
