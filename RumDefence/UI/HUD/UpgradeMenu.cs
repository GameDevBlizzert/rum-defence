using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.UI.Box;
using RumDefence.UI.Box.Components;

namespace RumDefence;

public class UpgradeMenu
{
    private const float TitleScale = 0.8f;
    private const float StatScale = 0.65f;
    private const float LabelScale = 0.6f;

    private Rectangle panelRect;
    private Box panel;

    private Box towerContent;
    private TextItem towerTitle;
    private TextItem damStat;
    private TextItem rngStat;
    private TextItem spdStat;
    private TextItem targetLabel;
    private ButtonBox targetModeButton;
    private TextItem costLabel;
    private ButtonBox upgradeButton;

    private LevelProgressSystem progress;
    public bool UpgradeClicked { get; private set; }
    public bool TargetModeClicked { get; private set; }
    public bool IsDisabled { get; set; }

    public BaseTower SelectedTower { get; set; }

    public bool IsMouseOver(Vector2 mousePos)
    {
        return panelRect.Contains(mousePos);
    }

    public UpgradeMenu(LevelProgressSystem progress)
    {
        this.progress = progress;

        int width = 400;
        int height = 500;
        int x = RumGame.VirtualWidth - width - 20; // right side
        int y = RumGame.VirtualHeight - height - 20; // bottom right
        panelRect = new Rectangle(x, y, width, height);

        BuildTowerContent();

        panel = new Box { Padding = 20 };
        panel.AddBackground(new ImageBox(Primitives.PanelTexture));
        panel.Add(towerContent);
        panel.Arrange(panelRect);
    }

    private void BuildTowerContent()
    {
        var buttonSize = new Vector2(panelRect.Width - 40, 52);

        towerTitle = new TextItem("", TitleScale);
        damStat = new TextItem("", StatScale);
        rngStat = new TextItem("", StatScale);
        spdStat = new TextItem("", StatScale);

        targetLabel = new TextItem("Target:", LabelScale);
        targetModeButton = new ButtonBox(Primitives.ButtonTexture, "Nearest", 0.72f, buttonSize);
        targetModeButton.OnClick = () => { TargetModeClicked = true; };

        costLabel = new TextItem("", StatScale);
        upgradeButton = new ButtonBox(Primitives.ButtonTexture, "Upgrade", 0.72f, buttonSize);
        upgradeButton.OnClick = () => { UpgradeClicked = true; };

        towerContent = new Box
        {
            Direction = Direction.Row,
            AlignX = Align.Start,
            AlignY = Align.Start,
            Gap = 12,
            Padding = 0
        };
        towerContent.Add(towerTitle);
        towerContent.Add(damStat);
        towerContent.Add(rngStat);
        towerContent.Add(spdStat);
        towerContent.Add(targetLabel);
        towerContent.Add(targetModeButton);
        towerContent.Add(costLabel);
        towerContent.Add(upgradeButton);
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
        upgradeButton.Label.Text = SelectedTower.CanUpgrade ? $"Upgrade ({upgradeKeyLabel})" : "MAX LEVEL";

        targetModeButton.Label.Text = SelectedTower.CurrentAttackModeLabel;
        targetModeButton.IsDisabled = false;
        upgradeButton.IsDisabled = !SelectedTower.CanUpgrade || progress.CoinsRemaining < SelectedTower.GetUpgradeCost();

        // This will potentially fire OnClick and set UpgradeClicked = true.
        targetModeButton.Update(gameTime);
        upgradeButton.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (SelectedTower == null)
            return;

        towerTitle.Text = SelectedTower.Label + $" LVL {SelectedTower.CurrentLevel + 1}";
        damStat.Text = $"DAM: {SelectedTower.CurrentDamage}";
        rngStat.Text = $"RNG: {(int)SelectedTower.CurrentRange}";
        spdStat.Text = $"SPD: {SelectedTower.CurrentFireRate:F1}/s";

        costLabel.Text = SelectedTower.CanUpgrade
            ? $"Cost: {SelectedTower.GetUpgradeCost()} coins"
            : "";

        panel.Arrange(panelRect);
        panel.Draw(spriteBatch);
    }
}
