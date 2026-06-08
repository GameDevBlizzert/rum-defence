using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RumDefence.UI.Box;
using RumDefence.UI.Box;

namespace RumDefence;

public class WallRepairMenu
{
    private const float TitleScale = 0.8f;
    private const float StatScale = 0.65f;
    private const float LabelScale = 0.5f;

    private Rectangle panelRect;
    private Box panel;
    private Box content;

    private TextItem title;
    private TextItem hpStat;

    private TextItem repairCostText;
    private ButtonBox repairButton;

    private TextItem upgradeCostText;
    private TextItem upgradeHpText;
    private ButtonBox upgradeButton;

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

        int width = 400;
        int height = 500;
        int x = RumGame.VirtualWidth - width - 20;
        int y = RumGame.VirtualHeight - height - 20;
        panelRect = new Rectangle(x, y, width, height);

        BuildContent();

        panel = new Box { Direction = Direction.Row, AlignX = Align.Start, AlignY = Align.Start, Padding = 20 };
        panel.AddBackground(new ImageBox(Primitives.PanelTexture));
        panel.Add(content);
        panel.Arrange(panelRect);
    }

    private void BuildContent()
    {
        var buttonSize = new Vector2(panelRect.Width - 40, 52);

        title = new TextItem("", TitleScale);

        hpStat = new TextItem("", StatScale);

        var repairLabel = new TextItem("REPAIR", LabelScale, Color.Gray);
        repairCostText = new TextItem("", StatScale);
        repairButton = new ButtonBox(Primitives.ButtonTexture, "Repair", 0.72f, buttonSize);
        repairButton.OnClick = () => { RepairClicked = true; };

        var repairSection = new Box { Direction = Direction.Row, AlignX = Align.Start, AlignY = Align.Start, Gap = 6, Padding = 0 };
        repairSection.Add(repairLabel);
        repairSection.Add(repairCostText);
        repairSection.Add(repairButton);

        var upgradeLabel = new TextItem("UPGRADE", LabelScale, Color.Gray);
        upgradeCostText = new TextItem("", StatScale);
        upgradeHpText = new TextItem("", StatScale);
        upgradeButton = new ButtonBox(Primitives.ButtonTexture, "Upgrade", 0.72f, buttonSize);
        upgradeButton.OnClick = () => { UpgradeClicked = true; };

        var upgradeSection = new Box { Direction = Direction.Row, AlignX = Align.Start, AlignY = Align.Start, Gap = 6, Padding = 0 };
        upgradeSection.Add(upgradeLabel);
        upgradeSection.Add(upgradeCostText);
        upgradeSection.Add(upgradeHpText);
        upgradeSection.Add(upgradeButton);

        content = new Box { Direction = Direction.Row, AlignX = Align.Start, AlignY = Align.Start, Gap = 14, Padding = 0 };
        content.Add(title);
        content.Add(hpStat);
        content.Add(repairSection);
        content.Add(upgradeSection);
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
        repairButton.Label.Text = $"Repair ({repairKeyLabel})";

        int upgradeCost = SelectedWall.GetUpgradeCost();
        upgradeButton.IsDisabled = !SelectedWall.CanUpgrade || progress.CoinsRemaining < upgradeCost;
        upgradeButton.Label.Text = SelectedWall.CanUpgrade ? $"Upgrade ({upgradeKeyLabel})" : "MAX LEVEL";

        repairButton.Update(gameTime);
        upgradeButton.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (SelectedWall == null) return;

        title.Text = SelectedWall.UpgradeLevel > 0
            ? $"Wall  LVL {SelectedWall.UpgradeLevel}"
            : "Wall";
        hpStat.Text = $"HP:  {SelectedWall.Health} / {SelectedWall.MaxHealth}";

        int repairCost = SelectedWall.GetRepairCostToFull();
        repairCostText.Text = (SelectedWall.IsDamaged && repairCost > 0)
            ? $"Cost: {repairCost} coins"
            : "Full health";

        if (SelectedWall.CanUpgrade)
        {
            int upgradeCost = SelectedWall.GetUpgradeCost();
            int hpGain = SelectedWall.NextMaxHealth - SelectedWall.MaxHealth;
            upgradeCostText.Text = $"Cost: {upgradeCost} coins";
            upgradeHpText.Text = $"HP: {SelectedWall.MaxHealth} -> {SelectedWall.NextMaxHealth}  (+{hpGain})";
        }
        else
        {
            upgradeCostText.Text = "Max level reached";
            upgradeHpText.Text = "";
        }

        panel.Arrange(panelRect);
        panel.Draw(spriteBatch);
    }
}
